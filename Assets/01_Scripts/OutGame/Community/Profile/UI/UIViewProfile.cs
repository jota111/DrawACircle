using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Community.Default;
using Cysharp.Threading.Tasks;
using Doozy.Engine.UI;
using OutGameCore;
using SH.Game.DataLocal;
using SH.Game.Manager;
using SH.Game.UserData;
using SH.Net;
using SH.Net.Pkt;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Community.Profile
{
    public class UIViewProfile : UIViewNotice
    {
        public static UIViewProfile Instance { get; private set; }
        [SerializeField] private Image Img_Portrait;
        [SerializeField] private Text Text_Nickname;
        [SerializeField] private Text Text_Level;
        
        [SerializeField] private Button Btn_EditPortrait;
        [SerializeField] private Button Btn_EditNickname;
        private ProfileDataLocal _profileData;

        [Inject]
        public void Construct(UserData userData)
        {
            Instance = this;
            _profileData = userData.ProfileData;
        }

        void Awake()
        {
            Btn_Close.OnClickAsObservable().Subscribe(_ => OnCloseButtonClicked()).AddTo(this);
            // Btn_EditPortrait.OnClickAsObservable().Subscribe(_ => OnPortraitEditButtonClicked()).AddTo(this);
            Btn_EditNickname.OnClickAsObservable().Subscribe(_ => OnNicknameEditButtonClicked()).AddTo(this);
        }

        public void SetView()
        {
            if (string.IsNullOrEmpty(_profileData.NickName.Value) == false)
                Text_Nickname.text = UIViewSetNickname.myNickName;
            else Text_Nickname.text = _profileData.NickName.Value;
            
            Text_Level.text = $"Lv.10";
            
            var selected = UIViewProfileImage.selectedKey;
            if (string.IsNullOrEmpty(selected))
            {
                selected = "Sunny";
            }
            Img_Portrait.sprite = Resources.Load<Sprite>($"OutGame/Dialogue/{selected}");
            Show();
        }
        
        public void OnPortraitEditButtonClicked()
        {
            GameSoundManager.Instance.PlaySfx(SFX.sh_common_click);
            UIViewProfileImage.Instance
                .SetView(() =>
                {
                    Img_Portrait.sprite = Resources.Load<Sprite>($"OutGame/Dialogue/{UIViewProfileImage.selectedKey}");
                });
        }

        public void OnNicknameEditButtonClicked()
        {
            GameSoundManager.Instance.PlaySfx(SFX.sh_common_click);
            UIViewSetNickname.Instance
                .SetView(() =>
                {
                    Text_Nickname.text = UIViewSetNickname.myNickName;
                });
        }

        public override void OnCloseButtonClicked()
        {
            GameSoundManager.Instance.PlaySfx(SFX.sh_common_click);
            onCloseAction?.Invoke();
            onCloseAction = null;
            Hide();
        }
    }
}
