/*
* Copyright (c) CookApps.
* 이진호(jhlee8@cookapps.com)
*/

using AppleAuth;
using AppleAuth.Enums;
using AppleAuth.Extensions;
using AppleAuth.Interfaces;
using AppleAuth.Native;
using Cysharp.Threading.Tasks;
using UniRx;

namespace SH.Platform
{
    public static class PlatformLoginApple
    {
        private static readonly AppleAuthManager _appleAuthManager;

        static PlatformLoginApple()
        {
            if (!AppleAuthManager.IsCurrentPlatformSupported)
                return;
            
            var deserializer = new PayloadDeserializer();
            // Creates an Apple Authentication manager with the deserializer
            _appleAuthManager = new AppleAuthManager(deserializer);

            Observable.EveryUpdate()
                .Subscribe(_ => _appleAuthManager.Update());
        }

        public static async UniTask<string> LoginApple()
        {
            if(_appleAuthManager == null)
                return string.Empty;
            
            var id = string.Empty;
            
            var end = false;
            var loginArgs = new AppleAuthLoginArgs(LoginOptions.None);
            _appleAuthManager.LoginWithAppleId(
                loginArgs,
                credential =>
                {
                    // Obtained credential, cast it to IAppleIDCredential
                    if (credential is IAppleIDCredential appleIdCredential)
                    {
                        // Apple User ID
                        id = appleIdCredential.User;
                    }
                    end = true;
                },
                error =>
                {
                    // Something went wrong
                    id = string.Empty;
                    var authorizationErrorCode = error.GetAuthorizationErrorCode();
                    end = true;
                });
            
            await UniTask.WaitUntil(() => end);
            return id;
        }
    }
}