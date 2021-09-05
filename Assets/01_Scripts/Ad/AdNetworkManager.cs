/*
* Copyright (c) CookApps.
* 이진호(jhlee8@cookapps.com)
*/

using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using OutGameCore;
using SH.Ad.Impl;
using SH.AppEvent;
using SH.Util;
using UniRx;
using UnityEngine;

namespace SH.Ad
{
    public class AdNetworkManager
    {
        private static AdNetworkManager _instance = null;
        public static AdNetworkManager Instance => _instance ??= new AdNetworkManager();

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ReloadDomain()
        {
            _instance = null;
        }

        [RuntimeInitializeOnLoadMethod]
        private static void RuntimeInit()
        {
            Instance.Init();
        }
        
        /// <summary>
        /// 보상형 로딩?
        /// </summary>
        public IReadOnlyReactiveProperty<bool> IsLoadedReward { get; private set; }
        private readonly List<IAdNetwork> listAdNetworks = new List<IAdNetwork>();
        
        private void Init()
        {
#if UNITY_AWS && !UNITY_EDITOR
            var mopubAmazon = new MoPubAmazon();
            listAdNetworks.Add(mopubAmazon);
            IsLoadedReward = listAdNetworks.Select(ads => ads.IsLoadedReward).CombineLatest().Select(ads => ads.Any(x => x)).ToReadOnlyReactiveProperty();
            listAdNetworks.ForEach(ad => ad.Init(OnOpenAd, OnCloseAd));
#elif (UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR
            var appLovinMax = new AppLovinMax();
            listAdNetworks.Add(appLovinMax);
            IsLoadedReward = listAdNetworks.Select(ads => ads.IsLoadedReward).CombineLatest().Select(ads => ads.Any(x => x)).ToReadOnlyReactiveProperty();
            listAdNetworks.ForEach(ad => ad.Init(OnOpenAd, OnCloseAd));        
#endif
#if  UNITY_EDITOR
            IsLoadedReward = new ReactiveProperty<bool>(true);
#endif
            IsLoadedReward.Subscribe(load => Debug.Log($"Reward Ad ready : {load}"));
        }
        
        /// <summary>
        /// 보상형 광고
        /// </summary>
        /// <param name="rewardCallBack"></param>
        /// <returns></returns>
        public bool ShowRewardAd(Action<bool> rewardCallBack, AppEventManager.RewardAdsType type = AppEventManager.RewardAdsType.etc)
        {
#if UNITY_EDITOR
            rewardCallBack?.Invoke(true);
            return true;
#endif
            foreach (var ad in listAdNetworks)
            {
                if (ad.IsLoadedReward.Value)
                {
                    ad.ShowRewardAd(rewardCallBack, type.ToString());
                    return true;
                }
            }

            ShowNotReadyReward();
            
            return false;
        }
        
        public async UniTask<bool> AsyncShowRewardAd(AppEventManager.RewardAdsType rewardAdsType = AppEventManager.RewardAdsType.etc)
        {
            var utcs = new UniTaskCompletionSource<bool>();
            ShowRewardAd(result => utcs.TrySetResult(result), rewardAdsType);
            var value = await utcs.Task;
            return value;
        }
        
        private void ShowNotReadyReward()
        {
            var msg = GameUtils.I2Format("Toast_text_adnotready");
            ToastHelper.Show(msg);
        }

        private void OnOpenAd()
        {
            if(AudioController.Instance != null)
                AudioController.Instance.soundMuted = true;
        }
        
        private void OnCloseAd()
        {
            if(AudioController.Instance != null)
                AudioController.Instance.soundMuted = false;
        }
    }
}