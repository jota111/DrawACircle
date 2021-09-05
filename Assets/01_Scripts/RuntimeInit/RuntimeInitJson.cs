/*
* Copyright (c) CookApps.
* 이진호(jhlee8@cookapps.com)
*/

using Newtonsoft.Json;
using SH.Util.Json;
using UnityEngine;

namespace SH.RuntimeInit
{
    public static class RuntimeInitJson
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void RuntimeInit()
        {
            JsonConvert.DefaultSettings = () => new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                ContractResolver = ReactivePropertyContractResolver.Instance
            };
        }
    }
}