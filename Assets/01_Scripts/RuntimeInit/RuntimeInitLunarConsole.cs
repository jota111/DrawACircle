/*
* Copyright (c) CookApps.
* 이진호(jhlee8@cookapps.com)
*/

using UnityEngine;

namespace SH.RuntimeInit
{
    public static class RuntimeInitLunarConsole
    {
#if __DEV || UNITY_EDITOR
        [RuntimeInitializeOnLoadMethod]
        private static void RuntimeInit()
        {
            Object.Instantiate(Resources.Load("LunarConsole"));
        }
#endif
    }
}