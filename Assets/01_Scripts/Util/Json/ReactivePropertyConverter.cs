/*
* Copyright (c) CookApps.
* 이진호(jhlee8@cookapps.com)
*/

using System;
using Newtonsoft.Json;

namespace SH.Util.Json
{
    public sealed class ReactivePropertyConverter : JsonConverter
    {
        public static readonly ReactivePropertyConverter Instance = new ReactivePropertyConverter();
        
        private const string Value = "Value";       
        
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var propValue = value.GetType().GetProperty(Value);
            if (propValue != null)
            {
                var v = propValue.GetValue(value, null);
                if (propValue.PropertyType.IsClass)
                {
                    serializer.Serialize(writer, v);
                }
                else
                {                
                    writer.WriteValue(v);
                }
            }
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var property = objectType.GetProperty(Value);
            if (property != null)
            {
                var argType = property.PropertyType;
                var value = serializer.Deserialize(reader, argType);
                var inst = Activator.CreateInstance(objectType, value);
                return inst;
            }


            throw new ArgumentNullException("property", "not found json property");
        }

        public override bool CanConvert(Type objectType)
        {
            return false;
        }
    }
}