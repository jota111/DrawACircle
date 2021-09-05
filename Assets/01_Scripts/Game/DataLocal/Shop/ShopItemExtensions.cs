/*
* Copyright (c) CookApps.
* 이진호(jhlee8@cookapps.com)
*/

using JetBrains.Annotations;
using SH.Util;

namespace SH.Game.DataLocal.Shop
{
    public static class ShopItemExtensions
    {
        public static bool Expire([NotNull]this IShopCollectionEnd collection)
        {
            return collection.End >= TimeUtil.Now;
        }
    }
}