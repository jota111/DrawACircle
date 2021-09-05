/*
* Copyright (c) CookApps.
* 이진호(jhlee8@cookapps.com)
*/

using System;
using Beebyte.Obfuscator;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json.Linq;
using UnityEngine.Networking;

namespace SH.Ob
{
    [ReplaceLiteralsWithName]
    public static class CountryCode
    {
        private static string Code { get; set; } = string.Empty;
        public static string GetCode => Code;

        [ObfuscateLiterals]
        public static async UniTask<string> AsyncCountryCode()
        {
            try
            {
                if (!string.IsNullOrEmpty(Code))
                    return Code;
                
                var req = UnityWebRequest.Get("https://pro.ip-api.com/json/?fields=49155&key=KdJCcyh60RkJvhY");
                req.timeout = 7;
                await req.SendWebRequest();
                if (req.result == UnityWebRequest.Result.Success)
                {
                    var text = req.downloadHandler.text;
                    var json = JObject.Parse(text);
                    var c = json["countryCode"];
                    var cd = c?.Value<string>();
                    Code = cd ?? string.Empty;
                }
            }
            catch (Exception)
            {
                return Code;
            }

            return Code;
        }
    }
}