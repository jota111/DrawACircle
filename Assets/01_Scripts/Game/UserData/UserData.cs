/*
* Copyright (c) CookApps.
* 이진호(jhlee8@cookapps.com)
*/

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Cysharp.Threading.Tasks;
using I2.Loc;
using Newtonsoft.Json;
using OutGameCore;
using SH.Constant;
using SH.Data;
using SH.Game.DataLocal;
using SH.Util;
using UniRx;
using UnityEngine;

namespace SH.Game.UserData
{
    public class UserData
    {
        /// <summary>
        /// 호환 가능한 데이타 버전
        /// </summary>
        public const int CompatibleDataVersion = 3;

        public static UserData Instance { get; private set; }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ReloadDomain()
        {
            Instance = null;
        }

        public int DataVersion { get; set; }
        public ObscuredIntReactiveProperty Gold { get; set; }
        public ObscuredIntReactiveProperty Jewel { get; set; }
        public ObscuredIntReactiveProperty Exp { get; set; }
        public ObscuredIntReactiveProperty Level { get; set; }
        public ObscuredIntReactiveProperty Energy { get; set; }
        public ReactiveProperty<DateTime> LastEnergyChargeTime { get; set; }
        public ShopLocal Shop { get; set; }
        public TutorialLocal Tutorial { get; set; }
        public DateTime DateTimeSave { get; set; }
        public SettingsLocal Settings { get; set; }
        public NotificaltionLocal Notification { get; set; }
        public ObscuredIntReactiveProperty LastStep { get; set; }

        public AppEventDataLocal AppEventData { get; set; }

        public ObscuredIntReactiveProperty InduceRating { get; set; }
        public ReactiveProperty<DateTime> EnergyBoosterEndTime { get; set; }
        public ProfileDataLocal ProfileData { get; set; }
        public ReactiveDictionary<string, DateTime> FirstViewTime { get; set; }

        /// <summary>
        /// 게임 버전
        /// </summary>
        public int GameVersion = 0;

        /// <summary>
        /// 최초 설치 버전
        /// </summary>
        public int FirstInstallVersion { get; set; }

        public int IsFirstPlay { get; set; }

        /// <summary>
        /// 최초 접속 일자
        /// </summary>
        /// <returns></returns>
        public DateTime FirstAccessDate { get; set; } = DateTime.MinValue;
        

        [OnDeserialized]
        internal void OnDeserialized(StreamingContext context)
        {
            if (GameVersion == 0)
            {
                GameVersion = StaticSet.VersionNumber;
            }

            if (DataVersion == 0)
            {
                DataVersion = CompatibleDataVersion;
            }

            if (DateTimeSave == DateTime.MinValue)
                DateTimeSave = TimeUtil.Now;

            if (FirstInstallVersion == 0)
                FirstInstallVersion = StaticSet.VersionNumber;
            if (FirstAccessDate == DateTime.MinValue)
                FirstAccessDate = TimeUtil.Now;

            Gold ??= new ObscuredIntReactiveProperty();
            Jewel ??= new ObscuredIntReactiveProperty(100);
            Exp ??= new ObscuredIntReactiveProperty();
            Level ??= new ObscuredIntReactiveProperty(1);
            Energy ??= new ObscuredIntReactiveProperty(100);
            LastEnergyChargeTime ??= new ReactiveProperty<DateTime>(DateTime.MinValue);
            Shop ??= JsonUtil.DeserializeObject<ShopLocal>();
            Tutorial ??= JsonUtil.DeserializeObject<TutorialLocal>();
            Settings ??= new SettingsLocal(); //JsonConvert.DeserializeObject<SettingsLocal>("{}");
            Notification ??= new NotificaltionLocal();
            LastStep ??= new ObscuredIntReactiveProperty(-1);
            AppEventData ??= new AppEventDataLocal();
            InduceRating ??= new ObscuredIntReactiveProperty(-1);
            EnergyBoosterEndTime ??= new ReactiveProperty<DateTime>(DateTime.MinValue);
            ProfileData ??= new ProfileDataLocal();
            FirstViewTime ??= new ReactiveDictionary<string, DateTime>(); 
            IsFirstPlay = IsFirstPlay == 0 && FirstInstallVersion != GameVersion ? 1 : IsFirstPlay;
            
            StartRestorePurchase().Forget();

            //호환 가능한 데이타 버전 올리기
            Instance = this;
            if(!string.IsNullOrEmpty(Settings.SelectedLanguage))
                LocalizationManager.CurrentLanguage = Settings.SelectedLanguage;
        }

        [OnSerializing]
        internal void OnSerializing(StreamingContext context)
        {
            DateTimeSave = TimeUtil.Now;
            GameVersion = StaticSet.VersionNumber;
            DataVersion = CompatibleDataVersion;
        }
        
        /// <summary>
        /// 내부적인 구매복원
        /// </summary>
        internal async UniTask StartRestorePurchase()
        {
            if (Shop == null) return;
        }
    }
}