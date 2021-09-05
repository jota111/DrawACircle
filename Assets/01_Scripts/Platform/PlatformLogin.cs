/*
* Copyright (c) CookApps.
* 이진호(jhlee8@cookapps.com)
*/

namespace SH.Platform
{
    public enum PlatformLoginType
    {
        Google = 1, Apple = 2, Facebook = 3
    }
    
    public sealed class PlatformLoginId
    {
        public string Google { get; }
        public string Apple { get; }
        public string Facebook { get; }

        public PlatformLoginId(string google = "", string apple = "", string facebook = "")
        {
            Google = google;
            Apple = apple;
            Facebook = facebook;
        }
    }
}