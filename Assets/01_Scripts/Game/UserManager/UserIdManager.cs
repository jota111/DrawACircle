/*
* Copyright (c) CookApps.
* 이진호(jhlee8@cookapps.com)
*/

using System;
using Firebase.Analytics;
using Firebase.Crashlytics;
using SH.Util;
using UniRx;
using UnityEngine;

namespace SH.Game.UserManager
{
    public sealed class UserIdManager
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ReloadDomain()
        {
            _instance = null;
        }

        private static UserIdManager _instance;

        public static UserIdManager Instance
        {
            get
            {
                _instance ??= new UserIdManager();
                return _instance;
            }
        }


        private const string Key = "UserId";

        public long Id { get; private set; }
        public bool EmptyId => Id == 0;

        private UserIdManager()
        {
            Id = ES3.Load(Key, 0L);
            if (Id != 0)
            {
                Crashlytics.SetUserId(Id.ToString());
                FirebaseAnalytics.SetUserId(Id.ToString());
            }
        }

        public void SetId(long id)
        {
            Id = id;
            SaveUtil.Save(Key, id);
            UserIdUpdate.Publish();
            if (Id != 0)
            {
                Crashlytics.SetUserId(Id.ToString());
                FirebaseAnalytics.SetUserId(Id.ToString());
            }
        }

        public readonly struct UserIdUpdate
        {
            public static void Publish() => MessageBroker.Default.Publish(new UserIdUpdate());

            public static IObservable<Unit> Receive()
                => MessageBroker.Default.Receive<UserIdUpdate>()
                    .AsUnitObservable().BatchFrame();
        }
    }
}