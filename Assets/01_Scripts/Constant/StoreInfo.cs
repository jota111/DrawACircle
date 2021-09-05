/*
* Copyright (c) CookApps.
* 이진호(jhlee8@cookapps.com)
*/

using UnityEngine;

namespace SH.Constant
{
    public static class StoreInfo
    {
#if UNITY_ANDROID
    #if UNITY_AWS
        private static readonly string StoreUrl = $"http://www.amazon.com/gp/mas/dl/android?p={Application.identifier}";
    #else
        private static readonly string StoreUrl = $"https://play.google.com/store/apps/details?id={Application.identifier}";
    #endif
#else
        private static readonly string StoreUrl = "https://itunes.apple.com/app/id1573861950?mt=8";
#endif
        public static void MoveToStore()
        {
            Application.OpenURL(StoreUrl);
        }
    }
}