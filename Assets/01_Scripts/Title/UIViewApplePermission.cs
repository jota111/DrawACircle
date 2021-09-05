/*
* Copyright (c) CookApps.
* 이진호(jhlee8@cookapps.com)
*/

using System;
using Balaso;
using Cysharp.Threading.Tasks;
using Doozy.Engine.UI;
using Facebook.Unity;
using Sirenix.OdinInspector;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

#if UNITY_IOS
using SH.AppEvent;
using Unity.Notifications.iOS;
#endif

namespace SH.Title
{
    [RequireComponent(typeof(UIView))]
    public class UIViewApplePermission : MonoBehaviour
    {
        private const string KeyNotification = "ApplePermission_Notification";
        private const string KeyATT = "ApplePermission_ATT";

        [SerializeField, Required, ChildGameObjectsOnly]
        private Button _buttonOK;

        private bool _isFinished;

        private void Start()
        {
            _buttonOK.OnClickAsObservable()
                .First()
                .Subscribe(_ => OnClickRun())
                .AddTo(this);
        }

        private async void OnClickRun()
        {
#if UNITY_IOS && !UNITY_EDITOR
            if (!PlayerPrefs.HasKey(KeyATT))
            {
                var attFinished = false;
                Observable.FromEvent<AppTrackingTransparency.AuthorizationStatus>(
                        h => AppTrackingTransparency.OnAuthorizationRequestDone += h,
                        h => AppTrackingTransparency.OnAuthorizationRequestDone -= h)
                    .Subscribe(status =>
                    {
                        attFinished = true;
                        var allow = status == AppTrackingTransparency.AuthorizationStatus.AUTHORIZED;
                        if (allow)
                        {
                            PlayerPrefs.SetInt("ApplePermission_ATE", 1);
                            if (FB.IsInitialized)
                            {
                                FB.Mobile.SetAdvertiserTrackingEnabled(true);   
                            }
                        }
                        AppEventManager.LogEvent_Lobby_ATT_POPUP(allow);
                    })
                    .AddTo(this);
                
                AppTrackingTransparency.RequestTrackingAuthorization();
                await UniTask.WaitWhile(() => !attFinished);
                PlayerPrefs.SetInt(KeyATT, 1);
                await UniTask.Delay(TimeSpan.FromSeconds(0.5f));
            }
            
            if (!PlayerPrefs.HasKey(KeyNotification))
            {
                using var reqNotification = new AuthorizationRequest(AuthorizationOption.Alert, false);
                await UniTask.WaitWhile(() => !reqNotification.IsFinished);
                PlayerPrefs.SetInt(KeyNotification, 1);
            }
#endif
            GetComponent<UIView>().Hide();
            await UniTask.Delay(TimeSpan.FromSeconds(0.5f));
            _isFinished = true;
        }


        public async UniTask CheckAndShow()
        {
#if UNITY_IOS && !UNITY_EDITOR
            if (PlayerPrefs.HasKey(KeyNotification) && PlayerPrefs.HasKey(KeyATT))
            {
                return;
            }
            GetComponent<UIView>().Show();

#else
            return;
#endif

            await UniTask.WaitWhile(() => !_isFinished);
        }
    }
}