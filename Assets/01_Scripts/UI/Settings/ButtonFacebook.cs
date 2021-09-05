/*
* Copyright (c) CookApps.
* 이진호(jhlee8@cookapps.com)
*/

using SH.Game.UserData;
using SH.Game.UserManager;
using SH.Platform;

namespace SH.UI.Settings
{
    public class ButtonFacebook : ButtonPlatform
    {
        protected override void OnClick()
        {
            if (string.IsNullOrEmpty(PlatformLoginManager.Instance.FacebookId))
            {
                TryLogin();
            }
            else
            {
                AlreadyLogin();
            }
        }
        
        protected override PlatformLoginType GetPlatformType() => PlatformLoginType.Facebook;
        protected override bool IsLogin()
        {
            return !string.IsNullOrEmpty(PlatformLoginManager.Instance.FacebookId);
        }

        private async void TryLogin()
        {
            using var indicator = ScopeIndicator();
            var id = await PlatformLoginManager.Instance.LoginFacebook();
            if (string.IsNullOrEmpty(id))
            {
                FailedLogin();
                return;
            }

            UserDataSaveServerManager.Instance?.SetState(false);
            if (await AsyncServer(new PlatformLoginId(facebook:id)))
            {
                PlatformLoginManager.Instance.LogoutAll();
                PlatformLoginManager.Instance.SetFacebookId(id);    
            }
            else
            {
                UserDataSaveServerManager.Instance?.SetState(true);
                FailedLogin();
            }
        }
    }
}