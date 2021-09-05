/*
* Copyright (c) CookApps.
* 이진호(jhlee8@cookapps.com)
*/

using System;
using Newtonsoft.Json;
using SH.Constant;
using SH.Game.UserData;
using SH.Game.UserManager;
using SH.Platform;
using SH.Title;
using UnityEngine;

// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Local
// ReSharper disable UnassignedGetOnlyAutoProperty

namespace SH.Net
{
    public abstract class IRequest
    {
        [JsonIgnore]
        public string url { get; }
        public long ca_uid { get; private set; }
        public string device_id { get; private set; }
        public int version { get; private set; }
        public string device { get; private set; }
        public string fb_uid { get; protected set; }
        public string gg_uid { get; protected set; }
        public string apple_uid { get; protected set; }

        protected IRequest(string url, PlatformLoginId platformLoginId = null)
        {
            this.url = url;
            version = StaticSet.VersionNumber;
            ca_uid = UserIdManager.Instance.Id;
            device_id = SystemInfo.deviceUniqueIdentifier;
            device = NetDefine.PLATFORM;

            if (platformLoginId != null)
            {
                fb_uid = platformLoginId.Facebook;
                gg_uid = platformLoginId.Google;
                apple_uid = platformLoginId.Apple;
            }
            else
            {
                fb_uid = PlatformLoginManager.Instance.FacebookId;
                gg_uid = PlatformLoginManager.Instance.GoogleId;
                apple_uid = PlatformLoginManager.Instance.AppleId;   
            }
        }
    }

    public class Recv
    {
        public bool result { get; private set; }
    }

    public class DataError
    {
        public int code { get; private set; }
        public string message { get; private set; }
    }

    public class RecvErrorJson
    {
        public bool result { get; private set; }

        public static string CreateJson()
        {
            var inst = new RecvErrorJson
            {
                result = false
            };
            var json = JsonConvert.SerializeObject(inst);
            return json;
        }
    }
}

namespace UniRx
{
    public static partial class ObservableForNet
    {
        public static IObservable<T> WhereSuccess<T>(this IObservable<T> source) where T : SH.Net.Recv
        {
            return source.Where(recv => recv.result);
        }

        public static IObservable<T> WhereFailed<T>(this IObservable<T> source) where T : SH.Net.Recv
        {
            return source.Where(recv => !recv.result);
        }
    }
}