/*
* Copyright (c) CookApps.
* 이진호(jhlee8@cookapps.com)
*/

using System;
using Cysharp.Threading.Tasks;
using GameDataEditor;
using OutGameCore;
using SH.Ad;
using SH.AppEvent;
using SH.Constant;
using SH.Data;
using SH.Game.DataLocal;
using SH.Game.DataLocal.Shop;
using SH.Game.Msg;
using SH.Game.Shop;
using SH.Game.UserData;
using SH.Game.UserManager;
using SH.Setting;
using SH.UI;
using SH.UI.Monetization;
using SH.Util;
using UniRx;
using UnityEngine;
using UnityEngine.Purchasing;
using Zenject;

namespace SH.Game.Manager
{
    public sealed class ShopManager : IDisposable
    {
        private readonly UserData.UserData _userData;
        private readonly ShopLocal _userDataShop;
        private readonly FundManager _fundManager;
        private readonly EnergyManager _energyManager;
        private readonly IapManager _iapManager;


        [InjectOptional] private GetEffectManager _getEffectManager;
        [InjectOptional] private UIViewBuyEnergy _uiViewBuyEnergy;
        [InjectOptional] private UIViewBuyEnergyBooster _uiViewBuyEnergyBooster;
        
        public static ShopManager Instance { get; private set; }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ReloadDomain()
        {
            Instance = null;
        }

        public ShopManager(UserData.UserData userData, FundManager fundManager, EnergyManager energyManager,
            IapManager iapManager)
        {
            _userData = userData;
            _userDataShop = userData.Shop;
            _fundManager = fundManager;
            _energyManager = energyManager;
            _iapManager = iapManager;
            Instance = this;
            DataShop.SetAllDatas();
        }

        public static void ShowShop(string TabType)
        {
            if (UIViewManager.CheckView(UIViewName.Shop))
                UIView_Shop.Instance.JumpToData(TabType, 0.5f);
            else
                UIView_Shop.Instance.Show(false, TabType);
        }

        /// <summary>
        /// 해당 상품 구매 가능 횟수
        /// </summary>
        public int GetPurchasableCount(string shopKey)
        {
            var data = DataShop.GetData(shopKey);
            if (data.PurchaseCount > 0)
            {
                var count = _userDataShop.GetPurchaseCount(shopKey);
                var purchasable = data.PurchaseCount - count;
                return Mathf.Max(purchasable, 0);
            }
            else return 1;
        }

        /// <summary>
        /// 해당 상품 구매횟수
        /// </summary>
        public int GetPurchaseCount(string shopKey)
        {
            var count = _userDataShop.GetPurchaseCount(shopKey);
            return count;
        }

        /// <summary>
        /// 해당 상품 가격
        /// </summary>
        public float GetCost(ShopBaseData baseData)
        {
            ShopBaseData data = DataShop.GetData(baseData.Key);
            var baseCost = data.Cost;
            if (baseData.SaleTypes.Contains(SaleType.FlashSale))
                baseCost = baseData.Cost;
            if (!data.SaleTypes.Contains(SaleType.IncreaseCost)) return baseCost;
            long cost = baseCost;
            var purchased = GetPurchaseCount(baseData.Key);
            if (data.Cost_IncreasedValue > 0)
                cost = purchased * data.Cost_IncreasedValue + baseCost;
            else
            {
                var pow = Math.Pow(data.Cost_IncreasedMultiple, purchased); 
                cost = (long)(pow * baseCost);
            }
            if (data.Cost_MaxValue != 0)
                cost = (long)Mathf.Min(cost, data.Cost_MaxValue);
            cost = (long)Mathf.Max(cost, 0);
            return cost;
        }

        public async UniTask<bool> TryPurchase(ShopBaseData baseData)
        {
            //인앱은 따로 처리
            if (baseData.PurchaseType == PurchaseType.InApp)
            {
                //var checkIA = await _inAppManager.BuyProductID(baseData.InAppKey);
                var checkIA = await _iapManager.AsyncPurchase(baseData.InAppKey);
                return checkIA;
            }

            var check = await CheckCanPurchase(baseData);
            if (check == false) return false;
            PurchaseSucceed(baseData.Key);
            return true;
        }

        public bool IsPurchasable(ShopBaseData baseData)
        {
            if (baseData.PurchaseCount > 0 && GetPurchasableCount(baseData.Key) <= 0)
                return false;
            if (baseData.SaleTypes.Contains(SaleType.LimitedPurchase))
            {
                var check = _userData.Shop.CheckLimitedPurchase(baseData.Key, baseData.LimitedInfo_Cooltime);
                if (check)
                    return false;
            }
            return true;
        }

        private async UniTask<bool> CheckCanPurchase(ShopBaseData baseData)
        {
            if (!IsPurchasable(baseData)) return false;

            switch (baseData.PurchaseType)
            {
                case PurchaseType.InApp:
                    return true;
                case PurchaseType.Free:
                    return true;
                case PurchaseType.Coin:
                    var checkCoin = GetCost(baseData) <= _fundManager.Gold.Value;
                    if (!checkCoin)
                        FundManager.DialogNotEnoughResource("Coins");
                    return checkCoin;
                case PurchaseType.Diamond:
                    var checkDiamond = GetCost(baseData) <= _fundManager.Jewel.Value;
                    if (!checkDiamond)
                        FundManager.DialogNotEnoughResource("Diamonds");
                    return checkDiamond;
                case PurchaseType.AD:
                    var checkAD = await AdNetworkManager.Instance.AsyncShowRewardAd(AppEventManager.RewardAdsType.etc); // (AppEventManager.RewardAdsType)
                    return checkAD;
            }

            return false;
        }

        public async UniTask<bool> CheckCanPurchase(string shopKey)
        {
            var check = await CheckCanPurchase(DataShop.GetData(shopKey));
            return check;
        }

        public async UniTask<bool> CheckCanPurchase(PurchaseType purchaseType, int count = 0, params object[] args)
        {
            switch (purchaseType)
            {
                case PurchaseType.Free:
                    return true;
                case PurchaseType.Coin:
                    var checkCoin = count <= _fundManager.Gold.Value;
                    if (!checkCoin)
                        FundManager.DialogNotEnoughResource("Coins");
                    return checkCoin;
                case PurchaseType.Diamond:
                    var checkDiamond = count <= _fundManager.Jewel.Value;
                    if (!checkDiamond)
                        FundManager.DialogNotEnoughResource("Diamonds");
                    return checkDiamond;
                case PurchaseType.AD:
                    var checkAD = await AdNetworkManager.Instance.AsyncShowRewardAd((AppEventManager.RewardAdsType)args[0]);
                    return checkAD;
            }

            return false;
        }

        public void PurchaseSucceedProductId(string productId)
        { 
            var shopKey = DataShop.InappKeyToKey(productId);
            _userDataShop.SetPayInfo(shopKey);
            
            PurchaseSucceed(shopKey);
        }

        private void PurchaseSucceed(string shopKey)
        {
            var baseData = PuchaseSucced_Base(shopKey);

            //상점 뷰 세팅
            if (baseData.SaleTypes.Contains(SaleType.LimitedRepeat)) //TabID.Equals(ShopTabID.LimitedTime.ToString()))
            {
                var tabData = UserData.UserData.Instance.Shop.GetRepeatItems(baseData.TabID);
                var data = DataShop.GetData(baseData.Key);
                var coolTime = GameUtils.GetGameTime().AddMinutes(data.RepeatInfo_Cooltime);
                tabData.SetEndTime(GameUtils.GetGameTime());
                tabData.SetCoolTime(coolTime);
                if (data.RepeatInfo_OpenOnCooltime && tabData.CoolTimeItem != null && tabData.CoolTimeItem.ShopKey.Equals(shopKey))
                    tabData.CoolTimeItem = null;
                UIView_Shop.Instance.ResetView();
            }
            else if (baseData.SaleTypes.Contains(SaleType.LimitedPurchase))
            {
                UIView_Shop.Instance.ResetView();
            }
            else
            {
                if (shopKey.Equals("Energy_0") && (UIView_Shop.Instance == null || UIView_Shop.Instance.gameObject.activeSelf == false) && _uiViewBuyEnergy != null)
                    _uiViewBuyEnergy.Reset();
                else
                    UIView_Shop.Instance.RefreshActiveCellViews();
            }
        }

        public void PurchaseSucceed_Force(string shopKey)
        {
            PuchaseSucced_Base(shopKey);
        }

        private ShopBaseData PuchaseSucced_Base(string shopKey)
        {
            var baseData = DataShop.GetConvertedData(shopKey);
            DeductCost(baseData);
            ProvideItems(baseData);
            // Invalidate.Publish(Invalidate.TargetType.All);

            _userDataShop.AddPurchaseCount(shopKey);
            // 특정 상품 추가 처리
            if (shopKey.Equals("EnergyBooster_0")) // && _userDataShop.CheckEnergyBooster()) 처리가 안될경우를 위한 예외처리
            {
                UserData.UserData.Instance.EnergyBoosterEndTime.Value = _userDataShop.GetEnergyBoosterExpiredTime();
            }

            UserDataSaveManager.Instance.Save();
            UserDataSaveServerManager.Instance?.RequestAsync();
            MessageBroker.Default.Publish(new PurchaseSucceed(shopKey));
            return baseData;
        }

        private void DeductCost(ShopBaseData baseData)
        {
            switch (baseData.PurchaseType)
            {
                case PurchaseType.Free:
                    break;
                case PurchaseType.Coin:

                    #region AppEvent

                    var appEvent = AppEventManager.CoinSpent.ETC;
                    if (baseData.TabID.Equals(ShopTabID.FlashSale.ToString()))
                        appEvent = AppEventManager.CoinSpent.Flash_Sale;
                    else if (baseData.TabID.Equals(ShopTabID.DailyDeals.ToString()))
                        appEvent = AppEventManager.CoinSpent.Daily_Deals;
                    else if (baseData.TabID.Equals(ShopTabID.Boxes.ToString()))
                        appEvent = AppEventManager.CoinSpent.Boxes;

                    #endregion

                    var cost = GetCost(baseData);
                    _fundManager.UseGold((int) cost, appEvent);
                    break;
                case PurchaseType.Diamond:

                    #region AppEvent

                    var appEvent2 = AppEventManager.JewelSpent.Shop;
                    if (baseData.TabID.Equals(ShopTabID.Energy.ToString()))
                        appEvent2 = AppEventManager.JewelSpent.Energy;

                    #endregion

                    var cost2 = GetCost(baseData);
                    _fundManager.UseJewel((int) cost2, appEvent2);
                    break;
                case PurchaseType.AD:
                    break;
                case PurchaseType.InApp:
                    break;
            }
        }

        public void DeductCost(string shopKey)
        {
            DeductCost(DataShop.GetData(shopKey));
        }

        public void DeductCost(PurchaseType purchaseType, int cost = 0, params object[] args)
        {
            switch (purchaseType)
            {
                case PurchaseType.Free:
                    break;
                case PurchaseType.Coin:
                    _fundManager.UseGold(cost);
                    break;
                case PurchaseType.Diamond:
                    _fundManager.UseJewel(cost, (AppEventManager.JewelSpent)args[0]);
                    break;
                case PurchaseType.AD:
                    break;
                case PurchaseType.InApp:
                    break;
            }
        }

        private (string, int) ProvideItems(ShopBaseData baseData, params Vector3[] pos)
        {
            var shopItem = UIView_Shop.Instance.gameObject.activeSelf == false
                ? (_uiViewBuyEnergy.gameObject.activeSelf ? _uiViewBuyEnergy.GetItem : (_uiViewBuyEnergyBooster.gameObject.activeSelf ? _uiViewBuyEnergyBooster.GetItem : null))
                : UIView_Shop.Instance.GetItem(baseData.Key);

            string provideItem = "";
            int provideItemCount = 0;
            

            if (!CheckRandomItem())
            {
                for (int i = 0; i < baseData.ItemKey.Count; i++)
                {
                    var item = baseData.ItemKey[i];
                    var count = baseData.ItemCount[i];
                    ProvideItem(item, count, i);
                    provideItem = item;
                    provideItemCount = count;
                }
            }
            else
            {
                var index = GameUtils.RandomWeightedList(baseData.ItemRandomChance);
                var item = baseData.ItemKey[index];
                var count = baseData.ItemCount.Count != baseData.ItemCountRandom.Count
                    ? baseData.ItemCount[index]
                    : GameUtils.RandomRange(baseData.ItemCount[index], baseData.ItemCountRandom[index]);
                ProvideItem(item, count, 0);
                provideItem = item;
                provideItemCount = count;
            }

            return (provideItem, provideItemCount);

            bool CheckRandomItem()
            {
                return baseData.ItemRandomChance != null && baseData.ItemRandomChance.Count > 0;
            }

            void ProvideItem(string item, int count, int i)
            {
                if (item.Equals(Currencies.Diamond.ToString()))
                {
                    GameSoundManager.Instance.PlaySfx(SFX.sh_tap_jewel);
                    _fundManager.AddJewel(count, AppEventManager.JewelEarnedBy.Purchased_with_Money);
                    if (pos.Length > 0)
                        _getEffectManager?.Get(DataCurrencies.GetDiamondShopIcon(count), pos[0]);
                    else if (shopItem != null && shopItem.GetItemInfos[i] != null)
                        _getEffectManager?.Get(DataCurrencies.GetDiamondShopIcon(count), shopItem.GetItemInfos[i].GetIconPos);
                }
                else if (item.Equals(Currencies.Coin.ToString()))
                {
                    GameSoundManager.Instance.PlaySfx(SFX.sh_tap_coin);
                    _fundManager.AddGold(count, baseData.Key.Equals("AdsInteraction_0")?AppEventManager.CoinEarnedBy.LobbyAds:AppEventManager.CoinEarnedBy.Purchased_with_Money);
                    if (pos.Length > 0)
                        _getEffectManager?.Get(DataCurrencies.GetCoinShopIcon(count), pos[0]);
                    else if (shopItem != null && shopItem.GetItemInfos[i] != null)
                        _getEffectManager?.Get(DataCurrencies.GetCoinShopIcon(count), shopItem.GetItemInfos[i].GetIconPos);
                }
                else if (item.Equals(Currencies.Energy.ToString()))
                {
                    GameSoundManager.Instance.PlaySfx(SFX.sh_tap_energy);
                    _energyManager.AddEnergy(count);
                    if (pos.Length > 0)
                        _getEffectManager?.Get(DataCurrencies.GetEnergyIcon(count), pos[0]);
                    else if (shopItem != null && shopItem.GetItemInfos[i] != null)
                        _getEffectManager?.Get(DataCurrencies.GetEnergyIcon(count), shopItem.GetItemInfos[i].GetIconPos);
                }
            }
        }

        public (string, int) ProvideItems(string shopKey, params Vector3[] pos)
        {
            return ProvideItems(DataShop.GetData(shopKey), pos);
        }

        private (string, int) JustProvideItems(ShopBaseData baseData, params Vector3[] pos)
        {
            var shopItem = UIView_Shop.Instance.gameObject.activeSelf == false
                ? (_uiViewBuyEnergy.gameObject.activeSelf ? _uiViewBuyEnergy.GetItem : (_uiViewBuyEnergyBooster.gameObject.activeSelf ? _uiViewBuyEnergyBooster.GetItem : null))
                : UIView_Shop.Instance.GetItem(baseData.Key);

            string provideItem = "";
            int provideItemCount = 0;
            

            if (!CheckRandomItem())
            {
                for (int i = 0; i < baseData.ItemKey.Count; i++)
                {
                    var item = baseData.ItemKey[i];
                    var count = baseData.ItemCount[i];
                    ProvideItem(item, count);
                    provideItem = item;
                    provideItemCount = count;
                }
            }
            else
            {
                var index = GameUtils.RandomWeightedList(baseData.ItemRandomChance);
                var item = baseData.ItemKey[index];
                var count = baseData.ItemCount.Count != baseData.ItemCountRandom.Count
                    ? baseData.ItemCount[index]
                    : GameUtils.RandomRange(baseData.ItemCount[index], baseData.ItemCountRandom[index]);
                ProvideItem(item, count);
                provideItem = item;
                provideItemCount = count;
            }

            return (provideItem, provideItemCount);

            bool CheckRandomItem()
            {
                return baseData.ItemRandomChance != null && baseData.ItemRandomChance.Count > 0;
            }

            void ProvideItem(string item, int count)
            {
                if (item.Equals(Currencies.Diamond.ToString()))
                {
                    GameSoundManager.Instance.PlaySfx(SFX.sh_tap_jewel);
                    _fundManager.AddJewel(count, AppEventManager.JewelEarnedBy.Purchased_with_Money);
                }
                else if (item.Equals(Currencies.Coin.ToString()))
                {
                    GameSoundManager.Instance.PlaySfx(SFX.sh_tap_coin);
                    _fundManager.AddGold(count, baseData.Key.Equals("AdsInteraction_0")?AppEventManager.CoinEarnedBy.LobbyAds:AppEventManager.CoinEarnedBy.Purchased_with_Money);
                }
                else if (item.Equals(Currencies.Energy.ToString()))
                {
                    GameSoundManager.Instance.PlaySfx(SFX.sh_tap_energy);
                    _energyManager.AddEnergy(count);
                }
            }
        }

        public (string, int) JustProvideItems(string shopKey, params Vector3[] pos)
        {
            return JustProvideItems(DataShop.GetData(shopKey), pos);
        }

        public int GetCurrentCurrency(Currencies type)
        {
            switch (type)
            {
                case Currencies.Coin:
                    return _userData.Gold.Value;
                case Currencies.Diamond:
                    return _userData.Jewel.Value;
                case Currencies.XP:
                    return _userData.Exp.Value;
                case Currencies.Energy:
                    return _userData.Energy.Value;
                default:
                    return 0;
            }
        }

        public void ConsumeCurrency(Currencies type, int cost)
        {
            switch (type)
            {
                case Currencies.Coin:
                    _fundManager.UseGold(cost);
                    Invalidate.Publish(Invalidate.TargetType.Gold);
                    break;
                case Currencies.Diamond:
                    _fundManager.UseJewel(cost);
                    Invalidate.Publish(Invalidate.TargetType.Jewel);
                    break;
                case Currencies.Energy:
                    _energyManager.UseEnergy(cost);
                    Invalidate.Publish(Invalidate.TargetType.Energy);
                    break;
                default:
                    break;
            }
        }

        public void Dispose()
        {
            Instance = null;
        }
    }
}