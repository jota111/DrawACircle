using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameDataEditor;
using MoreLinq;
using OutGameCore;
using SH.Constant;
using SH.Game.DataLocal.Shop;
using SH.Game.Manager;
using SH.Game.UserData;
using SH.UI;
using SH.UI.View;
using SH.Util;
using UnityEngine;

namespace SH.Data
{
    public static class DataShop
    {
        private static List<GDEShopData> _shopDatas;
        private static List<GDEShop_InAppData> _shopInAppDatas;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        public static void CleanUp()
        {
            _shopDatas = null;
            _shopInAppDatas = null;
        }

        public static void SetAllDatas()
        {
            _shopDatas ??= GDEDataManager.GetAllItems<GDEShopData>();
            _shopInAppDatas ??= GDEDataManager.GetAllItems<GDEShop_InAppData>();
        }

        public static ShopBaseData GetData(string key)
        {
            var defaultData = GDEDataManager.Get<GDEShopData>(key);
            if (defaultData == null)
            {
                var inappData = GDEDataManager.Get<GDEShop_InAppData>(key);
                if (inappData == null) return null;
                ShopBaseData data = new ShopBaseData(inappData);
                return data;
            }

            ShopBaseData baseData = new ShopBaseData(defaultData);
            return baseData;
        }

        public static ShopBaseData GetConvertedData(string shopKey)
        {
            var baseData = GetData(shopKey);

            return baseData;
        }

        public static bool CheckCondition(ShopBaseData data)
        {
            var check = true;
            //레벨 조건 체크
            check &= data.OpenedLevel <= UserData.Instance.Level.Value;
            check &= data.RepeatInfo_LevelCondition == null || data.RepeatInfo_LevelCondition.Count == 0 ||
                     data.RepeatInfo_LevelCondition[0] == 0 || (data.RepeatInfo_LevelCondition[0] <= UserData.Instance.Level.Value &&
                                                                data.RepeatInfo_LevelCondition[1] >= UserData.Instance.Level.Value);
            return check;
        }

        /// <summary>
        /// 표시되는 데이터
        /// </summary>
        /// <returns></returns>
        public static List<ShopBaseData> GetAppearedDatas()
        {
            _shopDatas ??= GDEDataManager.GetAllItems<GDEShopData>();
            var datas = _shopDatas.Select(item => new ShopBaseData(item)).Where(CheckCondition).ToList();
            _shopInAppDatas ??= GDEDataManager.GetAllItems<GDEShop_InAppData>();
            var datasInApp = _shopInAppDatas.Select(item => new ShopBaseData(item)).Where(CheckCondition).ToList();

            //LimitedTime은 동일 키값 1번 Index만 가져와서
            var limitedTimeDict = new Dictionary<string, (ShopItemRepeatCollection, int)>();
            foreach (var tabID in LimitedTimeTabKeys)
            {
                var data = UserData.Instance.Shop.GetRepeatItems(tabID);
                int checkIndex = 0;
                if (data.Item == null)
                {
                    var list = datasInApp.FindAll(x => x.TabID.Equals(tabID));
                    if (list.Count > 0)
                        checkIndex = list[0].Index;
                    else checkIndex = -1;
                }

                limitedTimeDict.Add(tabID, (data, checkIndex));
            }

            //InApp ShopData 추가하기
            datas.AddRange(datasInApp.FindAll(x =>
                (limitedTimeDict.ContainsKey(x.TabID) && x.Index == (limitedTimeDict[x.TabID].Item1.Item != null
                    ? GDEDataManager.Get<GDEShop_InAppData>(limitedTimeDict[x.TabID].Item1.Item.ShopKey).Index
                    : limitedTimeDict[x.TabID].Item2) && !limitedTimeDict[x.TabID].Item1.CheckCooltime()) ||
                (!limitedTimeDict.ContainsKey(x.TabID))));
            //LimitedTime / LimitedTimeSpecial Cooltime아이템 추가
            datas.AddRange(datasInApp.FindAll(x =>
                (limitedTimeDict.ContainsKey(x.TabID) && limitedTimeDict[x.TabID].Item1.Item != null &&
                 limitedTimeDict[x.TabID].Item1.CoolTimeItem != null && limitedTimeDict[x.TabID].Item1.CheckCooltime()
                 && x.Index == GDEDataManager.Get<GDEShop_InAppData>(limitedTimeDict[x.TabID].Item1.CoolTimeItem.ShopKey).Index)));
            // datas.AddRange(datasInApp.FindAll(x=>x.))

            //LimitedPurchase SaleType 적용
            datas.RemoveAll(x => x.SaleTypes.Contains(SaleType.LimitedPurchase) &&
                                 UserData.Instance.Shop.CheckLimitedPurchase(x.Key, x.LimitedInfo_Cooltime));

            //레벨 오픈 적용
            datas.RemoveAll(x => x.SaleTypes.Contains(SaleType.LevelOpen) && x.OpenedLevel > 0 && x.OpenedLevel > UserData.Instance.Level.Value);

            //탭 오픈조건 체크
            ApplyTabOpenCondition(datas);

            return datas;
        }

        /// <summary>
        /// 모든 데이터
        /// </summary>
        /// <returns></returns>
        public static List<ShopBaseData> GetAllDatas()
        {
            _shopDatas ??= GDEDataManager.GetAllItems<GDEShopData>();
            var datas = _shopDatas.Select(item => new ShopBaseData(item)).ToList();
            _shopInAppDatas ??= GDEDataManager.GetAllItems<GDEShop_InAppData>();
            datas.AddRange(_shopInAppDatas.Select(item => new ShopBaseData(item)));

            //탭 오픈조건 체크
            ApplyTabOpenCondition(datas);

            return datas;
        }

        private static void ApplyTabOpenCondition(List<ShopBaseData> datas)
        {
            // var tabOrder = GDEDataManager.Get<GDEShop_BaseData>("TabOrder");
            var tabList = new List<string>();
            foreach (var shopData in datas.Where(shopData => !tabList.Contains(shopData.TabID)))
            {
                tabList.Add(shopData.TabID);
            }

            var eventData = datas.Find(x => !x.TabID.Equals(ShopTabID.EventSale.ToString()) && x.TabID.Contains(ShopTabID.EventSale.ToString()));
            if (eventData == null)
            {
                datas.RemoveAll(data =>
                    data.TabID.Equals(ShopTabID.EventSale.ToString()) || data.TabID.Equals(ShopTabID.EventSpecialSale.ToString()));
            }
            else
            {
                var eventItems = datas.FindAll(x => x.TabID.Equals(ShopTabID.EventSale.ToString()));
                foreach (var item in eventItems)
                    item.TabID = eventData.TabID;
            }
        }

        public static PurchaseType GetPurchaseType(PurchaseType def, Currencies type) => type switch
        {
            Currencies.Coin => PurchaseType.Coin,
            Currencies.Diamond => PurchaseType.Diamond,
            _ => def,
        };

        public static Currencies GetCurrencies(Currencies def, PurchaseType type) => type switch
        {
            PurchaseType.Coin => Currencies.Coin,
            PurchaseType.Diamond => Currencies.Diamond,
            _ => def,
        };

        public static string InappKeyToKey(string inappkey)
        {
            foreach (var shopData in DataShop.GetAllDatas()
                .Where(mspData => !string.IsNullOrEmpty(mspData.InAppKey) && mspData.InAppKey.Equals(inappkey)))
            {
                return shopData.Key;
            }

            return "Error";
        }

        public static GDEShop_InAppData GetDataByProductId(string productId)
        {
            var data = GDEDataManager.GetAllItems<GDEShop_InAppData>()
                .FirstOrDefault(item => item.InAppKey == productId);
            return data;
        }

        public static string GetShopDetailPopup(string key)
        {
            return key switch
            {
                "EnergyBooster_0" => UIViewName.BuyEnergyBooster,
                _ => null
            };
        }

        public static List<ShopTabID> LimitedTimeTabs
            => new List<ShopTabID>() { ShopTabID.LimitedTime, ShopTabID.LimitedTimeSpecial, ShopTabID.FlowerLimitedTime };

        public static List<string> LimitedTimeTabKeys
            => new List<string>()
                { ShopTabID.LimitedTime.ToString(), ShopTabID.LimitedTimeSpecial.ToString(), ShopTabID.FlowerLimitedTime.ToString() };

        public static List<string> LimitedTimeTabKeys_WithRemainTime
            => new List<string>()
                { ShopTabID.LimitedTime.ToString(), ShopTabID.FlowerLimitedTime.ToString() };
    }

    public class ShopBaseData
    {
        private GDEShopData shopData;
        private GDEShop_InAppData shopInAppData;

        public ShopBaseData()
        {
        }

        public ShopBaseData(GDEShop_InAppData shopInAppData)
        {
            this.shopInAppData = shopInAppData;

            Key = shopInAppData.Key;
            TabID = shopInAppData.TabID;
            Index = shopInAppData.Index;
            ItemIcon = shopInAppData.ItemIcon;
            ItemKey = shopInAppData.ItemKey;
            ItemCount = shopInAppData.ItemCount;
            RepeatInfo_LevelCondition = shopInAppData.RepeatInfo_LevelCondition;
            RepeatInfo_Duration = shopInAppData.RepeatInfo_Duration;
            RepeatInfo_Cooltime = shopInAppData.RepeatInfo_Cooltime;
            RepeatInfo_OpenOnCooltime = shopInAppData.RepeatInfo_OpenOnCooltime;
            LimitedInfo_Cooltime = shopInAppData.LimitedInfo_Cooltime;
            InAppKey = shopInAppData.InAppKey;
            SalePriceKey = shopInAppData.SalePriceKey;
            NameKey = shopInAppData.NameKey;
            PrefabName = shopInAppData.PrefabName;
            OpenedLevel = shopInAppData.OpenedLevel;
            OnlyQuestOpened = shopInAppData.OnlyQuestOpened;
            AfterClearQuest = shopInAppData.AfterClearQuest;
            InAppUSD = shopInAppData.InAppUSD;
            DiscountedRate = shopInAppData.DiscountedRate;

            SetSaleTypes(shopInAppData.SaleTypes);
            SetPurchaseType(shopInAppData.PurchaseType);
            SetHeight();
        }

        public ShopBaseData(GDEShopData shopData)
        {
            this.shopData = shopData;

            Key = shopData.Key;
            TabID = shopData.TabID;
            Index = shopData.Index;
            RefreshHours = shopData.RefreshHours;
            PurchaseCount = shopData.PurchaseCount;
            OpenedLevel = shopData.OpenedLevel;
            Cost_IncreasedMultiple = shopData.Cost_IncreasedMultiple;
            Cost_IncreasedValue = shopData.Cost_IncreasedValue;
            Cost_MaxValue = shopData.Cost_MaxValue;
            LimitedInfo_Cooltime = shopData.LimitedInfo_Cooltime;
            ItemIcon = shopData.ItemIcon;
            ItemKey = shopData.ItemKey;
            ItemCount = shopData.ItemCount;
            Cost = shopData.Cost;
            NameKey = shopData.NameKey;
            ItemRandomChance = shopData.ItemRandomChance;
            ItemCountRandom = shopData.ItemCountRandom;
            AfterClearQuest = shopData.AfterClearQuest;
            FlashGrouping = shopData.FlashGrouping;

            SetSaleTypes(shopData.SaleTypes);
            SetPurchaseType(shopData.PurchaseType);
            ShopItemSize = 1f;
            PrefabName = shopData.PrefabName;
            SetHeight();
        }

        private void SetHeight()
        {
            if (string.IsNullOrEmpty(PrefabName))
                PrefabName = "UIItem_Shop";
            if (PrefabName.Equals("UIItem_ShopLimitedTime")
                || PrefabName.Equals("UIItem_ShopExpandBoard")
                || PrefabName.Equals("UIItem_ShopExpandEventBoard"))
                ShopItemSize = 3f;
            else if (PrefabName.Equals("UIItem_ShopEnergyBooster"))
                ShopItemSize = 2f;
            else if (PrefabName.Equals("UIItem_Shop2"))
                ShopItemSize = 1.5f;
        }

        public string Key;
        public string TabID;
        public int Index;
        public List<SaleType> SaleTypes = new List<SaleType>();
        public List<int> RefreshHours;
        public int PurchaseCount;
        public int OpenedLevel;
        public float Cost_IncreasedMultiple;
        public int Cost_IncreasedValue;
        public int Cost_MaxValue;
        public string ItemIcon;
        public List<int> ItemRandomChance;
        public List<string> ItemKey;
        public List<int> ItemCount;
        public List<int> ItemCountRandom;
        public PurchaseType PurchaseType;
        public int Cost;
        public List<int> OnlyQuestOpened;
        public List<int> AfterClearQuest;
        public int FlashGrouping;
        public int DiscountedRate;

        //인앱 데이터 전용
        public List<int> RepeatInfo_LevelCondition;
        public int RepeatInfo_Duration;
        public int RepeatInfo_Cooltime;
        public bool RepeatInfo_OpenOnCooltime;
        public int LimitedInfo_Cooltime;
        public string InAppKey;
        public string SalePriceKey;
        public string NameKey;
        public string PrefabName;
        public float InAppUSD;

        //로컬 세팅
        public float ShopItemSize;
        public bool IsNewItem = false;

        public void SetPurchaseType(string purchaseType)
        {
            PurchaseType = purchaseType switch
            {
                "Free" => PurchaseType.Free,
                "Coins" => PurchaseType.Coin,
                "Diamonds" => PurchaseType.Diamond,
                "IA" => PurchaseType.InApp,
                "AD" => PurchaseType.AD,
                _ => PurchaseType
            };
        }

        public void SetSaleTypes(List<string> saleTypes)
        {
            SaleTypes.Clear();
            foreach (var type in saleTypes.Select(saleType => saleType switch
            {
                "TimeRefresh" => SaleType.TimeRefresh,
                "LevelOpen" => SaleType.LevelOpen,
                "IncreaseCost" => SaleType.IncreaseCost,
                "LimitedRepeat" => SaleType.LimitedRepeat,
                "LimitedPurchase" => SaleType.LimitedPurchase,
                "EventSale" => SaleType.EventSale,
                "FlashSale" => SaleType.FlashSale,
            }))
            {
                SaleTypes.Add(type);
            }
        }
    }

    public enum PurchaseType
    {
        None = 0,
        Free = 1,
        Coin = 2,
        Diamond = 3,
        InApp = 4,
        AD = 5,
    }

    public enum SaleType
    {
        None = 0,
        TimeRefresh = 1,
        LevelOpen = 2,
        IncreaseCost = 3,
        LimitedRepeat = 4,
        LimitedPurchase = 5,
        EventSale = 6,
        FlashSale = 7,
    }

    public enum ShopFrom
    {
        Lobby = 1,
        InGame = 2,
        InGameEvent = 3,
        InGameFlower = 4,
    }
}