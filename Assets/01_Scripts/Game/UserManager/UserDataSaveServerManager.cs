/*
* Copyright (c) CookApps.
* 이진호(jhlee8@cookapps.com)
*/

using System;
using SH.Game.InGame.Msg;
using SH.Game.Misc;
using SH.Game.UserData;
using SH.Net;
using SH.Net.Pkt;
using SH.Ob;
using SH.Title;
using SH.Util.UniRx;
using UniRx;
using Zenject;

namespace SH.Game.UserManager
{
    public sealed class UserDataSaveServerManager : IInitializable, IDisposable
    {
        private static readonly TimeSpan Interval = TimeSpan.FromMinutes(5);
        public static UserDataSaveServerManager Instance { get; private set; }
        private readonly UserData.UserData _userData;
        private readonly SceneDisposable _disposable;
        private bool _async = true;
        private readonly Subject<Unit> _subjectRequest;

        public UserDataSaveServerManager(UserData.UserData userData, SceneDisposable disposable)
        {
            _userData = userData;
            _disposable = disposable;
            _subjectRequest = new Subject<Unit>();
            Instance = this;
        }
        
        public void Dispose()
        {
            Instance = null;
        }
        
        public void Initialize()
        {
            return;
            if (UserIdManager.Instance.Id != 0)
            {
                var observableSave = ObservableUserDataChanged().Share();

                observableSave.ThrottleFirst(Interval)
                    .Merge(observableSave.Throttle(Interval), _subjectRequest)
                    .BatchFrame()
                    .StartWith()
                    .Where(_ => _async)
                    .Subscribe(_ => SendToServer())
                    .AddTo(_disposable);
            }
        }

        private IObservable<Unit> ObservableUserDataChanged()
        {
            var observableUserData = Observable.Merge(
                            _userData.Energy.AsUnitObservable(),
                            _userData.Exp.AsUnitObservable(),
                            _userData.Gold.AsUnitObservable(),
                            _userData.Jewel.AsUnitObservable(),
                            _userData.Level.AsUnitObservable()
            );

            var messageBroker = MessageBroker.Default;
            var observableItem = Observable.Merge(
                messageBroker.Receive<ItemMove>().AsUnitObservable(),
                messageBroker.Receive<ItemRemove>().AsUnitObservable()
            );

            return observableUserData.Merge(observableItem)
                .BatchFrame();
        }

        public void SetState(bool async)
        {
            _async = async;
        }

        public void RequestAsync()
        {
            // _subjectRequest.OnNext(Unit.Default);
        }

        private void SendToServer()
        {
            if (!AppCheck.IsPass.Value)
                return;

            if (TestAccount.TestMode)
                return;
            
            NetManager.Post(new ReqSave(_userData))
                .WhereSuccess()
                .Subscribe(_ => Debug.Log("User Data Save Server"));
        }
    }
}