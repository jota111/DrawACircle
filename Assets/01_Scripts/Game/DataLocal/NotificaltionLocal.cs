using System;
using System.Collections;
using System.Collections.Generic;
using GameDataEditor;
using OutGameCore;
using SH.Constant;
using SH.Data;
using UnityEngine;
using UniRx;

namespace SH.Game.DataLocal
{
    public class NotificaltionLocal
    {
        public ReactiveDictionary<int, NotificationItem> notifications { get; set; } =
            new ReactiveDictionary<int, NotificationItem>();

        public void AddNotification(LocalPushType type, DateTime time)
        {
            var newNoti = new NotificationItem(type, time);
            if (notifications.ContainsKey((int)type) == false)
                notifications.Add((int)type, newNoti);
            else notifications[(int)type] = newNoti;
        }
    }

    public class NotificationItem
    {
        public string titleText; // 타이틀
        public string viewText; // 내용
        public long showTime; // 로컬푸쉬 발동시간 (틱단위)

        public NotificationItem()
        {
            
        }

        // public long effectTime_Start; // 아이템 지급가능 시간 / 시작 
        // public long effectTime_End; // 아이템 지급가능 시간 / 끝
        public NotificationItem(LocalPushType type, DateTime time)
        {
            titleText = GameUtils.I2Format("App_Name");
            switch (type)
            {
                case LocalPushType.Chest:
                    viewText = GameUtils.I2Format("Notification_Chest");
                    break;
                case LocalPushType.Energy:
                    viewText = GameUtils.I2Format("Notification_Energy");
                    break;
                case LocalPushType.DailyDeals:
                    viewText = GameUtils.I2Format("Notification_DailyDeals");
                    break;
            }
            showTime = time.Ticks;
        }
    }

    public enum LocalPushType
    {
        Chest = 100,    //상자
        Energy = 200,   //에너지
        DailyDeals = 300,   //무료 저금통
    }
}