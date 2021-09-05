/*
* Copyright (c) CookApps.
* 이진호(jhlee8@cookapps.com)
*/

using SH.Game.UserData;
using SH.Platform;

namespace SH.Net.Pkt
{
    public class ReqSave : IRequest
    {
        public UserData data { get; private set; }

        public ReqSave(UserData data, PlatformLoginId platformLoginId = null) : base("save", platformLoginId)
        {
            this.data = data;
        }
    }
}