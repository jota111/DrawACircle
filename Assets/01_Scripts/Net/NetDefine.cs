/*
* Copyright (c) CookApps.
* 이진호(jhlee8@cookapps.com)
*/

namespace SH.Net
{
    public static class NetDefine
    {
#if __DEV
        public const string NET_SERVER_ADDR = "https://sunnyhouse.cookapps.com/api/";
#else
        public const string NET_SERVER_ADDR = "https://sunnyhouse.cookappsgames.com/api/";
#endif
        
        public const int NET_TIMEOUT = 15;
        
#if UNITY_AWS
        public const string PLATFORM = "aws";
#elif UNITY_ANDROID
        public const string PLATFORM = "android";
#else
        public const string PLATFORM = "ios";
#endif
    }
}