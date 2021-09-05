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
    public class ButtonApple : ButtonPlatform
    {
        private void Awake()
        {
#if !UNITY_IOS
            gameObject.Destroy();            
#endif
        }
        
        protected override void OnClick()
        {
            if (string.IsNullOrEmpty(PlatformLoginManager.Instance.AppleId))
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
            return !string.IsNullOrEmpty(PlatformLoginManager.Instance.AppleId);
        }
        
        protected override PlatformLoginType GetPlatformType() => PlatformLoginType.Apple;

        private async void TryLogin()
        {
            using var indicator = ScopeIndicator();
            var id = await PlatformLoginManager.Instance.LoginApple();
            if (string.IsNullOrEmpty(id))
            {
                FailedLogin();
                return;
            }

            UserDataSaveServerManager.Instance?.SetState(false);
            if (await AsyncServer(new PlatformLoginId(apple:id)))
            {
                PlatformLoginManager.Instance.LogoutAll();
                PlatformLoginManager.Instance.SetAppleId(id);    
            }
            else
            {
                UserDataSaveServerManager.Instance?.SetState(true);
                FailedLogin();
            }
        }
    }
}