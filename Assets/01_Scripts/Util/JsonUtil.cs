/*
* Copyright (c) CookApps.
* 이진호(jhlee8@cookapps.com)
*/

using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SH.Util.Json;

namespace SH.Util
{
    public class JsonUtil
    {
        public static readonly JsonSerializerSettings JsonConvertSettings = new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore,
            TypeNameHandling = TypeNameHandling.Auto,
            ContractResolver = ReactivePropertyContractResolver.Instance,
        };

        public static T DeserializeObject<T>(string value = null)
        {
            var json = string.IsNullOrEmpty(value) ? "{}" : value;
            var obj = JsonConvert.DeserializeObject<T>(json, JsonConvertSettings);
            return obj;
        }

        public static string SerializeObject(object value, Formatting formatting = Formatting.None)
        {
            var json = JsonConvert.SerializeObject(value, formatting, JsonConvertSettings);
            return json;
        }

        public static T ParseValue<T>(string json, string propertyName)
        {
            try
            {
                var jObj = JObject.Parse(json);
                var token = jObj[propertyName];
                if (token != null)
                {
                    var value = token.Value<T>();
                    return value;
                }
            }
            catch (Exception e)
            {
                return default;
            }

            return default;
        }
    }
}