/*
* Copyright (c) CookApps.
* 이진호(jhlee8@cookapps.com)
*/

using Facebook.Unity;
using UniRx;
using UnityEngine;

/// <summary>
/// 페이스북 초기화 완료
/// </summary>
public readonly struct MsgFaceBookInitialized
{
}

namespace SH.RuntimeInit
{
    namespace RB
    {
        public static class RuntimeInitFB
        {
            [RuntimeInitializeOnLoadMethod]
            private static void RuntimeInit()
            {
                InitActivateApp();
            
                Observable.EveryApplicationPause()
                    .Subscribe(pauseStatus =>
                    {
                        if (!pauseStatus) 
                        {
                            InitActivateApp();
                        }
                    });
            }

            private static void InitActivateApp()
            {
                if (FB.IsInitialized) 
                {
                    FB.ActivateApp();
                } 
                else
                {
                    FB.Init(() =>
                    {
                        Debug.Log("FB Initialized");
                        var ate = PlayerPrefs.GetInt("ApplePermission_ATE", 0) == 1;
                        FB.Mobile.SetAdvertiserTrackingEnabled(ate);
                        FB.ActivateApp();
                        MessageBroker.Default.Publish(new MsgFaceBookInitialized());
                    });
                }
            }
        }
    }
}