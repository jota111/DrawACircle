/*
* Copyright (c) CookApps.
* 이진호(jhlee8@cookapps.com)
*/

namespace SH.Game.Msg
{
    public class PurchaseSucceed
    {
        public readonly string ShopKey;

        public PurchaseSucceed(string shopKey)
        {
            this.ShopKey = shopKey;
        }
    }
}