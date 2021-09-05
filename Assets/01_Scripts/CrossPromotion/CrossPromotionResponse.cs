/*
* Copyright (c) CookApps.
* 이진호(jhlee8@cookapps.com)
*/

using System;
using UnityEngine;

namespace SH.CrossPromotion
{
    [Serializable]
    public class CrossPromotionResponse
    {
        [Serializable]
        public class Data
        {
            public int app_id;
            public string app_nick;
            public int target_app_id;

            public int show_lobby_icon_on;
            public int show_life_zero_on;
            public int show_fail_lobby_on;

            public string url;
            public string banner_image;
            public string icon_image;
        }

        public bool success;
        public Data data;
    }

    public class CrossPromotionData
    {
        public CrossPromotionResponse.Data Data { get; internal set; }
        public Texture2D Icon { get; internal set; }
        public Texture2D Banner { get; internal set; }
    }
}