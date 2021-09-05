/*
* Copyright (c) CookApps.
* 이진호(jhlee8@cookapps.com)
*/

using System;
using System.Linq;
using GameDataEditor;
using OutGameCore;
using MoreLinq;
using SH.Constant;
using SH.Data;
using SH.Game.DataLocal.Shop;
using UniRx;

namespace SH.Game.DataLocal
{
    public sealed class ShopLocal
    {
        #region 데이터

        /// <summary>
        /// 일반
        /// </summary>
        public ReactiveDictionary<string, ShopItemDailyCollection> ShopItems { get; set; } =
            new ReactiveDictionary<string, ShopItemDailyCollection>();

        /// <summary>
        /// FlashSale
        /// </summary>
        public ReactiveDictionary<MergeBoardIdent, ShopItemBoardCollection> Board { get; set; } =
            new ReactiveDictionary<MergeBoardIdent, ShopItemBoardCollection>();

        /// <summary>
        /// 반복 패키지
        /// </summary>
        public ReactiveDictionary<string, ShopItemRepeatCollection> RepeatItems { get; set; } =
            new ReactiveDictionary<string, ShopItemRepeatCollection>();

        public DateTime RedDotTime;

        public bool IsPayUser = false;
        public int PayCount = 0;
        public string LastPayItem;
        public DateTime LastPayTime;
        public int LastPayJewel;
        public int LastPayCoin;

        #endregion

        #region Shop아이템 데이터

        public ShopItemDailyCollection GetDailyItems(string tabID)
        {
            if (ShopItems.ContainsKey(tabID) == false)
            {
                ShopItems.Add(tabID, new ShopItemDailyCollection());
            }

            return ShopItems[tabID];
        }

        #endregion

        #region Board아이템 데이터

        public ShopItemBoardCollection GetBoard(MergeBoardIdent board)
        {
            if (Board.ContainsKey(board) == false)
            {
                Board.Add(board, new ShopItemBoardCollection(board));
            }

            return Board[board];
        }

        public void SetBoard(MergeBoardIdent board, ShopItemBoardCollection boardData)
        {
            if (Board.ContainsKey(board))
                Board[board] = boardData;
            else
                Board.Add(board, boardData);
        }

        public void AddShopItem(MergeBoardIdent board, string shopKey, string itemKey, Currencies currencyType)
        {
            if (Board.ContainsKey(board) == false)
                Board.Add(board, new ShopItemBoardCollection(board));
            var newData = new ShopItemBoardLocal()
            {
                BoardIdent = board,
                ShopKey = shopKey,
                ItemKey = itemKey,
                Currencies = currencyType,
                PurchaseCount = new IntReactiveProperty(0),
            };
            if (Board[board].Items.ContainsKey(shopKey))
                Board[board].Items[shopKey] = newData;
            else
                Board[board].Items.Add(shopKey, newData);
        }

        public ShopItemBoardLocal GetShopItem(MergeBoardIdent board, string key)
        {
            if (Board.ContainsKey(board) == false) return null;
            var dataSet = Board[board];
            return dataSet.Items.ContainsKey(key) == false ? null : dataSet.Items[key];
        }

        #endregion

        #region Repeat아이템 데이터

        public ShopItemRepeatCollection GetRepeatItems(string tabID)
        {
            if (RepeatItems.ContainsKey(tabID) == false)
            {
                RepeatItems.Add(tabID, new ShopItemRepeatCollection());
            }

            return RepeatItems[tabID];
        }

        #endregion

        #region 함수

        public int GetPurchaseCount(string key)
        {
            var baseData = DataShop.GetData(key);
            if (DataShop.LimitedTimeTabKeys.Contains(baseData.TabID))
            {
                if (RepeatItems.ContainsKey(baseData.TabID) == false)
                    RepeatItems.Add(baseData.TabID, new ShopItemRepeatCollection());
                if (RepeatItems[baseData.TabID].Item == null)
                    RepeatItems[baseData.TabID].SetItem(key);
                if (RepeatItems[baseData.TabID].CoolTimeItem != null && RepeatItems[baseData.TabID].CoolTimeItem.ShopKey.Equals(key)
                                                                     && RepeatItems[baseData.TabID].CheckCooltime())
                    return RepeatItems[baseData.TabID].CoolTimeItem.PurchaseCount.Value;

                return RepeatItems[baseData.TabID].Item.PurchaseCount.Value;
            }

            if (ShopItems.ContainsKey(baseData.TabID) == false) return 0;
            var dataSet = ShopItems[baseData.TabID];
            if (dataSet.Items.ContainsKey(key) == false) return 0;
            return dataSet.Items[key].PurchaseCount.Value;
        }

        public ShopItemLocal GetShopItemLocal(string key)
        {
            ShopItemLocal data = null;
            var baseData = DataShop.GetData(key);
            if (DataShop.LimitedTimeTabKeys.Contains(baseData.TabID))
            {
                if (RepeatItems.ContainsKey(baseData.TabID) == false)
                    RepeatItems.Add(baseData.TabID, new ShopItemRepeatCollection());
                if (RepeatItems[baseData.TabID].Item == null)
                    RepeatItems[baseData.TabID].SetItem(key);
                if (RepeatItems[baseData.TabID].CoolTimeItem != null && RepeatItems[baseData.TabID].CoolTimeItem.ShopKey.Equals(key)
                                                                     && RepeatItems[baseData.TabID].CheckCooltime())
                    data = RepeatItems[baseData.TabID].CoolTimeItem;
                else
                    data = RepeatItems[baseData.TabID].Item;
            }
            else
            {
                if (ShopItems.ContainsKey(baseData.TabID) == false) return null;
                var dataSet = ShopItems[baseData.TabID];
                if (dataSet.Items.ContainsKey(key) == false) return null;
                data = dataSet.Items[key];
            }

            return data;
        }

        public void SetPurchaseCount(string key, int count)
        {
            var baseData = DataShop.GetData(key);
            if (DataShop.LimitedTimeTabKeys.Contains(baseData.TabID))
            {
                if (RepeatItems.ContainsKey(baseData.TabID) == false)
                    RepeatItems.Add(baseData.TabID, new ShopItemRepeatCollection());
                if (RepeatItems[baseData.TabID].Item == null)
                    RepeatItems[baseData.TabID].SetItem(key);
                if (RepeatItems[baseData.TabID].CoolTimeItem != null && RepeatItems[baseData.TabID].CoolTimeItem.ShopKey.Equals(key)
                                                                     && RepeatItems[baseData.TabID].CheckCooltime())
                {
                    RepeatItems[baseData.TabID].CoolTimeItem.PurchaseCount.Value = count;
                }
                else
                {
                    RepeatItems[baseData.TabID].Item.PurchaseCount.Value = count;
                }
            }
            else
            {
                if (ShopItems.ContainsKey(baseData.TabID) == false)
                    ShopItems.Add(baseData.TabID, new ShopItemDailyCollection());
                if (ShopItems[baseData.TabID].Items.ContainsKey(key) == false)
                {
                    ShopItems[baseData.TabID].Items.Add(key, new ShopItemDailyLocal()
                    {
                        ShopKey = key,
                        PurchaseCount = new IntReactiveProperty(count),
                    });
                }
                else
                {
                    ShopItems[baseData.TabID].Items[key].PurchaseCount.Value = count;
                }
            }
        }

        public void AddPurchaseCount(string key)
        {
            var baseData = DataShop.GetData(key);
            if (DataShop.LimitedTimeTabKeys.Contains(baseData.TabID))
            {
                if (RepeatItems.ContainsKey(baseData.TabID) == false)
                    RepeatItems.Add(baseData.TabID, new ShopItemRepeatCollection());
                if (RepeatItems[baseData.TabID].Item == null)
                    RepeatItems[baseData.TabID].SetItem(key);
                if (RepeatItems[baseData.TabID].CoolTimeItem != null && RepeatItems[baseData.TabID].CoolTimeItem.ShopKey.Equals(key)
                                                                     && RepeatItems[baseData.TabID].CheckCooltime())
                {
                    RepeatItems[baseData.TabID].CoolTimeItem.PurchaseCount.Value++;
                    RepeatItems[baseData.TabID].CoolTimeItem.LatestPurchase = GameUtils.GetGameTime().Ticks;
                }
                else
                {
                    RepeatItems[baseData.TabID].Item.PurchaseCount.Value++;
                    RepeatItems[baseData.TabID].Item.LatestPurchase = GameUtils.GetGameTime().Ticks;
                }
            }
            else
            {
                if (ShopItems.ContainsKey(baseData.TabID) == false)
                    ShopItems.Add(baseData.TabID, new ShopItemDailyCollection());
                if (ShopItems[baseData.TabID].Items.ContainsKey(key) == false)
                {
                    ShopItems[baseData.TabID].Items.Add(key, new ShopItemDailyLocal()
                    {
                        ShopKey = key,
                        PurchaseCount = new IntReactiveProperty(1),
                        LatestPurchase = GameUtils.GetGameTime().Ticks
                    });
                }
                else
                {
                    ShopItems[baseData.TabID].Items[key].PurchaseCount.Value++;
                    ShopItems[baseData.TabID].Items[key].LatestPurchase = GameUtils.GetGameTime().Ticks;
                }
            }
        }

        public void SetEndTime(string key, DateTime endTime)
        {
            var baseData = DataShop.GetData(key);
            if (DataShop.LimitedTimeTabKeys.Contains(baseData.TabID))
            {
                if (RepeatItems.ContainsKey(baseData.TabID) == false)
                    RepeatItems.Add(baseData.TabID, new ShopItemRepeatCollection());
                RepeatItems[baseData.TabID].End = endTime;
            }
            else
            {
                if (ShopItems.ContainsKey(baseData.TabID) == false)
                    ShopItems.Add(baseData.TabID, new ShopItemDailyCollection());
                ShopItems[baseData.TabID].End = endTime;
            }
        }

        public DateTime GetEndTime(string key)
        {
            var baseData = DataShop.GetData(key);
            if (DataShop.LimitedTimeTabKeys.Contains(baseData.TabID))
            {
                if (RepeatItems.ContainsKey(baseData.TabID) == false)
                    RepeatItems.Add(baseData.TabID, new ShopItemRepeatCollection());
                return RepeatItems[baseData.TabID].End;
            }

            if (ShopItems.ContainsKey(baseData.TabID) == false) return DateTime.MinValue;
            var dataSet = ShopItems[baseData.TabID];
            return dataSet.End;
        }


        public void AddRefreshCount(string TabID, int value)
        {
            if (DataShop.LimitedTimeTabKeys.Contains(TabID))
            {
                if (RepeatItems.ContainsKey(TabID) == false)
                    RepeatItems.Add(TabID, new ShopItemRepeatCollection());
                RepeatItems[TabID].RefreshCount += value;
            }
            else
            {
                if (ShopItems.ContainsKey(TabID) == false)
                    ShopItems.Add(TabID, new ShopItemDailyCollection());
                ShopItems[TabID].RefreshCount += value;
            }
        }

        public int GetRefreshCount(string TabID)
        {
            if (DataShop.LimitedTimeTabKeys.Contains(TabID))
            {
                if (RepeatItems.ContainsKey(TabID) == false)
                    RepeatItems.Add(TabID, new ShopItemRepeatCollection());
                return RepeatItems[TabID].RefreshCount;
            }

            if (ShopItems.ContainsKey(TabID) == false) return 0;
            var dataSet = ShopItems[TabID];
            return dataSet.RefreshCount;
        }

        #endregion

        #region 이벤트

        public bool CheckEventOpen(string tabID)
        {
            return false;
        }

        #endregion

        #region 스페셜 상점아이템 체크

        /// <summary>
        /// 보드판 확장이 적용됐나
        /// </summary>
        /// <returns></returns>
        public bool CheckEventBoardExpand()
        {
            var shopData = DataShop.GetData("EventBoardExpand_0");
            bool check = CheckLimitedPurchase("EventBoardExpand_0", shopData.LimitedInfo_Cooltime);
            return check;
        }

        /// <summary>
        /// 보드판 확장이 적용됐나
        /// </summary>
        /// <returns></returns>
        public bool CheckBoardExpand()
        {
            var shopData = DataShop.GetData("BoardExpand_0");
            bool check = CheckLimitedPurchase("BoardExpand_0", shopData.LimitedInfo_Cooltime);
            return check;
        }

        /// <summary>
        /// 에너지 부스터 켜져있나
        /// </summary>
        /// <returns></returns>
        public bool CheckEnergyBooster()
        {
            var shopData = DataShop.GetData("EnergyBooster_0");
            bool check = CheckLimitedPurchase("EnergyBooster_0", shopData.LimitedInfo_Cooltime);
            return check;
        }

        public bool CheckLimitedPurchase(string key, int coolTimeHours)
        {
            var check = false;
            var data = GetShopItemLocal(key);
            if (data != null)
            {
                var timeSpan = GameUtils.GetGameTime().Subtract(new DateTime(data.LatestPurchase));
                var totalHours = timeSpan.TotalHours;
                check = data.PurchaseCount.Value > 0 && ((coolTimeHours == 0) || (totalHours < coolTimeHours));
            }

            return check;
        }

        // /// <summary>
        // /// 에너지 부스터 마감시간
        // /// </summary>
        // /// <returns></returns>
        public DateTime GetEnergyBoosterExpiredTime()
        {
            var shopData = DataShop.GetData("EnergyBooster_0");
            var data = GetShopItemLocal("EnergyBooster_0");
            DateTime expired = DateTime.MinValue;
            if (data.PurchaseCount.Value > 0)
                expired = new DateTime(data.LatestPurchase).AddHours(shopData.LimitedInfo_Cooltime);
            return expired;
        }

        #endregion
        
        #region 결제

        public void SetPayInfo(string shopKey)
        {
            if (!IsPayUser || PayCount > 0 || UserData.UserData.Instance.FirstInstallVersion >= 1026)
            {
                PayCount++;
                LastPayItem = shopKey;
                LastPayTime = GameUtils.GetGameTime();
                LastPayJewel = UserData.UserData.Instance.Jewel.Value;
                LastPayCoin = UserData.UserData.Instance.Gold.Value;
            }
            IsPayUser = true;
        }

        #endregion
    }
}