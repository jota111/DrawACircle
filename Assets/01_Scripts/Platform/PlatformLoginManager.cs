/*
* Copyright (c) CookApps.
* 이진호(jhlee8@cookapps.com)
*/
//#if GOOGLE_GAME_PLAY

using Cysharp.Threading.Tasks;
using Facebook.Unity;
using SH.Util;
using UnityEngine;

#if UNITY_ANDROID && GOOGLE_GAME_PLAY
using GooglePlayGames;
using GooglePlayGames.BasicApi;
#endif

namespace SH.Platform
{
    public class PlatformLoginManager
    {
#if UNITY_ANDROID
        public string GoogleId { get; private set; }
        public string AppleId { get => string.Empty; private set { } }
#elif UNITY_IOS
        public string GoogleId { get => string.Empty; private set { } }
        public string AppleId { get; private set; }
#else
        public string GoogleId { get => string.Empty; private set { } }
        public string AppleId { get => string.Empty; private set { } }
#endif
        public string FacebookId { get; private set; }

        private const string KeyAppleId = "AppleId";
        private const string KeyGoogleId = "GoogleId";
        private const string KeyFacebookId = "FaceBookId";
        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ReloadDomain()
        {
            _instance = null;
        }
        
        private static PlatformLoginManager _instance;
        public static PlatformLoginManager Instance
        {
            get
            {
                _instance ??= new PlatformLoginManager();
                return _instance;
            }
        }

        [RuntimeInitializeOnLoadMethod]
        static void RuntimeInit()
        {
#if UNITY_ANDROID && GOOGLE_GAME_PLAY
            var config = new PlayGamesClientConfiguration.Builder().Build();
            PlayGamesPlatform.InitializeInstance(config);
#if __DEV
            PlayGamesPlatform.DebugLogEnabled = true;
#endif
            PlayGamesPlatform.Activate();
#endif
        }

        private PlatformLoginManager()
        {
            GoogleId = ES3.LoadString(KeyGoogleId, string.Empty);
            AppleId = ES3.LoadString(KeyAppleId, string.Empty);
            FacebookId = ES3.LoadString(KeyFacebookId, string.Empty);
        }

        public void LogoutAll()
        {
            GoogleId = string.Empty;
            SaveUtil.Save(KeyGoogleId, GoogleId);
            
            FacebookId = string.Empty;
            SaveUtil.Save(KeyGoogleId, FacebookId);
            
            AppleId = string.Empty;
            SaveUtil.Save(KeyGoogleId, AppleId);
        }

        public void SetGoogleId(string id)
        {
            LogoutAll();
            GoogleId = id;
            SaveUtil.Save(KeyGoogleId, string.IsNullOrEmpty(id) ? string.Empty : id);
        }
        
        public void SetFacebookId(string id)
        {
            LogoutAll();
            FacebookId = id;
            SaveUtil.Save(KeyFacebookId, string.IsNullOrEmpty(id) ? string.Empty : id);
        }

        public void SetAppleId(string id)
        {
            LogoutAll();
            AppleId = id;
            SaveUtil.Save(KeyAppleId, string.IsNullOrEmpty(id) ? string.Empty : id);
        }

        public async UniTask<string> LoginGoogle()
        {
#if UNITY_ANDROID && GOOGLE_GAME_PLAY
            if (!string.IsNullOrEmpty(GoogleId))
                return GoogleId;
            
            var end = false;
            string id = null;
            Social.localUser.Authenticate(success =>
            {
                id = success ? Social.localUser.id : string.Empty;
                end = true;
            });

            await UniTask.WaitUntil(() => end);
            return id;
#else
            await UniTask.NextFrame();
            return string.Empty;
#endif
        }

        public async UniTask<string> LoginFacebook()
        {
            if (FB.IsLoggedIn)
            {
                return AccessToken.CurrentAccessToken.UserId;
            }
            
            var end = false;
            string id = null;
            var permissions = new[] {"public_profile"};
            FB.LogInWithReadPermissions(permissions, LoginCallback);
            await UniTask.WaitUntil(() => end);
            return id;
            
            //-------------------------------------------------------------------------------
            void LoginCallback(ILoginResult loginResult)
            {
                id = (FB.IsLoggedIn && loginResult.AccessToken != null) ? loginResult.AccessToken.UserId : string.Empty;
                end = true;
            }
        }

        public async UniTask<string> LoginApple()
        {
            return await PlatformLoginApple.LoginApple();
        }
    }
}