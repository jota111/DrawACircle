using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using OutGameCore;
using MoreLinq.Extensions;
using SH.AppEvent;
using SH.Data;
using SH.Game.DataLocal;
using SH.Game.Manager;
using SH.Game.Tutorial;
using SH.UI.View;
using UniRx;
using UnityEngine;
using Zenject;

namespace SH.Game.Shop
{
    public class UIViewBuyEnergy : UIViewBase
    {
        [SerializeField] private UIItem_Shop shopItem1;
        [SerializeField] private UIItem_Shop shopItem2;
        [SerializeField] private UIItem_Shop shopItemAds;
        [SerializeField] private UIItem_Shop shopBannerItem;
        [SerializeField] private GameObject goShop;
        [SerializeField] private GameObject goEnergy;
        [SerializeField] private GameObject goEnergyAds;
        public UIItem_Shop GetItem;
        private ShopManager shopManager;
        private const string targetKey = "Energy_0";
        private const string adsTargetKey = "AdsEnergy_0";
        private const string bannerTargetKey = "EnergyBooster_0";
        private IContentPossible _contentPossible;
        private bool _isShowedAdsEnergy = false;
        private ShopLocal _shopLocal;

        [Inject]
        private void Construct(ShopManager _shopManager, IContentPossible contentPossible, UserData.UserData userData)
        {
            shopManager = _shopManager;
            _contentPossible = contentPossible;
            _shopLocal = userData.Shop;
            GetItem = shopItem1;
        }

        public void Reset()
        {
            OnStartShow();
        }

        protected override void OnStartShow()
        {
            base.OnStartShow();

            var shopData = DataShop.GetData(targetKey);
            var shopDataAds = DataShop.GetData(adsTargetKey);
            var shopDataBanner = DataShop.GetData(bannerTargetKey);

            CheckExpiredEnergy(shopData);
            SetDatas(shopData, shopDataAds);

            //구매여부 설정
            var isPurchasable = shopManager.IsPurchasable(shopDataAds);
            goEnergy.SetActive(!isPurchasable);
            goEnergyAds.SetActive(isPurchasable);
            _isShowedAdsEnergy = isPurchasable;

            SetBannerItem(shopDataBanner);

            Observable.Timer(TimeSpan.FromSeconds(0.5f)).Subscribe(x =>
            {
                shopItem1.SetButtonEnable(true);
                shopItem2.SetButtonEnable(true);
                shopItemAds.SetButtonEnable(true);
                if (shopBannerItem != null)
                    shopBannerItem.SetButtonEnable(true);
            }).AddTo(this);
        }

        private void CheckExpiredEnergy(ShopBaseData shopData)
        {
            var data = _shopLocal.GetDailyItems(shopData.TabID);
            var endTime = data.End;
            if (data.CheckExpire())
            {
                endTime = GameUtils.GetFlooredGameTime_Day().AddDays(1);
                _shopLocal.ShopItems[shopData.TabID].End = endTime;
                _shopLocal.ShopItems[shopData.TabID].Items.ForEach(x =>
                {
                    if (DataShop.GetData(x.Key).SaleTypes.Contains(SaleType.IncreaseCost))
                        x.Value.ResetPurchaseCount();
                });
            }
        }

        private void SetDatas(ShopBaseData shopData, ShopBaseData shopDataAds)
        {
            shopItem1.SetView(shopData);
            shopItem1.SetButtonEnable(false);
            shopItem2.SetView(shopData);
            shopItem2.SetButtonEnable(false);
            shopItemAds.SetView(shopDataAds);
            shopItemAds.SetButtonEnable(false);
        }

        private void SetBannerItem(ShopBaseData shopDataBanner)
        {
            if (shopBannerItem != null)
            {
                var value = !_shopLocal.CheckLimitedPurchase(shopDataBanner.Key, shopDataBanner.LimitedInfo_Cooltime) &&
                            _contentPossible.PossibleShopSale();
                shopBannerItem.gameObject.SetActive(value);
                goShop.SetActive(value);
                shopBannerItem.SetView(shopDataBanner);
                shopBannerItem.SetButtonEnable(false);
            }
        }

        protected override void OnStartHide()
        {
            base.OnStartHide();
        }

        public void OnPurchaseButtonClicked()
        {
            UniTask.Void(async () =>
            {
                GetItem = shopItem1.gameObject.activeInHierarchy ? shopItem1 : shopItem2;
                GameSoundManager.Instance.PlaySfx(SFX.sh_common_click);
                var check = await shopManager.TryPurchase(shopItem1.GetShopData);
                if (check == false) return;
                shopItem1.SetButtonEnable(false);
                shopItem2.SetButtonEnable(false);
                shopItemAds.SetButtonEnable(false);
                Hide();
            });
        }

        public void OnAdWatchButtonClicked()
        {
            UniTask.Void(async () =>
            {
                GetItem = shopItemAds;
                GameSoundManager.Instance.PlaySfx(SFX.sh_common_click);
                var check = await shopManager.TryPurchase(shopItemAds.GetShopData);
                if (check == false) return;
                shopItem1.SetButtonEnable(false);
                shopItem2.SetButtonEnable(false);
                shopItemAds.SetButtonEnable(false);
                Hide();
            });
        }
    }
}