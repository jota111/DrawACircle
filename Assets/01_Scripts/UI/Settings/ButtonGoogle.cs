/*
* Copyright (c) CookApps.
* 이진호(jhlee8@cookapps.com)
*/

using SH.Game.UserData;
using SH.Game.UserManager;
using SH.Platform;
using Unity.Linq;

namespace SH.UI.Settings
{
    public class ButtonGoogle : ButtonPlatform
    {
        private void Awake()
        {
#if !UNITY_ANDROID
            gameObject.Destroy();            
#endif
        }

        protected override void OnClick()
        {
            if (string.IsNullOrEmpty(PlatformLoginManager.Instance.GoogleId))
            {
                TryLogin();
            }
            else
            {
                AlreadyLogin();
            }
        }
        
        protected override bool IsLogin()
        {
            return !string.IsNullOrEmpty(PlatformLoginManager.Instance.GoogleId);
        }

        protected override PlatformLoginType GetPlatformType() => PlatformLoginType.Google;

        private async void TryLogin()
        {
            using var indicator = ScopeIndicator();
            var id = await PlatformLoginManager.Instance.LoginGoogle();
            if (string.IsNullOrEmpty(id))
            {
                FailedLogin();
                return;
            }
            
            UserDataSaveServerManager.Instance?.SetState(false);
            if (await AsyncServer(new PlatformLoginId(google:id)))
            {
                PlatformLoginManager.Instance.LogoutAll();
                PlatformLoginManager.Instance.SetGoogleId(id);
            }
            else
            {
                UserDataSaveServerManager.Instance?.SetState(true);
                FailedLogin();
            }
        }
    }
}