using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using MoreLinq;
using SH.Util;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace OutGameCore
{
    public static class GameStaticUtils
    {
        #region ReactiveCollection

        public static T Find<T>(this ReactiveCollection<T> value, Predicate<T> match)
        {
            for (int i = 0; i < value.Count; i++)
            {
                if (match(value[i]))
                    return value[i];
            }
            return default;
        }
        
        public static List<T> FindAll<T>(this ReactiveCollection<T> value, Predicate<T> match)
        {
            List<T> list = new List<T>();
            for (int i = 0; i < value.Count; i++)
            {
                if (match(value[i]))
                    list.Add(value[i]); 
            }
            
            return list;
        }

        public static int FindIndex<T>(this ReactiveCollection<T> value, Predicate<T> match)
        {
            for (int i = 0; i < value.Count; i++)
            {
                if (match(value[i]))
                    return i;
            }

            return -1;
        }

        public static void RemoveAll<T>(this ReactiveCollection<T> value, Predicate<T> match)
        {
            List<T> removeList = new List<T>();
            for (int i = 0; i < value.Count; i++)
            {
                if (match(value[i]))
                    removeList.Add(value[i]);
            }

            removeList.ForEach(x => value.Remove(x));
        }

        #endregion

        #region Text

        public static void SetAlpha(this Text text, float alpha)
        {
            text.color = new Color(text.color.r, text.color.g, text.color.b, alpha);
        }

        /// <summary>
        /// Text Fade
        /// </summary>
        public static async UniTask Fade(this Text text, bool onOff, float duration)
        {
            if (onOff)
            {
                text.DOKill();
                if (text.color.a < 1f && !text.gameObject.activeSelf)
                    text.SetAlpha(0);
                text.gameObject.SetActive(true);
                await text.DOFade(1f, duration);
            }
            else
            {
                text.DOKill();
                await text.DOFade(0f, duration);
                text.gameObject.SetActive(false);
            }
        }

        public static void SetAlpha(this Image image, float alpha)
        {
            image.color = new Color(image.color.r, image.color.g, image.color.b, alpha);
        }

        /// <summary>
        /// 이미지 Fade
        /// </summary>
        public static async UniTask Fade(this Image image, bool onOff, float duration)
        {
            if (onOff)
            {
                image.DOKill();
                if (image.color.a < 1f && !image.gameObject.activeSelf)
                    image.SetAlpha(0);
                image.gameObject.SetActive(true);
                await image.DOFade(1f, duration);
            }
            else
            {
                image.DOKill();
                await image.DOFade(0f, duration);
                image.gameObject.SetActive(false);
            }
        }
        
        public static async UniTask FadeForce(this Image image, bool onOff, float duration)
        {
            if (onOff)
            {
                image.DOKill();
                image.SetAlpha(0);
                image.gameObject.SetActive(true);
                await image.DOFade(1f, duration);
            }
            else
            {
                image.DOKill();
                await image.DOFade(0f, duration);
                image.gameObject.SetActive(false);
            }
        }
        
        public static async UniTask JustFade(this Image image, bool onOff, float duration)
        {
            if (onOff)
            {
                image.DOKill();
                if (image.color.a < 1f && !image.gameObject.activeSelf)
                    image.SetAlpha(0);
                await image.DOFade(1f, duration);
            }
            else
            {
                image.DOKill();
                await image.DOFade(0f, duration);
            }
        }

        public static void SetAlpha(this SpriteRenderer spriteRenderer, float alpha)
        {
            spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, alpha);
        }

        /// <summary>
        /// Text Fade
        /// </summary>
        public static async UniTask Fade(this SpriteRenderer spriteRenderer, bool onOff, float duration)
        {
            if (spriteRenderer == null) return;
            if (onOff)
            {
                spriteRenderer.DOKill();
                if (spriteRenderer.color.a < 1f && !spriteRenderer.gameObject.activeSelf)
                    spriteRenderer.SetAlpha(0);
                spriteRenderer.gameObject.SetActive(true);
                await spriteRenderer.DOFade(1f, duration);
            }
            else
            {
                spriteRenderer.DOKill();
                await spriteRenderer.DOFade(0f, duration);
                if (spriteRenderer == null) return;
                spriteRenderer.gameObject.SetActive(false);
            }
        }

        #endregion
        
        #region ReactiveProperty<int>

        /// <summary>
        /// 해당 target을 end값까지 duration동안 순차적으로 증가
        /// </summary>
        /// <param name="target"></param>
        /// <param name="end"></param>
        /// <param name="duration"></param>
        public static void Anim(this ReactiveProperty<int> target, int end, float duration)
        {
            var start = target.Value;
            var diff = (end > start ? (end - start) : (start - end));
            if (diff == 0) return;
            var check = duration / diff;
            check = Mathf.Max(check, 0.1f);
            var value = (int)Math.Ceiling(diff * check);
            var max = value == 0 ? 1 : diff / value;
            StartAnim(0).Forget();
            
            async UniTaskVoid StartAnim(int index)
            {
                if (index >= max)
                    target.Value = end;
                else
                {
                    await UniTask.Delay(TimeSpan.FromSeconds(check));
                    if (target.Value + value > end)
                        target.Value = end;
                    else
                        target.Value += value;
                    StartAnim(index + 1).Forget();
                }
            }
        }

        #endregion
        
        #region List<T>

        /// <summary>
        /// list에 추가 시도
        /// </summary>
        /// <returns></returns>
        public static bool TryAdd<T>(this List<T> list, T item)
        {
            if (!list.Contains(item))
            {
                list.Add(item);
                return true;
            }
            return false;
        }

        public static List<T> Clone<T>(this List<T> list)
        {
            List<T> newList = new List<T>();
            list.ForEach(x=>newList.Add(x));
            return newList;
        }

        #endregion
        
        #region Dictionary

        public static bool ContainsIndex<TKey, TValue>(this Dictionary<TKey, TValue> variable, int index)
        {
            var keyValuePair = variable.ElementAtOrDefault(index);
            return !keyValuePair.Equals(default(KeyValuePair<TKey, TValue>));
        }
        
        public static void TryAddInt<TKey>(this Dictionary<TKey, int> variable, TKey key, int value)
        {
            if (variable.ContainsKey(key))
                variable[key] += value;
            else
                variable.Add(key, value);
        }
        
        public static void TryAdd<TKey, TValue>(this Dictionary<TKey, TValue> variable, TKey key, TValue value)
        {
            if (variable.ContainsKey(key))
                variable[key] = value;
            else
                variable.Add(key, value);
        }
        
        public static bool Merge<TKey, TValue>(this Dictionary<TKey, TValue> variable, params Dictionary<TKey, TValue>[] target)
        {
            foreach (var dictionary in target)
            {
                foreach (var item in dictionary)
                {
                    if (!variable.ContainsKey(item.Key))
                    {
                        variable.Add(item.Key, item.Value);
                    }
                    else
                    {
                        GameUtils.Error($"Dictionary Merge 실패, {item.Key}가 이미 있음");
                        return false;
                    }
                }
            }
            return true;
        }
        
        public static void TryAdd<TKey, TValue>(this ReactiveDictionary<TKey, TValue> variable, TKey key, TValue value)
        {
            if (variable.ContainsKey(key))
                variable[key] = value;
            else
                variable.Add(key, value);
        }
        
        public static bool Merge<TKey, TValue>(this ReactiveDictionary<TKey, TValue> variable, params ReactiveDictionary<TKey, TValue>[] target)
        {
            foreach (var dictionary in target)
            {
                foreach (var item in dictionary)
                {
                    if (!variable.ContainsKey(item.Key))
                    {
                        variable.Add(item.Key, item.Value);
                    }
                    else
                    {
                        GameUtils.Error($"Dictionary Merge 실패, {item.Key}가 이미 있음");
                        return false;
                    }
                }
            }
            return true;
        }
        
        #endregion
        
        #region Enum
        
        public static TEnum ToEnumString<TEnum>(this string value) where TEnum : Enum
        {
            return (TEnum) Enum.Parse(typeof(TEnum), value);
        }
        
        #endregion
        
        #region string

        public static string ToCostString(this int n)
        {
            float calc = n / 1000000000f;
            if (calc >= 1d)
                return $"{(Mathf.Floor(calc * 10) / 10):0.#}b";

            calc = n / 1000000f;
            if (calc >= 1d)
                return $"{(Mathf.Floor(calc * 10) / 10):0.#}m";

            calc = n / 1000f;
            if (calc >= 1d)
                return $"{(Mathf.Floor(calc * 10) / 10):0.#}k";

            return $"{n:n0}";
        }

        public static bool IsAvailableKey(this string key)
        {
            var check = !string.IsNullOrEmpty(key) && !key.Equals("0");
            return check;
        }

        #endregion
        
        #region Camera

        public static bool CheckVisible(this Camera camera, Collider collider)
        {
            Plane[] planes = GeometryUtility.CalculateFrustumPlanes(camera);
            var check = GeometryUtility.TestPlanesAABB(planes, collider.bounds);
            return check;
        }
        
        public static bool CheckVisible(this Camera camera, Collider2D collider2D)
        {
            Plane[] planes = GeometryUtility.CalculateFrustumPlanes(camera);
            var check = GeometryUtility.TestPlanesAABB(planes, collider2D.bounds);
            return check;
        }

        #endregion
        
        #region ENUM

        public static TEnum RandomRange<TEnum>(int from, int to) where TEnum : Enum
        {
            TEnum value = default;
            var values = Enum.GetValues(typeof(TEnum)).Cast<TEnum>().ToList();
            values = values.FindAll(x => from <= GetConvertValue(x) && to >= GetConvertValue(x)).ToList();
            return values.Random();

            int GetConvertValue(TEnum value)
            {
                var iEnum = Convert.ChangeType(value, typeof(int));
                return (int)iEnum;
            }
        }

        #endregion
    }
}