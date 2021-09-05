/*
* Copyright (c) CookApps.
* 이진호(jhlee8@cookapps.com)
*/

using System;
using Beebyte.Obfuscator;
using Firebase.Crashlytics;
using Newtonsoft.Json;
using SH.Game.UserManager;
using SH.Platform;
using SH.Util;

namespace SH.Game.UserData
{
    [ObfuscateLiterals]
    public static class UserDataExtensions
    {
        public class UserDataComparison
        {
            public int Exp { get; set; }
            public int InventoryAmount { get; set; }
            public DateTime DateTimeSave { get; set; }
        }

        public static string ToJson(this UserData target)
        {
            var json = JsonUtil.SerializeObject(target);
            return json;
        }

        /// <summary>
        /// 최신 유저 데이타 얻기
        /// </summary>
        /// <param name="localData"></param>
        /// <param name="serverData"></param>
        /// <returns></returns>
        public static (UserData userData, bool definitive/*확정적인가?*/) LatestComparison(this UserData localData, UserData serverData)
        {
            if (serverData == null) 
                return (localData, true);

            if (localData.Exp.Value != serverData.Exp.Value)
                return localData.Exp.Value > serverData.Exp.Value ? (a: localData, true) : (b: serverData, true);
            
            return localData.DateTimeSave + TimeSpan.FromMinutes(1 + 5) >= serverData.DateTimeSave ? (a: localData, true) : (b: serverData, false);
        }

        public static bool LocalIsLatest(this UserData a, string json)
        {
            try
            {
                var b = JsonConvert.DeserializeObject<UserDataComparison>(json, UserDataSaveManager.JsonConvertSettings);
                if (b == null)
                    return true;
                
                if (a.Exp.Value != b.Exp)
                    return a.Exp.Value > b.Exp;

                return a.DateTimeSave + TimeSpan.FromMinutes(1 + 5) >= b.DateTimeSave;
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                Debug.LogError($"UserDataComparison Json Load Error : {json}");
                return true;
            }
        }
    }
}