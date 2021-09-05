/*
* Copyright (c) CookApps.
* 이진호(jhlee8@cookapps.com)
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using UniRx;

namespace SH.Util.Json
{
    public sealed class ReactivePropertyContractResolver : PrivateSetterContractResolver
    {
        public static readonly ReactivePropertyContractResolver Instance = new ReactivePropertyContractResolver(false);
        public static readonly ReactivePropertyContractResolver InstancePrivateSetter = new ReactivePropertyContractResolver(true);

        private ReactivePropertyContractResolver(bool privateSetter)
        : base(privateSetter)
        {
        }

        protected override JsonContract CreateContract(Type objectType)
        {
            JsonContract contract = base.CreateContract(objectType);
            

            Type genericType = null;
            if (objectType.IsGenericType)
            {
                genericType = objectType;
            }
            else
            {
                var baseType = objectType.BaseType;
                if (baseType != null && baseType.IsGenericType)
                {
                    genericType = baseType;
                }
            }

            if (genericType?.GetGenericTypeDefinition() == typeof(ReactiveProperty<>))
            {
                contract.Converter = ReactivePropertyConverter.Instance;
            }
            
            if (objectType == typeof(ObscuredIntReactiveProperty))
            {
                contract.Converter = ReactivePropertyConverter.Instance;
            }
            
            if (objectType == typeof(ObscuredFloatReactiveProperty))
            {
                contract.Converter = ReactivePropertyConverter.Instance;
            }
            return contract;
        }
    }
}