using System;
using System.Collections;
using System.Collections.Generic;
using Doozy.Engine.UI;
using OutGameCore;
using SH.Constant;
using SH.Data;
using SH.Game.Manager;
using SH.Setting;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace SH.Game.Shop
{
    public class UIItemInfo_Shop : MonoBehaviour
    {
        [SerializeField] private Button Btn_Image;
        [SerializeField] private Button Btn_Information;
        [SerializeField] private Image Img_ItemIcon;
        [SerializeField] private Image Img_EnergyIcon;
        [SerializeField] private GameObject BG_ItemCount;
        [SerializeField] private Text Text_ItemCount;
        [SerializeField] private Image Img_ItemCount;
        private bool availDetailView;
        [SerializeField] private bool updated = true;
        private Vector2 defaultSize;
        
        [SerializeField] private Sprite CoinSprite;
        [SerializeField] private Sprite DiamondSprite;
        [SerializeField] public bool showCount = false;
        private float posY_ItemIcon;

        public void Start()
        {
            if (Btn_Image != null)
                Btn_Image.OnClickAsObservable()
                    .Subscribe(_ => OnInfoButtonClicked()).AddTo(this);
            if (Btn_Information != null)
                Btn_Information.OnClickAsObservable()
                    .Subscribe(_ => OnInfoButtonClicked()).AddTo(this);
        }

        public Vector3 GetIconPos => Img_ItemIcon.transform.position;

        public void SetShopInfo(string iconKey, int count)
        {
            if (Img_ItemIcon != null)
            {
                if(defaultSize == Vector2.zero)
                    defaultSize = Img_ItemIcon.GetComponent<RectTransform>().sizeDelta;
                if(posY_ItemIcon == 0)
                    posY_ItemIcon = Img_ItemIcon.GetComponent<RectTransform>().anchoredPosition.y;
            }
            if (!updated) return;
            availDetailView = false;
            SetItemCount(count);
            if (Img_ItemIcon != null)
            {
                Img_ItemIcon.sprite = AtlasSetting.Instance.GetShopIcon(iconKey);
                Img_ItemIcon.GetComponent<RectTransform>().sizeDelta = defaultSize;
                Img_ItemIcon.gameObject.SetActive(true);
                if(Img_EnergyIcon != null)
                    Img_EnergyIcon.gameObject.SetActive(false);
            }
            if(iconKey.Contains("Diamonds_tier"))
                SetItemCountImage(DiamondSprite);
            else if(iconKey.Contains("Coins_Tier"))
                SetItemCountImage(CoinSprite);
            else
                SetItemCountImage();
        }

        public void SetItemInfo(string key, int count)
        {
            if (!updated) return;
            if (Img_ItemIcon != null)
            {
                if(defaultSize == Vector2.zero)
                    defaultSize = Img_ItemIcon.GetComponent<RectTransform>().sizeDelta;
                if(posY_ItemIcon == 0)
                    posY_ItemIcon = Img_ItemIcon.GetComponent<RectTransform>().anchoredPosition.y;
                Img_ItemIcon.gameObject.SetActive(true);
            }
            availDetailView = false;
            if(Img_EnergyIcon != null)
                Img_EnergyIcon.gameObject.SetActive(false);
            SetItemCount(count, (DataCurrencies.CheckCurrency(key)?"+":"x"));
            try
            {
                if (Btn_Information != null)
                    Btn_Information.gameObject.SetActive(false);
                if (Btn_Image != null)
                    Btn_Image.enabled = false;
                if (key.Equals(Currencies.Diamond.ToString()))
                {
                    Img_ItemIcon.sprite = DataCurrencies.GetDiamondShopIcon(count);//AtlasSetting.Instance.GetItemIcon(ItemType.Jewel_04);
                    Img_ItemIcon.GetComponent<RectTransform>().sizeDelta = defaultSize;
                    SetItemCountImage();
                }
                else if (key.Equals(Currencies.Coin.ToString()))
                {
                    Img_ItemIcon.sprite = DataCurrencies.GetCoinShopIcon(count);//AtlasSetting.Instance.GetItemIcon(ItemType.Gold_06);
                    Img_ItemIcon.GetComponent<RectTransform>().sizeDelta = defaultSize;
                    SetItemCountImage();
                }
                else if (key.Equals(Currencies.Energy.ToString()))
                {
                    // Img_ItemIcon.sprite = AtlasSetting.Instance.GetItemIcon(ItemType.Energy_05);
                    // Img_ItemIcon.SetNativeSize();
                    if(Img_ItemIcon != null)
                        Img_ItemIcon.gameObject.SetActive(false);
                    if(Img_EnergyIcon != null)
                        Img_EnergyIcon.gameObject.SetActive(true);
                    SetItemCountImage();
                }
            }
            catch (Exception ex)
            {
                GameUtils.LogException(ex);
            }
        }

        public void SetItemCountText(Text text)
        {
            Text_ItemCount = text;
        }

        public void SetItemCount(int count, string value = "")
        {
            if (!updated) return;
            if (Text_ItemCount == null) return;
            Text_ItemCount.gameObject.SetActive(count != 1 || showCount);
            Text_ItemCount.text = $"{value}{count}";
            if(BG_ItemCount != null) BG_ItemCount.SetActive(count != 1);
        }

        public void SetIconPosY(float value)
        {
            if (Img_ItemIcon == null) return;
            Img_ItemIcon.GetComponent<RectTransform>().anchoredPosition = new Vector2(
                Img_ItemIcon.GetComponent<RectTransform>().anchoredPosition.x,
                posY_ItemIcon + value);
        }

        public void SetItemCountImage(Sprite sprite = null)
        {
            if (Img_ItemCount == null) return;
            if (sprite == null) Img_ItemCount.gameObject.SetActive(false);
            else
            {
                Img_ItemCount.gameObject.SetActive(true);
                Img_ItemCount.sprite = sprite;
            }
        }

        public void OnInfoButtonClicked()
        {
            if (!updated) return;
            if (availDetailView)
            {
                GameSoundManager.Instance.PlaySfx(SFX.sh_common_click);
                // _uiViewItemInfo.Show(type);
            }
        }
    }
}