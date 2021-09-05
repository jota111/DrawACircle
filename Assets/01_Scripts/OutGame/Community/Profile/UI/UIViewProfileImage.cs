using System;
using System.Collections;
using System.Collections.Generic;
using Community.Default;
using Cysharp.Threading.Tasks;
using Doozy.Engine.UI;
using OutGameCore;
using SH.Game.Manager;
using SH.Game.UserData;
using SH.UI.View.Dialog;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Community.Profile
{
    public class UIViewProfileImage : UIViewNotice
    {
        public static string selectedKey = "";
        private static string beforeKey = "";
        [SerializeField] private List<UIItem_ProfileImage> ProfileImageList = new List<UIItem_ProfileImage>();

        public static UIViewProfileImage Instance { get; private set; }

        [Inject]
        public void Construct()
        {
            Instance = this;
        }

        void Awake()
        {
            Btn_Close.OnClickAsObservable().Subscribe(_ => OnCloseButtonClicked()).AddTo(this);
        }

        protected override void OnStartHide()
        {
            base.OnStartHide();
            selectedKey = beforeKey;
            onCloseAction?.Invoke();
            onCloseAction = null;
            // TODO : 유저데이터에 저장
            // UserData.Instance.
        }

        public void SetView(Action _onCloseAction = null)
        {
            onCloseAction = _onCloseAction;
            if (string.IsNullOrEmpty(beforeKey))
            {
                beforeKey = "Sunny";
            }

            foreach (var item in ProfileImageList)
            {
                item.SetSelected(item.Key.Equals(beforeKey));
            }
            Show();
        }

        public void SetSelected(string key)
        {
            selectedKey = key;
            foreach (var item in ProfileImageList)
            {
                item.SetSelected(item.Key.Equals(key));
            }
        }

        private async UniTask<int> TrySetProfileImage(string key)
        {
            int checker = 1;
            var chance = GameUtils.RandomBoolFromPercent(0.3f);
            if (chance)
            {
                checker = 2;
                // checker = GameUtils.RandomBoolFromPercent(0.3f) ? 0 : 2;
            }
            return checker;
        }
        
        /// <summary>
        /// 프로필 사진 변경 시도
        /// 0: 실패, 1: 성공
        /// </summary>
        public override void OnCloseButtonClicked()
        {
            GameSoundManager.Instance.PlaySfx(SFX.sh_common_click);
            if (selectedKey.Equals(beforeKey))
            {
                HideSelf();
                return;
            }
            
            UniTask.Void(async () =>
            {
                var checker = await TrySetProfileImage(selectedKey);
                switch (checker)
                {
                    case 0:
                        UIViewDialog.Instance.DialogForget( "MailBox_NetworkFailDescription", "UINameKey_Friends", "Dialog_OK", true, false);
                        break;
                    case 1:
                        beforeKey = selectedKey;
                        HideSelf();
                        break;
                }
            });
        }
    }
}
