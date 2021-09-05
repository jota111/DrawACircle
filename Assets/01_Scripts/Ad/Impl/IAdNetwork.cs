/*
* Copyright (c) CookApps.
* 이진호(jhlee8@cookapps.com)
*/

using System;
using OutGameCore;
using SH.AppEvent;
using UniRx;

namespace SH.Ad.Impl
{
    public interface IAdNetwork
    {
        void Init(Action open, Action close);
        IReadOnlyReactiveProperty<bool> IsLoadedReward { get; }
        void ShowRewardAd(Action<bool> rewardCallBack, string AdsType);
    }
    
    public abstract class AdNetwork : IAdNetwork
    {
        public enum eAppEvent
        {
            reward_request_failed,
            reward_request_success,
            reward_start,
            reward_close,
            reward_complete,
        }

        private Action _actionOpen;
        private Action _actionClose;
        private Action<bool> _actionReward;
        private bool _successReward;
        protected double _delayFailed = 2.0;
        protected double _delayCompleted = 0.1;
        private int _retryCount = int.MaxValue;
        private int _retryReward = 0;
        private readonly ReactiveProperty<bool> _isLoadedReward = new ReactiveProperty<bool>();
        protected abstract void Init();
        protected abstract void AdShowReward();
        protected abstract void AdLoadReward();
        public IReadOnlyReactiveProperty<bool> IsLoadedReward
        {
            get { return _isLoadedReward; }
        }

        private string _adsType;

        public void Init(Action open, Action close)
        {
            _actionOpen = open;
            _actionClose = close;
            Init();
        }

        #region Reward

        public void ShowRewardAd(Action<bool> rewardCallBack, string AdsType)
        {
            if (_isLoadedReward.Value)
            {
                _isLoadedReward.Value = false;
                _successReward = false;
                _actionReward = rewardCallBack;
                _adsType = AdsType;
                AdShowReward();
            }
        }

        protected void OnOpenReward(string mediation)
        {
            Observable.NextFrame()
                .Subscribe(_ =>
                {
                    if (_actionOpen != null)
                        _actionOpen.Invoke();
                    
                    LogAppEvent(eAppEvent.reward_start);
                });
        }

        protected void OnCloseReward(string mediation)
        {
            Observable.NextFrame()
                .Subscribe(_ =>
                {
                    if (_actionClose != null)
                        _actionClose.Invoke();

                    _actionReward?.Invoke(_successReward);
                    if (!_successReward)
                        LogAppEvent(eAppEvent.reward_close);

                    _successReward = false;
                    _actionReward = null;

                    Observable.Timer(TimeSpan.FromSeconds(_delayCompleted), Scheduler.MainThreadIgnoreTimeScale)
                        .Subscribe(__ =>
                        {
                            AdLoadReward();
                        }); 
                });
        }

        protected void OnLoadReward(string param)
        {
            _isLoadedReward.Value = true;
            LogAppEvent(eAppEvent.reward_request_success);
        }

        protected void OnFailedLoadReward(string error)
        {
            _isLoadedReward.Value = false;
            _retryReward++;
            
            Debug.Log(string.Format("ad load failed reward({0}) - {1}/{2}", error, _retryReward, _retryCount));

            if (_retryCount > _retryReward)
            {
                Observable.Timer(TimeSpan.FromSeconds(_delayFailed), Scheduler.MainThreadIgnoreTimeScale)
                    .Subscribe(_ =>
                    {
                        AdLoadReward();
                        LogAppEvent(eAppEvent.reward_request_failed);
                        _adsType = "";
                    });
            }
        }

        protected void OnSuccessReward(string param)
        {
            _successReward = true;
            LogAppEvent(eAppEvent.reward_complete);
            _adsType = "";
        }

        #endregion

        private void LogAppEvent(eAppEvent logEvent)
        {
            // if (FirebaseAnalyticsInitialize.IsInitialized)
            // {
            //     FirebaseAnalytics.LogEvent(logEvent.ToString(), "mediation", mediation ?? "none");
            // }
            GameUtils.Log($"IAdNetwork LogAppEvent {logEvent} {_adsType}");
        }
    }
}