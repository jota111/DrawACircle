/*
* Copyright (c) CookApps.
* 이진호(jhlee8@cookapps.com)
*/

using UnityNative.Toasts;

namespace SH.Util
{
    public static class ToastHelper
    {
        private static readonly IUnityNativeToasts unityNativeToasts;

        static ToastHelper()
        {
            unityNativeToasts = UnityNativeToasts.Create();
        }

        public static void Show(string toastText)
        {
            unityNativeToasts.ShowShortToast(toastText);
        }
    }
}