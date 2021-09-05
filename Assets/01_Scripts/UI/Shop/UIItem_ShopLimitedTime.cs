using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Doozy.Engine.UI;
using GameDataEditor;
using OutGameCore;
using SH.Constant;
using SH.Data;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Zenject;

namespace SH.Game.Shop
{
    public class UIItem_ShopLimitedTime : UIItem_Shop
    {
        [SerializeField] private Image BG_Item;
        [SerializeField] private Sprite[] BGSprite = new Sprite[4];
        [SerializeField] private GameObject Go_SaleTag;
        [SerializeField] private Text Text_Sale;
        [SerializeField] private Text Text_SaleDescription;

        public override void SetView(ShopBaseData data)
        {
            SetBG(data);
            SetSaleTag(data);
            base.SetView(data);
        }

        private void SetBG(ShopBaseData data)
        {
            if (BG_Item == null) return;
            var index = data.Index % BGSprite.Length;
            BG_Item.sprite = BGSprite[index];
        }

        private void SetSaleTag(ShopBaseData data)
        {
            if (Go_SaleTag == null) return;
            Go_SaleTag.SetActive(data.DiscountedRate > 0);
            if (data.DiscountedRate > 100)
            {
                Text_Sale.text = $"+{data.DiscountedRate}%";
                Text_SaleDescription.text = "More";
            }
            else
            {
                Text_Sale.text = $"{data.DiscountedRate}%";
                Text_SaleDescription.text = "Sale";
            }
        }
    }
}
