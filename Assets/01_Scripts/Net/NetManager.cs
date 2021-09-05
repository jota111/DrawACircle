/*
* Copyright (c) CookApps.
* 이진호(jhlee8@cookapps.com)
*/

#define ENABLE_GZIP

using System;
using System.Text;
using BestHTTP;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using SH.Util;
using SH.Util.Json;
using UniRx;
using UnityEngine;
using UnityEngine.Networking;

#if ENABLE_GZIP
using SH.Util;
#endif

namespace SH.Net
{
    public static class NetManager
    {
        private static readonly JsonSerializerSettings JsonSettings = new JsonSerializerSettings()
        {
            NullValueHandling = NullValueHandling.Ignore,
            TypeNameHandling = TypeNameHandling.Auto,
            ContractResolver = ReactivePropertyContractResolver.InstancePrivateSetter,
        };

        public static IObservable<T> Post<T>(IRequest request) where T : Recv
        {
            return AsyncPost<T>(request).ToObservable()
                .PublishLast()
                .RefCount();
        }

        public static IObservable<Recv> Post(IRequest request)
        {
            return Post<Recv>(request);
        }
        
        public static async UniTask<T> AsyncPost<T>(IRequest request) where T : Recv
        {
            var json = JsonConvert.SerializeObject(request, JsonSettings);
            var url = request.url;
            return await AsyncPost<T>(url, json);
        }

        public static async UniTask<Recv> AsyncPost(IRequest request)
        {
            return await AsyncPost<Recv>(request);
        }

        public static async UniTask<bool> AsyncPostSuccess(IRequest request)
        {
            var recv = await AsyncPost(request);
            return recv.result;
        }

        public static void PostForget(IRequest request)
        {
            AsyncPost<Recv>(request).Forget();
        }

        public static void PostForget(string url, string json)
        {
            AsyncPost<Recv>(url, json).Forget();
        }
        private static async UniTask<T> AsyncPost<T>(string url, string json) where T : Recv
        {
            return await AsyncPostBestHTTP<T>(url, json);
        }

        private static async UniTask<T> AsyncPostBestHTTP<T>(string url, string json) where T : Recv
        {
            try
            {
                Debug.Log($"[Network|Send|{url}]\n{json}");
                var time = Time.realtimeSinceStartup;
                
                var fullURL = NetDefine.NET_SERVER_ADDR + url;
                var req = new HTTPRequest(new Uri(fullURL), HTTPMethods.Post);
                req.SetHeader("Content-Type", "application/json; charset=UTF-8");
                
#if ENABLE_GZIP
                var data = Encoding.UTF8.GetBytes(json);
                var gzipData = CompressUtil.CompressGZip(data);
                if (data.Length > gzipData.Length)
                {
                    req.SetHeader("Content-Encoding", "gzip");
                    req.RawData = gzipData;
                }
                else
                {
                    req.RawData = data;
                }
#else           
                req.RawData = Encoding.UTF8.GetBytes(json);
#endif
                req.DisableCache = true;
                req.Timeout = TimeSpan.FromSeconds(NetDefine.NET_TIMEOUT);
                var text = await req.GetAsStringAsync();
                
                var ms = Mathf.FloorToInt((Time.realtimeSinceStartup - time) * 1000);
                Debug.Log($"[Network|Recv|{url}|{ms}ms]\n{text}");
                var obj = JsonConvert.DeserializeObject<T>(text, JsonSettings);
                return obj;
            }
            catch (Exception e)
            {
                Debug.LogError($"[Network|Recv|{url}] Error \n {e.Message}");
                return CreateError<T>();
            }
        }

        private static T CreateError<T>() where T : Recv
        {
            var json = RecvErrorJson.CreateJson();
            return JsonConvert.DeserializeObject<T>(json);
        }

        public static async UniTask<T> AsyncGet<T>(IRequest request) where T : Recv
        {
            var url = request.url;
            return await AsyncGet<T>(url);
        }
        
        private static async UniTask<T> AsyncGet<T>(string url) where T : Recv
        {
            try
            {
                var fullURL = NetDefine.NET_SERVER_ADDR + url;
                using var req = UnityWebRequest.Get(fullURL);
                // req.method = UnityWebRequest.kHttpVerbGET;
                req.useHttpContinue = false;
                req.timeout = NetDefine.NET_TIMEOUT;
                // req.SetRequestHeader("Content-Type", "application/json");
                
                Debug.Log($"[Network|Send|{url}]");
                var time = Time.realtimeSinceStartup;
                await req.SendWebRequest();
                var ms = Mathf.FloorToInt((Time.realtimeSinceStartup - time) * 1000);
                if (req.result == UnityWebRequest.Result.Success)
                {
                    var text = req.downloadHandler.text;
                    Debug.Log($"[Network|Recv|{url}|{ms}ms]\n{text}");
                    var obj = JsonConvert.DeserializeObject<T>(text, JsonSettings);
                    return obj;
                }
                else
                {
                    Debug.LogError($"[Network|Recv|{url}|{ms}ms] Error \n {req.error}");
                    return CreateError();
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"[Network|Recv|{url}] Error \n {e.Message}");
                return CreateError();
            }
            
            //-------------------------------------------------------------------
            static T CreateError()
            {
                var json = RecvErrorJson.CreateJson();
                return JsonConvert.DeserializeObject<T>(json);
            }
        }
    }
}