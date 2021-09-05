/*
* Copyright (c) CookApps.
* 이진호(jhlee8@cookapps.com)
*/

using System;
using System.Collections.Generic;
using BestHTTP;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;

namespace SH.CrossPromotion
{
    public class CrossPromotionManager
    {
        public static CrossPromotionManager Instance => _instance ??= new CrossPromotionManager(); 
        private static CrossPromotionManager _instance;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ReloadDomain()
        {
            _instance = null;
        }
        
        private const string ApiUrl = "https://purplecow.cookappsgames.com/common/api.php";
        public IReadOnlyReactiveProperty<CrossPromotionData> Data => _data;
        private readonly ReactiveProperty<CrossPromotionData> _data = new ReactiveProperty<CrossPromotionData>();

        public void Init(string appKey, string cmd, string platform)
        {
            var param = new Dictionary<string, string>
            {
                { "device", platform },
                { "app_key", appKey },
                { "cmd", cmd }
            };
            Request(param).Forget();
        }

        private async UniTaskVoid Request(Dictionary<string, string> param)
        {
            try
            {
                var req = new HTTPRequest(new Uri(ApiUrl), HTTPMethods.Post);
                req.DisableCache = true;
                foreach (var pair in param)
                {
                    req.AddField(pair.Key, pair.Value);
                }

                var json = await req.GetAsStringAsync();
                OnSuccess(json).Forget();
            }
            catch (Exception e)
            {
                Debug.LogWarning($"[CrossPromotion] Error : {e.Message}");
            }
        }

        private async UniTaskVoid OnSuccess(string json)
        {
            try
            {
                var data = JsonUtility.FromJson<CrossPromotionResponse>(json);
                if (data.success)
                {
                    var (icon, banner) = await UniTask.WhenAll(DownloadImage(data.data.icon_image), DownloadImage(data.data.banner_image));
                    if (icon != null && banner != null)
                    {
                        _data.Value = new CrossPromotionData
                        {
                            Data = data.data,
                            Icon = icon,
                            Banner = banner,
                        };   
                    }
                }
                else
                {
                    Debug.LogWarning($"[CrossPromotion] Error : {json}");
                }
            }
            catch (Exception e)
            {
                Debug.LogWarning($"[CrossPromotion] Error : {json}");
            }
        }

        private async UniTask<Texture2D> DownloadImage(string url)
        {
            try
            {
                var req = new HTTPRequest(new Uri(url), HTTPMethods.Get);
                var texture2D = await req.GetAsTexture2DAsync();
                return texture2D;
            }
            catch (Exception e)
            {
                Debug.LogWarning($"[CrossPromotion] Error Download Image : {url}");
                return null;
            }
        }
    }
}