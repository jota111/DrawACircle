/*
* Copyright (c) CookApps.
* 이진호(jhlee8@cookapps.com)
*/

// ReSharper disable UnusedAutoPropertyAccessor.Local

namespace SH.Net.Pkt
{
    public class ReqVersion : IRequest
    {
        public string country_code { get; private set; }
        
        public ReqVersion(string countryCode = "") : base("/auth/version")
        {
            country_code = countryCode;
        }
    }

    public class RecvVersion : Recv
    {
        public class Data
        {
            public string device { get; private set; }
            public int version { get; private set; }
            public bool force_update { get; private set; }
            public string link { get; private set; }
        }

        public Data data { get; private set; }
    }
}