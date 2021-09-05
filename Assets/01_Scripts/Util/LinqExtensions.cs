/*
* Copyright (c) CookApps.
* 이진호(jhlee8@cookapps.com)
*/

using System;
using System.Collections.Generic;
using System.Linq;

namespace SH.Util
{
    public static class LinqExtensions
    {
        /// <summary>
        /// 램덤으로 가져오자
        /// </summary>
        /// <param name="source"></param>
        /// <typeparam name="TSource"></typeparam>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static TSource Random<TSource>(this IEnumerable<TSource> source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof (source));

            if (source is IList<TSource> sourceList)
            {
                if (sourceList.Count > 0)
                    return sourceList[UnityEngine.Random.Range(0, sourceList.Count)];
            }

            if (source is TSource[] array)
            {
                if (array.Length > 0)
                    return array[UnityEngine.Random.Range(0, array.Length)];
            }
            else
            {
                var count = source.Count();
                if (count > 0)
                    return source.ElementAt(UnityEngine.Random.Range(0, count));

            }
            return default;
        }   
        
        public static void Shuffle<T>(this IList<T> list)
        {
            for (int index = 0; index < list.Count; ++index)
            {
                var random1 = UnityEngine.Random.Range(0, list.Count);
                var random2 = UnityEngine.Random.Range(0, list.Count);
 
                var tmp = list[random1];
                list[random1] = list[random2];
                list[random2] = tmp;
            }
        }

        public static IEnumerable<TResult> SelectWhereNotNull<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TResult> selector)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            
            if (selector == null)
            {
                throw new ArgumentNullException(nameof(selector));
            }

            return source.Select(selector).Where(value => value != null);
        }
    }
}