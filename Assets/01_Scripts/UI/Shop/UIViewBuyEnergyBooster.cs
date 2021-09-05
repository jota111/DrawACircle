using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using OutGameCore;
using MoreLinq.Extensions;
using SH.Data;
using SH.Game.Manager;
using SH.UI.View;
using UniRx;
using UnityEngine;
using Zenject;

namespace SH.Game.Shop
{
    public class UIViewBuyEnergyBooster : UIViewBase
    {
        [SerializeField] private UIItem_Shop shopItem;
        public UIItem_Shop GetItem => shopItem;
        private ShopManager shopManager;
        private const string targetKey = "EnergyBooster_0";

        [Inject]
        private void Construct(ShopManager _shopManager)
        {
            shopManager = _shopManager;
        }

        public void Reset()
        {
            OnStartShow();
        }

        protected override void OnStartShow()
        {
            base.OnStartShow();
            var shopData = DataShop.GetData(targetKey);
            
            var data = UserData.UserData.Instance.Shop.GetDailyItems(shopData.TabID);
            var endTime = data.End;
            if (data.CheckExpire())
            {
                endTime = GameUtils.GetFlooredGameTime_Day().AddDays(1);
                UserData.UserData.Instance.Shop.ShopItems[shopData.TabID].End = endTime;
                UserData.UserData.Instance.Shop.ShopItems[shopData.TabID].Items.ForEach(x =>
                {
                    if(DataShop.GetData(x.Key).SaleTypes.Contains(SaleType.IncreaseCost))
                        x.Value.ResetPurchaseCount();
                });
            }

            shopItem.SetView(shopData);
            shopItem.SetButtonEnable(false);
            Observable.Timer(TimeSpan.FromSeconds(0.5f)).Subscribe(x => shopItem.SetButtonEnable(true)).AddTo(this);
        }

        public void OnPurchaseButtonClicked()
        {
            UniTask.Void(async () =>
            {
                GameSoundManager.Instance.PlaySfx(SFX.sh_common_click);
                var check = await ShopManager.Instance.TryPurchase(shopItem.GetShopData);
                if (check == false) return;
                shopItem.SetButtonEnable(false);
                Hide();
            });
        }
    }
}
