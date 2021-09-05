using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Doozy.Engine.UI;
using Firebase.Crashlytics;
using OutGameCore;
using SH.AppEvent;
using SH.Constant;
using SH.Data;
using SH.Game.DataLocal.Shop;
using SH.Game.Manager;
using UniRx;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Zenject;

namespace SH.Game.Shop
{
    public class UIItem_Shop : MonoBehaviour
    {
        [SerializeField] private Button Btn_Purchase;
        [SerializeField] private Image Img_Button;

        [SerializeField] private Image Img_Price;
        [SerializeField] private Text Text_Price;
        [SerializeField] private Text Text_PriceDisabled;
        [SerializeField] private Text Text_PriceSaled;
        [SerializeField] private Text Text_ShopName;
        [SerializeField] private Text Text_LeftCount;

        [SerializeField] private Sprite CoinSprite;
        [SerializeField] private Sprite DiamondSprite;

        [SerializeField] private Sprite BtnEnabledSprite;
        [SerializeField] private Sprite BtnDisabledSprite;

        [SerializeField] protected UIItemInfo_Shop[] ItemInfos = new UIItemInfo_Shop[3];
        [SerializeField] public UIItemInfo_Shop[] GetItemInfos => ItemInfos;
        [SerializeField] private GameObject Obj_RedDot;
        [SerializeField] private bool SetButtonsOnStart = true;
        
        [SerializeField] public GameObject Obj_Timer;
        [SerializeField] public Text Text_Timer;

        private ShopBaseData shopData;
        public ShopBaseData GetShopData => shopData;

        public void Start()
        {
            if (SetButtonsOnStart)
                SetButtons();
        }

        public virtual void SetView(ShopBaseData data)
        {
            shopData = data;
            transform.name = $"UIItem_{data.Key}";
            if (Img_Button != null)
                Img_Button.sprite = BtnEnabledSprite;
            SetPriceText(true);
            if (Obj_RedDot != null)
                Obj_RedDot.SetActive(data.IsNewItem || (data.PurchaseType == PurchaseType.Free && data.PurchaseCount > 0 &&
                                                        ShopManager.Instance.GetPurchasableCount(data.Key) > 0));
            SetText(data);
            SetItemInfos(data);
            SetPurchaseType(data);
        }

        private void SetText(ShopBaseData data)
        {
            if (Text_LeftCount != null)
                Text_LeftCount.gameObject.SetActive(data.PurchaseCount > 0);
            if (data.PurchaseCount > 0)
            {
                var left = ShopManager.Instance.GetPurchasableCount(data.Key);
                if (left <= 0)
                {
                    if (Img_Button != null)
                        Img_Button.sprite = BtnDisabledSprite;
                    SetPriceText(false);
                    if (Text_LeftCount != null)
                        GameUtils.I2Format(Text_LeftCount, "UIShop_OutOfStock");
                }
                else if (Text_LeftCount != null)
                {
                    GameUtils.I2Format(Text_LeftCount, "UIShop_ItemLeft", left);
                }
            }
            else if (string.IsNullOrEmpty(data.NameKey) == false)
            {
                if (Text_ShopName != null)
                {
                    Text_ShopName.gameObject.SetActive(true);
                    GameUtils.I2Format(Text_ShopName, data.NameKey);
                }
            }
            else if (Text_ShopName != null) Text_ShopName.gameObject.SetActive(false);
        }

        private void SetPurchaseType(ShopBaseData data)
        {
            switch (data.PurchaseType)
            {
                case PurchaseType.Coin:
                    if (Img_Price != null)
                    {
                        Img_Price.gameObject.SetActive(true);
                        Img_Price.sprite = CoinSprite;
                    }

                    SetCost();
                    break;
                case PurchaseType.Diamond:
                    if (Img_Price != null)
                    {
                        Img_Price.gameObject.SetActive(true);
                        Img_Price.sprite = DiamondSprite;
                    }

                    SetCost();
                    break;
                case PurchaseType.Free:
                    if (Img_Price != null)
                        Img_Price.gameObject.SetActive(false);
                    if (Text_Price != null)
                        GameUtils.I2Format(Text_Price, "Text_Free");
                    if (Text_PriceDisabled != null)
                        GameUtils.I2Format(Text_PriceDisabled, "Text_Free");
                    break;
                case PurchaseType.InApp:
                    if (Img_Price != null)
                        Img_Price.gameObject.SetActive(false);
                    if (Text_Price != null)
                        IapManager.Instance.ObsLocalizedPrice(shopData.InAppKey)
                            .TakeUntilDisable(Text_Price)
                            .SubscribeToText(Text_Price);
                    if (Text_PriceDisabled != null)
                        IapManager.Instance.ObsLocalizedPrice(shopData.InAppKey)
                            .TakeUntilDisable(Text_PriceDisabled)
                            .SubscribeToText(Text_PriceDisabled);
                    if (Text_PriceSaled != null && !string.IsNullOrEmpty(shopData.SalePriceKey))
                        IapManager.Instance.ObsLocalizedPrice(shopData.SalePriceKey)
                            .TakeUntilDisable(Text_PriceSaled)
                            .SubscribeToText(Text_PriceSaled);
                    break;
            }
        }

        private void SetPriceText(bool onOff)
        {
            if (Text_PriceDisabled != null)
                Text_PriceDisabled.gameObject.SetActive(!onOff);
            if (Text_Price != null)
                Text_Price.gameObject.SetActive(onOff);
            if(Text_PriceSaled != null)
                Text_PriceSaled.gameObject.SetActive(onOff);
        }

        private void SetItemInfos(ShopBaseData data)
        {
            try
            {
                for (int i = 0; i < ItemInfos.Length; i++)
                {
                    if (ItemInfos[i] == null) continue;
                    if (i < data.ItemCount.Count)
                    {
                        if (string.IsNullOrEmpty(data.ItemIcon) || i > 0)
                        {
                            if (string.IsNullOrEmpty(data.ItemKey[i]))
                            {
                                GameUtils.Error($"ShopKey Null {data.Key}");
                            }
                            else
                            {
                                ItemInfos[i].SetItemInfo(data.ItemKey[i], data.ItemCount[i]);
                                ItemInfos[i].SetIconPosY((Text_LeftCount == null || !Text_LeftCount.gameObject.activeSelf) && data.ItemCount[i] == 1 && !ItemInfos[i].showCount ? 33 : 0);
                            }
                        }
                        else
                        {
                            ItemInfos[i].SetShopInfo(data.ItemIcon, data.ItemCount[i]);
                            ItemInfos[i].SetIconPosY((Text_LeftCount == null || !Text_LeftCount.gameObject.activeSelf) && data.ItemCount[i] == 1 && !ItemInfos[i].showCount ? 33 : 0);
                        }

                        ItemInfos[i].gameObject.SetActive(true);
                    }
                    else
                    {
                        ItemInfos[i]?.gameObject.SetActive(false);
                    }
                }
            }
            catch (Exception ex)
            {
                GameUtils.LogException(ex);
            }
        }

        private void SetCost()
        {
            var cost = ShopManager.Instance.GetCost(shopData);
            if (Text_Price != null)
                Text_Price.text = cost.ToString("N0");
            if (Text_PriceDisabled != null)
                Text_PriceDisabled.text = cost.ToString("N0");
        }

        private void SetButtons()
        {
            Crashlytics.SetCustomKey("UIItem_Shop SetButtons Flow", "0");
            if (Btn_Purchase == null) return;
            Crashlytics.SetCustomKey("UIItem_Shop SetButtons Flow", "1");
            Btn_Purchase.OnClickAsObservable()
                .Subscribe(_ =>
                {
                    if (shopData == null) return;
                    Crashlytics.SetCustomKey("UIItem_Shop SetButtons Flow", "2");
                    var shopDetailPopup = DataShop.GetShopDetailPopup(shopData.Key);
                    Crashlytics.SetCustomKey("UIItem_Shop SetButtons Flow", "3");
                    if (string.IsNullOrEmpty(shopDetailPopup))
                        OnPurchaseButtonClicked();
                    else UIView.ShowView(shopDetailPopup);
                    Crashlytics.SetCustomKey("UIItem_Shop SetButtons Flow", $"{shopDetailPopup}");
                }).AddTo(this);
        }

        public void SetButtonEnable(bool onOff)
        {
            Btn_Purchase.enabled = onOff;
        }

        public void OnPurchaseButtonClicked()
        {
            UniTask.Void(async () =>
            {
                GameSoundManager.Instance.PlaySfx(SFX.sh_common_click);
                var check = await ShopManager.Instance.TryPurchase(shopData);
                if (check == false) return;
            });
        }

        public void ResetViewAfterPurchase()
        {
            SetView(shopData);
        }
    }
}