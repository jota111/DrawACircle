/*
* Copyright (c) CookApps.
* 이진호(jhlee8@cookapps.com)
*/

using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace SH.Ad.Impl
{
    /// <summary>
    /// 아마존 스토어를 위한 모펍
    /// </summary>
    public sealed class MoPubAmazon : AdNetwork
    {
        private string RewardedAdUnitId = "d17d6d0c06ac4111b7f22249ff04af3f";
        
        private bool _isInit = false;
        
        protected override async void Init()
        {
            while (Application.internetReachability == NetworkReachability.NotReachable)
            {
                await UniTask.Delay(TimeSpan.FromSeconds(1));
            }
            
            MoPubManager.OnRewardedVideoLoadedEvent += OnLoadReward;
            MoPubManager.OnRewardedVideoFailedEvent += (adUnitId, error) => OnFailedLoadReward(error);
            MoPubManager.OnRewardedVideoShownEvent += OnOpenReward;
            MoPubManager.OnRewardedVideoClosedEvent += OnCloseReward;
            MoPubManager.OnRewardedVideoReceivedRewardEvent += (adUnitId, label, amount) => OnSuccessReward(adUnitId);
            MoPubManager.OnSdkInitializedEvent += s =>
            {
                _isInit = true;
                Debug.Log("MoPubAmazon SDK Initialized");
                AdLoadReward();
            };

            MoPub.InitializeSdk(RewardedAdUnitId);
            var  adUnitIds = new[] {RewardedAdUnitId};
            MoPub.LoadRewardedVideoPluginsForAdUnits(adUnitIds);
        }

        protected override void AdShowReward()
        {
            if (!_isInit)
                return;

            if (!MoPub.HasRewardedVideo(RewardedAdUnitId))
                return;
            
            MoPub.ShowRewardedVideo(RewardedAdUnitId);   
        }

        protected override void AdLoadReward()
        {
            if (!_isInit)
                return;
            
            MoPub.RequestRewardedVideo(RewardedAdUnitId);
        }
    }
}