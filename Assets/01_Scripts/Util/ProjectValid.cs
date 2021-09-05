/*
* Copyright (c) CookApps.
* 이진호(jhlee8@cookapps.com)
*/

using System;
using System.Collections.Generic;
using System.Linq;
using Object = UnityEngine.Object;

namespace SH.Util
{
    /// <summary>
    /// 프로젝트 검증할때 사용하자!
    /// </summary>
    public static class ProjectValid
    {
        public static bool ValidFullEnum<TKey, TValue>(Dictionary<TKey, TValue> dict) where TValue : Object where TKey : Enum
        {
            var hasNull = dict.Values.Any(go => go == null);
            if (hasNull)
            {
                return false;
            }

            var except = EnumHelper.AllValues<TKey>().Except(dict.Keys).Any();
            return !except;
        }
        
        public static bool Valid<TKey, TValue>(Dictionary<TKey, TValue> dict) where TValue : Object
        {
            var hasNull = dict.Values.Any(go => go == null);
            return !hasNull;
        }

        public static bool Valid<T>(IEnumerable<T> enumerable) where T : Object
        {
            var hasNull = enumerable.Any(go => go == null);
            return !hasNull;
        }
        
        public static bool ValidCount<T>(IEnumerable<T> enumerable, int count) where T : Object
        {
            if (enumerable.Count() != count)
                return false;
            
            var hasNull = enumerable.Any(go => go == null);
            return !hasNull;
        }

        public static bool Valid(Object obj)
        {
            return obj != null;
        }
    }
}