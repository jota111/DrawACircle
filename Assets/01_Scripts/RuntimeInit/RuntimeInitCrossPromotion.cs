/*
* Copyright (c) CookApps.
* 이진호(jhlee8@cookapps.com)
*/

using SH.CrossPromotion;
using SH.Net;
using UnityEngine;

namespace SH.RuntimeInit
{
    public static class RuntimeInitCrossPromotion
    {
        [RuntimeInitializeOnLoadMethod]
        private static void RuntimeInit()
        {
            CrossPromotionManager.Instance.Init("sunnyhouse", "get_xpm_info", NetDefine.PLATFORM);
        }
    }
}