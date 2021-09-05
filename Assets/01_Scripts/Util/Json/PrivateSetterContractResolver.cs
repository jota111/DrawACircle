/*
* Copyright (c) CookApps.
* 이진호(jhlee8@cookapps.com)
*/

using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace SH.Util.Json
{
    public class PrivateSetterContractResolver : DefaultContractResolver
    {
        private readonly bool _privateSetter;

        protected PrivateSetterContractResolver(bool privateSetter)
        {
            _privateSetter = privateSetter;
        }

        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            if (!_privateSetter)
                return base.CreateProperty(member, memberSerialization);
            
            JsonProperty jsonProperty = base.CreateProperty(member, memberSerialization);
            PropertyInfo propertyInfo;
            if ((propertyInfo = (member as PropertyInfo)) == null || IsModifiable(jsonProperty))
            {
                return jsonProperty;
            }

            // JsonIgnoreAttribute 기본
            if (Attribute.IsDefined(member, typeof(JsonIgnoreAttribute)))
                return jsonProperty;
            
            jsonProperty.Readable = propertyInfo.CanRead;
            jsonProperty.Writable = propertyInfo.CanWrite;
            if (IsModifiable(jsonProperty))
            {
                return jsonProperty;
            }
            FieldInfo field = propertyInfo.DeclaringType.GetField(string.Format("<{0}>k__BackingField", propertyInfo.Name), BindingFlags.Instance | BindingFlags.NonPublic);
            if (field != null && field.IsDefined(typeof(CompilerGeneratedAttribute), true))
            {
                return FromBackingField(jsonProperty, propertyInfo, field);
            }
            return jsonProperty;
            
            //---------------------------------------------------------------------------------------------------------------
            static JsonProperty FromBackingField(JsonProperty originalProperty, PropertyInfo property, FieldInfo field)
            {
                return new JsonProperty
                {
                    DeclaringType = property.DeclaringType,
                    PropertyType = property.PropertyType,
                    PropertyName = originalProperty.PropertyName,
                    ValueProvider = new ReflectionValueProvider(field),
                    Readable = true,
                    Writable = true
                };
            }
            
            static bool IsModifiable(JsonProperty originalProperty)
            {
                if (originalProperty == null)
                {
                    throw new ArgumentNullException("jsonProperty");
                }
                return originalProperty.Readable && originalProperty.Writable;
            }
        }
    }
}