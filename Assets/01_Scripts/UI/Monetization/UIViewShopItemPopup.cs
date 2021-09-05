using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using OutGameCore;
using SH.Data;
using SH.Game.DataLocal;
using SH.Game.Manager;
using SH.Game.Shop;
using SH.Game.UserData;
using SH.UI.View;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace SH.UI.Monetization
{
    public class UIViewShopItemPopup : UIViewBase
    {
        [SerializeField] protected UIItem_Shop shopItem;
        public List<Image> image_Items = new List<Image>();
        public UIItem_Shop GetItem => shopItem;
        protected ShopManager shopManager;
        protected ShopLocal shopLocal;
        protected string targetKey;

        [Inject]
        protected virtual void Construct(ShopManager _shopManager, UserData userData)
        {
            shopManager = _shopManager;
            shopLocal = userData.Shop;
        }

        public void Reset()
        {
            OnStartShow();
        }

        protected override void OnStartShow()
        {
            base.OnStartShow();
            var shopData = DataShop.GetData(targetKey);

            var data = UserData.Instance.Shop.GetDailyItems(shopData.TabID);
            var endTime = data.End;
            shopItem.SetView(shopData);
            shopItem.SetButtonEnable(false);
            // TopUIManager.Instance.Hide(0.3f);
            Observable.Timer(TimeSpan.FromSeconds(0.5f)).Subscribe(x => shopItem.SetButtonEnable(true)).AddTo(this);
            // if (!data.CheckExpire())
            //     Observable.Timer(TimeSpan.FromSeconds(0.5f)).Subscribe(x => shopItem.SetButtonEnable(true));
            // else
            //     Observable.Timer(TimeSpan.FromSeconds(0.5f)).Subscribe(x => Hide());
            InitializeView();
        }

        protected virtual void InitializeView()
        {
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
