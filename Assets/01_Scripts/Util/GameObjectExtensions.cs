/*
* Copyright (c) CookApps.
* 이진호(jhlee8@cookapps.com)
*/

using JetBrains.Annotations;
using UnityEngine;

namespace SH.Util
{
    public static class GameObjectExtensions
    {
        [NotNull]
        public static T GetComponentOrAdd<T>([NotNull]this GameObject obj) where T : Component
        {
            var t = obj.GetComponent<T>();

            if (t == null)
            {
                t = obj.AddComponent<T>();
            }

            return t;
        }
    }
}