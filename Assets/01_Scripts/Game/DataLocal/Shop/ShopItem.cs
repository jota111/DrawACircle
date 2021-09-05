/*
* Copyright (c) CookApps.
* 이진호(jhlee8@cookapps.com)
*/

using System;
using OutGameCore;
using SH.Constant;
using SH.Data;
using SH.Util;
using UniRx;

namespace SH.Game.DataLocal.Shop
{
    public abstract class ShopItemLocal
    {
        public string ShopKey { get; set; }
        public IntReactiveProperty PurchaseCount { get; set; }
        public long LatestPurchase { get; set; }

        public void ResetPurchaseCount()
        {
            PurchaseCount = new IntReactiveProperty(0);
        }
    }

    public sealed class ShopItemDailyLocal : ShopItemLocal
    {
    }

    /// <summary>
    /// 보드아이템 FlashSale
    /// </summary>
    public sealed class ShopItemBoardLocal : ShopItemLocal
    {
        public Currencies Currencies { get; set; }
        public MergeBoardIdent BoardIdent { get; set; }
        public string ItemKey { get; set; }
    }

    public interface IShopCollectionEnd
    {
        DateTime End { get; set; }
        int RefreshCount { get; set; }
    }
    
    public sealed class ShopItemRepeatCollection : IShopCollectionEnd
    {
        public DateTime End { get; set; } = DateTime.MinValue;
        public DateTime CoolTime { get; set; } = DateTime.MinValue;
        public ShopItemDailyLocal Item { get; set; }
        public ShopItemDailyLocal CoolTimeItem { get; set; }
        public int RefreshCount { get; set; }
        
        public void SetCoolTime(DateTime endTime) => CoolTime = endTime;
        public void SetEndTime(DateTime endTime) => End = endTime;

        public void SetItem(string key)
        {
            Item = new ShopItemDailyLocal()
            {
                ShopKey = key,
                PurchaseCount = new IntReactiveProperty(0),
            };
            CoolTimeItem = null;
        }

        public void SetCoolTimeItem(string key)
        {
            CoolTimeItem = new ShopItemDailyLocal()
            {
                ShopKey = key,
                PurchaseCount = new IntReactiveProperty(0),
            }; 
        }

        /// <summary>
        /// Progress -> Cooltime -> Expire
        /// Expire이면 다음 상품 세팅
        /// </summary>
        /// <returns></returns>
        public bool CheckProgress() => End >= GameUtils.GetGameTime();

        public bool CheckCooltime()
        {
            var gameTime = GameUtils.GetGameTime();
            var value = CoolTime >= gameTime && End < gameTime;
            return value;
        }

        public bool CheckExpire() => CoolTime < GameUtils.GetGameTime();

        public ProgressInfo GetProgressInfo()
        {
            if (CheckProgress())
            {
                return ProgressInfo.Progress;
            }else if (CheckCooltime())
            {
                return ProgressInfo.Cooltime;
            }else if (CheckExpire())
            {
                return ProgressInfo.Expire;
            }

            return ProgressInfo.Expire;
        }

        public enum ProgressInfo
        {
            None = 0,
            Progress = 1,
            Cooltime = 2,
            Expire = 3,
        }
    }
    
    public sealed class ShopItemDailyCollection : IShopCollectionEnd
    {
        public DateTime End { get; set; } = DateTime.MinValue;
        public int RefreshCount { get; set; }
        public ReactiveDictionary<string, ShopItemDailyLocal> Items { get; set; } = new ReactiveDictionary<string, ShopItemDailyLocal>();
        public bool CheckExpire() => End < GameUtils.GetGameTime();
    }

    public sealed class ShopItemBoardCollection : IShopCollectionEnd
    {
        public int RefreshCount { get; set; }
        public DateTime End { get; set; } = DateTime.MinValue;
        public MergeBoardIdent BoardIdent { get; set; }
        public ReactiveDictionary<string, ShopItemBoardLocal> Items { get; set; } = new ReactiveDictionary<string, ShopItemBoardLocal>();

        public void SetEndTime(DateTime endTime) => End = endTime;

        public void Clear()
        {
            foreach (var item in Items)
            {
                Items[item.Key].ResetPurchaseCount();
                Items[item.Key].ItemKey = "";
            }
        }

        public bool CheckExpire() => End < GameUtils.GetGameTime();

        public ShopItemBoardCollection()
        {
        }

        public ShopItemBoardCollection(MergeBoardIdent boardIdent)
        {
            BoardIdent = boardIdent;
        }
    }

    public enum ShopTabID
    {
        DailyDeals,
        FlashSale,
        Boxes,
        Energy,
        LimitedTime,
        LimitedTimeSpecial,
        Diamonds,
        Coins,
        
        EventSale,
        EventSpecialSale,
        EventSale_Lemon,
        
        FlowerSeedFlash,
        FlowerLimitedTime,
    }
}