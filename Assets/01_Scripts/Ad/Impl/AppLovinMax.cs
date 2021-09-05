/*
* Copyright (c) CookApps.
* 이진호(jhlee8@cookapps.com)
*/

using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace SH.Ad.Impl
{
    public sealed class AppLovinMax : AdNetwork
    {
        
        private const string MaxSdkKey = "GieITmgGZW1s4hAO95Vi_ng7mPDu0FRHz4ZVqFnNnoEsTPEcUlKUV0cGQq8Q4ai1fHXTWIsI458TUMA-5UTbU4";
        
#if UNITY_ANDROID
        private string RewardedAdUnitId = "1ad10b678126b101";
#elif UNITY_IOS
        private string RewardedAdUnitId = "78ae310b6dc74c44";
#else
        private string RewardedAdUnitId = "1ad10b678126b101";
#endif
        

        private bool _isInit = false;
        
        protected override async void Init()
        {
            while (Application.internetReachability == NetworkReachability.NotReachable)
            {
                await UniTask.Delay(TimeSpan.FromSeconds(1));
            }

            MaxSdkCallbacks.Rewarded.OnAdLoadedEvent += (s, info) => OnLoadReward(info.NetworkName);
            MaxSdkCallbacks.Rewarded.OnAdLoadFailedEvent += (s, info) => OnFailedLoadReward(info.Message);
            MaxSdkCallbacks.Rewarded.OnAdHiddenEvent += (s, info) => OnCloseReward(info.NetworkName);
            MaxSdkCallbacks.Rewarded.OnAdDisplayedEvent += (s, info) => OnOpenReward(info.NetworkName);
            MaxSdkCallbacks.Rewarded.OnAdReceivedRewardEvent += (s, reward, info) => OnSuccessReward(info.NetworkName);
            
            MaxSdkCallbacks.OnSdkInitializedEvent += configuration =>
            {
                _isInit = true;
                Debug.Log("MAX SDK Initialized");
                AdLoadReward();
            };
            
            MaxSdk.SetSdkKey(MaxSdkKey);
            MaxSdk.InitializeSdk();
        }

        protected override void AdShowReward()
        {
            if (!_isInit)
                return;
            
            if (MaxSdk.IsRewardedAdReady(RewardedAdUnitId))
            {
                MaxSdk.ShowRewardedAd(RewardedAdUnitId);
            }
        }

        protected override void AdLoadReward()
        {
            if (!_isInit)
                return;
            
            MaxSdk.LoadRewardedAd(RewardedAdUnitId);
        }
    }
}