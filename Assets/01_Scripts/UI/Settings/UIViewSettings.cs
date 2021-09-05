using System.Collections;
using System.Collections.Generic;
using OutGameCore;
using SH.AppEvent;
using SH.Constant;
using SH.Game.DataLocal;
using SH.Game.Manager;
using SH.Game.UserData;
using SH.Game.UserManager;
using SH.UI.View;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace SH.Game.Settings
{
    public class UIViewSettings : UIViewBase
    {
        [Inject] private readonly UserData.UserData userData;

        private SettingsLocal _settingsLocal
        {
            get => userData.Settings;
            set => userData.Settings = value;
        }

        [SerializeField] private GameObject[] Obj_SoundEffect = new GameObject[2];
        [SerializeField] private GameObject[] Obj_BGM = new GameObject[2];
        [SerializeField] private GameObject[] Obj_Haptic = new GameObject[2];
        [SerializeField] private Text Text_Version;
        [SerializeField] private Text Text_PlayerID;
        [SerializeField] private Button Btn_SoundEffect, Btn_BGM, Btn_Haptic;
        [SerializeField] private Button Btn_Notification, Btn_Language;
        //[SerializeField] private Button Btn_TransferUser;

        [Inject] private UIViewNotification _uiViewNotification;
        [Inject] private UIViewLanguage _uiViewLanguage;
        //[Inject] private UIViewTransferStart _uiViewTransferStart;
        [Inject] private GameSoundManager _gameSoundManager;
        //[Inject] private UserData.UserData _userData;
        
        private void Awake()
        {
            Btn_SoundEffect.OnClickAsObservable().Subscribe(_ => { OnSEButtonClicked(); }).AddTo(this);
            Btn_BGM.OnClickAsObservable().Subscribe(_ => { OnBGMButtonClicked(); }).AddTo(this);
            if(Btn_Haptic != null)
                Btn_Haptic.OnClickAsObservable().Subscribe(_ => { OnHapticButtonClicked(); }).AddTo(this);
            
            Btn_Notification.OnClickAsObservable().Subscribe(_ => { OnNotificationButtonClicked(); }).AddTo(this);
            Btn_Language.OnClickAsObservable().Subscribe(_ => { OnLanguageButtonClicked(); }).AddTo(this);
            //Btn_TransferUser.OnClickAsObservable().Subscribe(_ => { OnTransferButtonClicked(); }).AddTo(this);
        }

        protected override void OnStartShow()
        {
            base.OnStartShow();
            SetButtons();
            SetTexts();
        }

        private void SetButtons()
        {
            //Btn_TransferUser.gameObject.SetActive(!_userData.TransferUser);
            Obj_SoundEffect[0].SetActive(!_settingsLocal.OptionSoundEffect.Value);
            Obj_SoundEffect[1].SetActive(_settingsLocal.OptionSoundEffect.Value);
            Obj_BGM[0].SetActive(!_settingsLocal.OptionBGM.Value);
            Obj_BGM[1].SetActive(_settingsLocal.OptionBGM.Value);
            if(Obj_Haptic.Length > 0 && Obj_Haptic[0] != null)
                Obj_Haptic[0].SetActive(!_settingsLocal.OptionHaptic.Value);
            if(Obj_Haptic.Length > 1 && Obj_Haptic[1] != null)
                Obj_Haptic[1].SetActive(_settingsLocal.OptionHaptic.Value);
        }

        private void SetTexts()
        {
            // TODO : 버전이랑 플레이어 아이디
            GameUtils.I2FormatSetParams(Text_Version, StaticSet.Version);
            //GameUtils.I2FormatSetParams(Text_PlayerID, UserIdManager.Instance.Id);
            Text_PlayerID.text = $"UID {UserIdManager.Instance.Id}";
        }

        public void OnBGMButtonClicked()
        {
            _gameSoundManager.PlaySfx(SFX.sh_common_click);
            _settingsLocal.OptionBGM.Value = !_settingsLocal.OptionBGM.Value;
            SetButtons();
        }

        public void OnSEButtonClicked()
        {
            _gameSoundManager.PlaySfx(SFX.sh_common_click);
            _settingsLocal.OptionSoundEffect.Value = !_settingsLocal.OptionSoundEffect.Value;
            SetButtons();
        }
        
        public void OnHapticButtonClicked()
        {
            _gameSoundManager.PlaySfx(SFX.sh_common_click);
            _settingsLocal.OptionHaptic.Value = !_settingsLocal.OptionHaptic.Value;
            SetButtons();
        }

        public void OnLanguageButtonClicked()
        {
            _gameSoundManager.PlaySfx(SFX.sh_common_click);
            _uiViewLanguage.Show();
        }

        public void OnNotificationButtonClicked()
        {
            _gameSoundManager.PlaySfx(SFX.sh_common_click);
            _uiViewNotification.Show();
        }

        // public void OnTransferButtonClicked()
        // {
        //     #region AppEvent
        //     
        //     if(ScreenManager.Instance.ScreenState.Value == ScreenState.Lobby)
        //         AppEventManager.LogEvent_Lobby_ButtonClicked(AppEventManager.LobbyButtonCategory.ETC);
        //     
        //     #endregion
        //     
        //     _gameSoundManager.PlaySfx(SFX.sh_common_click);
        //     _uiViewTransferStart.Show();
        // }
    }
}