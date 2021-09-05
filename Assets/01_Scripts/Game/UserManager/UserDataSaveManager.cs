/*
* Copyright (c) CookApps.
* 이진호(jhlee8@cookapps.com)
*/

using System;
using Firebase.Crashlytics;
using JetBrains.Annotations;
using Newtonsoft.Json;
using OutGameCore;
using SH.Game.Misc;
using SH.Game.UserData;
using SH.Ob;
using SH.Util;
using SH.Util.Json;
using SH.Util.UniRx;
using UniRx;
using Zenject;

namespace SH.Game.UserManager
{
    public sealed class UserDataSaveManager : IInitializable
    {
        private readonly UserData.UserData _userData;
        private readonly SceneDisposable _sceneDisposable;
        private const string Key = "UserData";

        public bool EnableAutoSave { get; set; } = true;
        public static UserDataSaveManager Instance { get; private set; }

        public static JsonSerializerSettings JsonConvertSettings => JsonUtil.JsonConvertSettings;

        public UserDataSaveManager(UserData.UserData userData, SceneDisposable sceneDisposable)
        {
            Instance = this;
            _userData = userData;
            _sceneDisposable = sceneDisposable;
        }
        
        public void Initialize()
        {
            var observableApplicationPause = Observable.EveryApplicationPause().WhereTrue().AsUnitObservable();
            var observableApplicationQuit = Observable.OnceApplicationQuit();
                
            observableApplicationPause.Merge(observableApplicationQuit)
                .Where(_ => EnableAutoSave)
                .Subscribe(_ => Save()).AddTo(_sceneDisposable);

            // 10분마다 저장
            Observable.Interval(TimeSpan.FromMinutes(10))
                .Where(_ => EnableAutoSave)
                .Subscribe(_ => Save()).AddTo(_sceneDisposable);
        }

        public void Save()
        {
            Save(_userData);
        }

        public static void Save([NotNull]UserData.UserData userData)
        {
            if (!AppCheck.IsPass.Value)
                return;
            
            var json = userData.ToJson();
            Save(json);
        }

        public static void Save(string json)
        {
            if(SaveUtil.Save(Key, json))
            {
                Debug.Log("user data saved");
            }
        }

        public static int GetSaveUserDataVersion()
        {
            if (!ES3.KeyExists(Key))
                return 0;
            
            var json = ES3.Load<string>(Key);
            try
            {
                var version = JsonUtil.DeserializeObject<DataVersionJson>(json);
                return version.DataVersion;
            }
            catch (Exception e)
            {
                GameUtils.LogException(e);
                Debug.LogError($"UserData(DataVersionJson) Json Load Error : {json}");
                return 0;
            }
        }
            

        [NotNull]
        public static UserData.UserData Load()
        {
            var json = ES3.KeyExists(Key) ? ES3.Load<string>(Key) : "{}";
            var userData = Load(json);
            Debug.Log("user data loaded");
            return userData;
        }

        /// <summary>
        /// 초기화 유저 데이타
        /// </summary>
        /// <returns></returns>
        [NotNull]
        public static UserData.UserData InitUserData()
        {
            var json = "{}";
            var userData = Load(json);
            Debug.Log("user data init");
            return userData;
        }

        /// <summary>
        /// 저장된시간 얻기
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public static DateTime GetSaveDateTime(string json)
        {
            try
            {
                var version = JsonUtil.DeserializeObject<DataSaveTime>(json);
                return version.DateTimeSave;
            }
            catch (Exception e)
            {
                GameUtils.LogException(e);
                Debug.LogError($"UserData(DataSaveTime) Json Load Error : {json}");
                return DateTime.MinValue;
            }
        }

        /// <summary>
        /// 저장 데이타 있는가?
        /// </summary>
        /// <returns></returns>
        public static bool ExistSaveData()
        {
            var result = ES3.KeyExists(Key);
            return result;
        }

        public static UserData.UserData Load(string json)
        {
            try
            {
                var userData = JsonUtil.DeserializeObject<UserData.UserData>(json);
                return userData;
            }
            catch (Exception e)
            {
                Crashlytics.SetCustomKey("UserData Load", json);
                GameUtils.LogException(e);
                Debug.LogError($"UserData Json Load Error : {json}");
                return null;
            }
        }

        private class DataVersionJson
        {
            /// <summary>
            /// UserData의 DataVersion 이름이 같아야한다
            /// </summary>
            public int DataVersion = 0;
        }

        private class DataSaveTime
        {
            public DateTime DateTimeSave { get; set; }
        }
    }
}