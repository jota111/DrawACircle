using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using OutGameCore;
using MoreLinq;
using SH.Constant;
using SH.Game.DataLocal;
using SH.Util;
using SH.Util.UniRx;
using UniRx;
using UnityEngine;


namespace SH.Game.Manager
{
    public sealed class LocalNotificationManager
    {
        private readonly UserData.UserData _userData;

        public SettingsLocal settingsLocal
        {
            get => _userData.Settings;
            set => _userData.Settings = value;
        }

        public NotificaltionLocal notificaltionLocal
        {
            get => _userData.Notification;
            set => _userData.Notification = value;
        }

        public static LocalNotificationManager Instance { get; private set; }
        private EnergyManager _energyManager;
        private ShopManager _shopManager;

        #region 초기화

        public LocalNotificationManager(UserData.UserData userData, SceneDisposable disposable, EnergyManager energyManager, ShopManager shopManager)
        {
            Instance = this;
            _userData = userData;
            _energyManager = energyManager;
            _shopManager = shopManager;
            GleyNotifications.Initialize();
            var check = GleyNotifications.AppWasOpenFromNotification();
            GameUtils.Log($"LocalNotificationManager 푸쉬 체크 : {check}");

            var observableApplicationPause = Observable.EveryApplicationPause().WhereTrue().AsUnitObservable();
            var observableApplicationQuit = Observable.OnceApplicationQuit();

            observableApplicationPause.Merge(observableApplicationQuit)
                .Subscribe(_ => OnApplicationPuase(true)).AddTo(disposable);

            Observable.EveryApplicationPause().Where(x => x == false).Subscribe(OnApplicationPuase).AddTo(disposable);
        }

        private void OnApplicationPuase(bool pause)
        {
            GameUtils.Log($"LocalNotificationManager OnApplicationPuase {pause}");
            if (pause)
            {
                GleyNotifications.Initialize();
                SetNotifications();
            }
            else
            {
                GleyNotifications.Initialize();
            }
        }

        #endregion

        #region 로컬푸쉬 세팅

        private void SetNotifications()
        {
            RemovePast();
            if (!settingsLocal.OptionNotification.Value) return;

            // 상자
            if (settingsLocal.OptionNotification_Chest.Value && notificaltionLocal.notifications.ContainsKey((int) LocalPushType.Chest))
            {
                var data = notificaltionLocal.notifications[(int) LocalPushType.Chest];
                AddNotification(data);
            }

            //DailyDeals
            if (settingsLocal.OptionNotification_DailyDeals.Value)
            {
                var pushTime = GameUtils.GetFlooredGameTime_Day().AddHours(8);
                if (TimeUtil.Now >= pushTime)
                    pushTime = GameUtils.GetFlooredGameTime_Day().AddHours(32);
                if (_shopManager.GetPurchasableCount("DailyDeals_0") > 0)
                {
                    var noti = new NotificationItem(LocalPushType.DailyDeals, pushTime);
                    AddNotification(noti);
                }
                for (int i = 1; i < 11; i++)
                {
                    var addTime = pushTime.AddHours(24 * i);
                    var notiAdd = new NotificationItem(LocalPushType.DailyDeals, addTime);
                    AddNotification(notiAdd);
                }
            }

            //Energy
            if (settingsLocal.OptionNotification_Energy.Value)
            {
                var pushTime = _energyManager.FullChargeTime();
                if (TimeUtil.Now < pushTime)
                {
                    var noti = new NotificationItem(LocalPushType.Energy, pushTime);
                    AddNotification(noti);
                }
            }
        }

        public void RemovePast()
        {
            List<LocalPushType> pastType = (from noti in notificaltionLocal.notifications
                where noti.Value.showTime < TimeUtil.Now.Ticks
                select (LocalPushType) noti.Key).ToList();
            pastType.ForEach(x => notificaltionLocal.notifications.Remove((int) x));
        }

        private void AddNotification(NotificationItem notiItem)
        {
            var timeSpan = new DateTime(notiItem.showTime).Subtract(TimeUtil.Now);
            GameUtils.Log($"LocalNotificationManager 알림추가 타이틀:{notiItem.titleText} 내용:{notiItem.viewText} 초:{timeSpan.TotalSeconds}");
            if (timeSpan.TotalSeconds > 0)
                GleyNotifications.SendNotification(notiItem.titleText, notiItem.viewText, timeSpan, null, null,
                    "Custom Data");
        }

        #endregion
    }
}