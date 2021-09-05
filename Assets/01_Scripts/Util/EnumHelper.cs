/*
* Copyright (c) CookApps.
* 이진호(jhlee8@cookapps.com)
*/

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SH.Util
{
    public static class EnumHelper
    {
        /// <summary>
        ///  enum 케싱용
        /// </summary>
        /// <typeparam name="TEnum"></typeparam>
        private static class EnumCache<TEnum>
        {
            public static TEnum[] Values = null;

            public static TEnum[] GetAll()
            {
                return Enum.GetValues(typeof(TEnum)).Cast<TEnum>().ToArray();
            }
        }

        private static class EnumStringCache<TEnum>
        {
            private static Dictionary<TEnum, string> Dict = null;

            public static string GetString(TEnum value)
            {
                Dict ??= new Dictionary<TEnum, string>();
                if (!Dict.TryGetValue(value, out var result))
                {
                    result = value.ToString();
                    Dict.Add(value, result);
                }

                return result;
            }
        }
        
        /// <summary>
        /// 모든 Enum 얻기
        /// </summary>
        /// <typeparam name="TEnum"></typeparam>
        /// <returns></returns>
        public static TEnum[] AllValues<TEnum>() where TEnum : Enum
        { 
            EnumCache<TEnum>.Values ??= EnumCache<TEnum>.GetAll();
            return EnumCache<TEnum>.Values;
        }

        /// <summary>
        /// Enum String
        /// </summary>
        /// <param name="value"></param>
        /// <typeparam name="TEnum"></typeparam>
        /// <returns></returns>
        public static string GetEnumString<TEnum>(TEnum value) where TEnum : Enum
        {
            return EnumStringCache<TEnum>.GetString(value);
        }

        /// <summary>
        /// Enum String
        /// </summary>
        /// <param name="value"></param>
        /// <typeparam name="TEnum"></typeparam>
        /// <returns></returns>
        public static string ToEnumString<TEnum>(this TEnum value) where TEnum : Enum
        {
            return GetEnumString(value);
        }
    }
}