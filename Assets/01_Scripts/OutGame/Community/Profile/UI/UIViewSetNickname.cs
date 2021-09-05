using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Doozy.Engine.UI;
using Community.Default;
using GameDataEditor;
using OutGameCore;
using SH.AppEvent;
using SH.Game.DataLocal;
using SH.Game.Manager;
using SH.Game.Misc;
using SH.Game.UserData;
using SH.Game.UserManager;
using SH.Net;
using SH.Net.Pkt;
using SH.UI.View.Dialog;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Community.Profile
{
    public class UIViewSetNickname : UIViewNotice
    {
        public const int MinText = 2;
        public const int MaxText = 15;
        public static string myNickName = "";
        public static UIViewSetNickname Instance { get; private set; }

        [SerializeField] private InputField _inputField;
        [SerializeField] private Text text_ConfirmName;
        [SerializeField] private Text text_NicknameTitle;
        [SerializeField] private Text text_NicknameBtn;
        [SerializeField] private Text text_NicknameCost;
        [SerializeField] private Text text_SubDescription;
        [SerializeField] private GameObject Obj_InputNickname;
        [SerializeField] private GameObject Obj_ConfirmNickname;
        [SerializeField] private Button Btn_Back;
        protected Action onChangeAction;

        private ProfileDataLocal _profileData;
        private State state = State.Input;
        private IIndicator _indicator;
        private int NicknameChangeCost = 50;

        public enum State
        {
            Input, // 새이름 입력
            Confirm, //확정
        }

        [Inject]
        public void Construct(UserData userData, IIndicator indicator)
        {
            Instance = this;
            _profileData = userData.ProfileData;
            _indicator = indicator;
            Btn_Back.OnClickAsObservable().Subscribe(_ =>
            {
                text_SubDescription.gameObject.SetActive(false);
                SetData(State.Input);
            }).AddTo(this);
        }

        void Awake()
        {
            Btn_Close.OnClickAsObservable().Subscribe(_ => OnCloseButtonClicked()).AddTo(this);
        }

        public void SetView(Action _onChagneAction = null, Action _onCloseAction = null)
        {
            text_SubDescription.gameObject.SetActive(false);
            onChangeAction = _onChagneAction;
            onCloseAction = _onCloseAction;
            SetData(State.Input);
        }

        private void SetData(State _state, string nickName = null)
        {
            text_SubDescription.gameObject.SetActive(false);
            this.state = _state;
            SetDescription(nickName);
            if (string.IsNullOrEmpty(_profileData.NickName.Value) == false)
                _inputField.text = _profileData.NickName.Value;
            else
                _inputField.text = "";
            Show();
        }

        private void SetDescription(string nickName = null)
        {
            Obj_InputNickname.SetActive(state == State.Input);
            Obj_ConfirmNickname.SetActive(state == State.Confirm);
            GameUtils.I2Format(text_NicknameTitle,
                state == State.Input
                    ? (_profileData.nickNameChangeCount.Value == 0 ? "UINameKey_initialTitle" : "UINickname_TitleKey")
                    : "UINickname_ConfirmTitleKey");
            GameUtils.I2Format(text_NicknameBtn, state == State.Input ? "Popup_btn_Continue" : "Text_OK");
            GameUtils.I2SetFont(text_ConfirmName, nickName);
            text_NicknameCost.gameObject.SetActive(state == State.Confirm && _profileData.nickNameChangeCount.Value > 1);
            text_NicknameBtn.gameObject.SetActive(state == State.Input || _profileData.nickNameChangeCount.Value <= 1);
            Btn_Back.gameObject.SetActive(state == State.Confirm);// && _profileData.nickNameChangeCount.Value > 1);
            switch (_profileData.nickNameChangeCount.Value)
            {
                case 0:
                    GameUtils.I2Format(Text_Description, "UINickname_ChangeFirst");
                    break;
                case 1:
                    GameUtils.I2Format(Text_Description, "UINickname_ChangeSecond");
                    break;
                case 2:
                    GameUtils.I2Format(Text_Description, "UINickname_ChangeThird");
                    break;
            }

            text_NicknameCost.text = NicknameChangeCost.ToString();
        }

        /// <summary>
        /// 닉네임 변경 시도
        /// 0: 실패, 1: 성공, 2: 중복
        /// </summary>
        private string _nickName;

        public override void OnCloseButtonClicked()
        {
            GameSoundManager.Instance.PlaySfx(SFX.sh_common_click);
            if (state == State.Input)
            {
                UniTask.Void(async () =>
                {
                    var tryNickName = _inputField.text;
                    if (_profileData.NickName != null && tryNickName.Equals(_profileData.NickName.Value))
                    {
                        GameUtils.I2Format(text_SubDescription, "UINickname_SameWarning");
                        await text_SubDescription.Fade(true, 0.25f);
                        await UniTask.Delay(TimeSpan.FromSeconds(0.5f));
                        await text_SubDescription.Fade(false, 0.25f);
                        return;
                    }

                    if (tryNickName.Length < MinText || tryNickName.Length > MaxText)
                    {
                        GameUtils.I2Format(text_SubDescription, "UINickname_LengthWarning2", UIViewSetNickname.MinText, UIViewSetNickname.MaxText);
                        await text_SubDescription.Fade(true, 0.25f);
                        await UniTask.Delay(TimeSpan.FromSeconds(0.5f));
                        await text_SubDescription.Fade(false, 0.25f);
                        return;
                    }

                    using var scope = _indicator.ScopeLock();
                    var recv = await NetManager.AsyncPost<RecvTrySetProfile>(new ReqTrySetProfile(tryNickName));
                    if (recv is {result: true} && state == State.Input)
                    {
                        _nickName = recv.data.clean_word;
                        myNickName = recv.data.clean_word;
                        SetData(State.Confirm, recv.data.clean_word);
                    }
                    else
                        UIViewDialog.Instance.DialogForget("MailBox_NetworkFailDescription", "UINameKey_Friends", "Dialog_OK", true, false);
                });
            }
            else if (state == State.Confirm)
            {
                UniTask.Void(async () =>
                {
                    using var scope = _indicator.ScopeLock();
                    var recv = await NetManager.AsyncPost<RecvSetProfile>(new ReqSetProfile(_nickName));
                    if (recv is {result: true} && state == State.Confirm)
                    {
                        if (_profileData.nickNameChangeCount.Value > 1)
                            FundManager.Instance.UseJewel(NicknameChangeCost);
                        _inputField.text = recv.data.clean_word;
                        myNickName = recv.data.clean_word;
                        _profileData.SetNickName(recv.data.clean_word);
                        UserDataSaveManager.Instance.Save();
                        UserDataSaveServerManager.Instance?.RequestAsync();
                        onChangeAction?.Invoke();
                        onChangeAction = null;
                        Hide();
                    }
                    else
                        UIViewDialog.Instance.DialogForget("MailBox_NetworkFailDescription", "UINameKey_Friends", "Dialog_OK", true, false);
                });
            }
        }
    }
}