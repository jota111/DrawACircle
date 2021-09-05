/*
* Copyright (c) CookApps.
* 이진호(jhlee8@cookapps.com)
*/

using System;
using Cysharp.Threading.Tasks;
using OutGameCore;
using SH.Constant;
using SH.Game.UserData;
using SH.Game.UserManager;
using SH.Net;
using SH.Net.Pkt;
using SH.Ob;
using SH.Util;
using UniRx;
using Zenject;

namespace SH.Game.Misc
{
    public sealed class AppFocusCheck : IDisposable, IInitializable
    {
        private static readonly TimeSpan TimeWaitTitle = TimeSpan.FromHours(4);
        private static readonly TimeSpan TimeSkip = TimeSpan.FromMinutes(3);
        private readonly IIndicator _indicator;
        private readonly UserData.UserData _userData;
        private DateTime? _back;
        private IDisposable _disposable;

        public AppFocusCheck(IIndicator indicator, UserData.UserData userData)
        {
            _indicator = indicator;
            _userData = userData;
        }
        
        public void Initialize()
        {
            _disposable = Observable.EveryApplicationPause().Subscribe(OnApplicationPause);
        }
        
        public void Dispose()
        {
            _disposable?.Dispose();
            _disposable = null;
        }
        
        
        private async void OnApplicationPause(bool pause)
        {
            if (!pause)
            {
                if (_back.HasValue)
                {
                    var diff = DateTime.Now - _back.Value;
                    if (TimeWaitTitle < diff)
                    {
                        MoveToTitle().Forget();
                    }
                    else if(diff < TimeSkip || await IsLatestLocalData()) // TimeSkip 보다 작거나 로컬이 최신 데이타이면 그냥 진행
                    {
                        // 저장기능 활성
                        UserDataSaveManager.Instance.EnableAutoSave = true;
                        UserDataSaveServerManager.Instance.SetState(true);
                    }
                    else
                    {
                        MoveToTitle().Forget();
                    }
                }
                _back = null;
            }
            else
            {
                _back = DateTime.Now;
                // 저장기능 비활성
                UserDataSaveManager.Instance.EnableAutoSave = false;
                UserDataSaveServerManager.Instance.SetState(false);
            }
        }

        private async UniTask<bool> IsLatestLocalData()
        {
            if (UserIdManager.Instance.EmptyId)
                return true;
            
            using var indicator = _indicator.ScopeLock();
            
            var recv = await NetManager.AsyncPost<RecvLogin>(new ReqLogin());
            if (!recv.result)
                return true;

            var localIsLatest = _userData.LocalIsLatest(recv.data.saveData);
            return localIsLatest;
        }

        private async UniTaskVoid MoveToTitle()
        {
            if (!AppCheck.IsPass.Value)
                return;
            
            Dispose();
            var indicator = _indicator.ScopeLock();
            var delay = UniTask.Delay(TimeSpan.FromSeconds(1)).AsAsyncUnitUniTask();
            var (loadScene, _) = await UniTask.WhenAll(SceneLoad.LoadSceneAsync(SceneType.SceneTitle), delay);
            indicator.Dispose();
            loadScene.allowSceneActivation = true;
        }
    }
}