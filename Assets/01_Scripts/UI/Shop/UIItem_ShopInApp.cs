using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Doozy.Engine.UI;
using GameDataEditor;
using OutGameCore;
using SH.Constant;
using SH.Data;
using SH.Game.Manager;
using SH.Ob;
using UniRx;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Zenject;

namespace SH.Game.Shop
{
    public class UIItem_ShopInApp : UIItem_Shop
    {
        [SerializeField] private Text Text_CoinText;
        [SerializeField] private Text Text_JewelText;
        [SerializeField] private GameObject Obj_Best;
        [SerializeField] private GameObject Obj_Most;
        [SerializeField] private GameObject Obj_Energy;
        
        public override void SetView(ShopBaseData data)
        {
            var index = data.ItemKey.IndexOf("Energy");
            if (index != -1)
                Obj_Energy.SetActive(true);
            if (data.ItemKey.Contains("Coin"))
            {
                Text_JewelText.gameObject.SetActive(false);
                Text_CoinText.gameObject.SetActive(true);
                ItemInfos[0].SetItemCountText(Text_CoinText);
            }else if (data.ItemKey.Contains("Diamond"))
            {
                Text_CoinText.gameObject.SetActive(false);
                Text_JewelText.gameObject.SetActive(true);
                ItemInfos[0].SetItemCountText(Text_JewelText);
            }

            var bestData = GDEDataManager.Get<GDEShop_BaseData>("InApp_BestProduct");
            var mostData = GDEDataManager.Get<GDEShop_BaseData>("InApp_MostProduct");
            if (!string.IsNullOrEmpty(CountryCode.GetCode))
            {
                if (GDEDataManager.TryGet<GDEShop_BaseData>($"InApp_BestProduct_{CountryCode.GetCode}", out var tryBestData))
                    bestData = tryBestData;
                if (GDEDataManager.TryGet<GDEShop_BaseData>($"InApp_MostProduct_{CountryCode.GetCode}", out var tryMostData))
                    mostData = tryMostData;
            }
            Obj_Best.SetActive(bestData.Var_1.Contains(data.Key));
            Obj_Most.SetActive(mostData.Var_1.Contains(data.Key));

            base.SetView(data);
        }
    }
}
