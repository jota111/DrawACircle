/*
* Copyright (c) CookApps.
* 이진호(jhlee8@cookapps.com)
*/

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Cysharp.Threading.Tasks;
using GameDataEditor;
using SH.Data;
using SH.Game.Misc;
using SH.Game.Shop;
using SH.Iap;
using SH.Net;
using SH.Net.Pkt;
using UniRx;
using UnityEngine.Purchasing;
using Zenject;

namespace SH.Game.Manager
{
    public class IapManager : IInitializable
    {
        public static IapManager Instance { get; private set; }
        private readonly IapModuleManager _iapModuleManager;
        private readonly LazyInject<ShopManager> _shopManager;
        private readonly IIndicator _indicator;

        public IapManager(IapModuleManager iapModuleManager, LazyInject<ShopManager> shopManager, IIndicator indicator)
        {
            _iapModuleManager = iapModuleManager;
            _shopManager = shopManager;
            _indicator = indicator;
            Instance = this;
        }

        public void Initialize()
        {
            SetInfoOfficial();
        }

        private void SetInfoOfficial()
        {
            var info = new IapInfoOfficial();
            
#if UNITY_AWS && __DEV
            info.AmazonSandboxMode = true;
#endif

            // 성공시 콜백
            info.Success = CallbackSuccess;
            // 검증
            info.VerifyReceipt = VerifyReceiptServer;
            // 상품 구성
            info.Products = GDEDataManager.GetAllItems<GDEShop_InAppData>()
                .Select(value => value.InAppKey)
                .Select(value => new IapInfoOfficial.Product
                {
                    id = value,
                    type = ProductType.Consumable,
                    ids = new IDs()
                    {
                        {value, GooglePlay.Name},
                        {value, AppleAppStore.Name}
                    }
                });
                
            _iapModuleManager.SetInfo(info);
            _iapModuleManager.Init();
        }

        private void CallbackSuccess(string productId)
        {
            Debug.Log($"Iap Purchase Success : {productId}");
            _shopManager.Value.PurchaseSucceedProductId(productId);
        }

        /// <summary>
        /// 영수증 검증
        /// </summary>
        /// <param name="args"></param>
        /// <param name="product"></param>
        /// <param name="result"></param>
        private async void VerifyReceiptServer(PurchaseEventArgs args, Product product, VerifyReceiptResultOfficial result)
        {
#if UNITY_EDITOR
            await UniTask.Delay(TimeSpan.FromSeconds(1));
            result.Invoke(product, true);
#else
            var data = DataShop.GetDataByProductId(product.definition.id);
            var receiptInfo = IapHelper.GetReceiptInfo(product.receipt);
            var req = new ReqIapVerifyReceipt
            {
                purchase_type = data.ServerPurchaseType,
                productID = product.definition.id,
                purchase_data = receiptInfo.purchase,
                data_signature = receiptInfo.signature,
                currency_code = "USD",
                currency_price = data.InAppUSD.ToString(CultureInfo.InvariantCulture),
            };

            var recv = await NetManager.Post<Recv>(req);
            result.Invoke(product, recv.result);

            if (recv.result)
            {
                // var param = new Dictionary<string, object>();
                // param.Add("af_currency", "USD");
                // param.Add("af_revenue", data.InAppUSD.ToString(CultureInfo.InvariantCulture));
                // param.Add("af_quantity", "1");
                // param.Add("af_content_id", data.Key);
                SingularSDK.Revenue(product.metadata.isoCurrencyCode, 1, "sku", data.Key, "category", 1, (double) product.metadata.localizedPrice);
            }
#endif
        }
        
        /// <summary>
        /// 결제
        /// </summary>
        /// <param name="id"></param>
        /// <param name="callback">결제 성공여부(상품 지급 여기서 하면 안됨)</param>
        // public void Purchase(string id, IapResult callback = null)
        // {
        //     _iapModuleManager.Purchase(id, callback);
        // }

        /// <summary>
        /// 결제
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async UniTask<bool> AsyncPurchase(string id)
        {
            using var indicator = _indicator.ScopeLock();
            bool? result = null;
            //Purchase(id, OnCallback);
            _iapModuleManager.Purchase(id, OnCallback);
            await UniTask.WaitUntil(() => result.HasValue);
            return result.Value;
            
            //---------------------------------------
            void OnCallback(bool b)
            {
                result = b;
            }
        }

        /// <summary>
        /// 현지화 가격 얻기
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public string LocalizedPrice(string id)
        {
            return _iapModuleManager.LocalizedPrice(id);
        }

        /// <summary>
        /// rx 현지화 가격 얻기
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IReadOnlyReactiveProperty<string> ObsLocalizedPrice(string id)
        {
            return _iapModuleManager.ObsLocalizedPrice(id);
        }

        /// <summary>
        /// 결제 복원(IPhonePlayer, OSXPlayer)
        /// </summary>
        public void Restore()
        {
            _iapModuleManager?.Restore();
        }
    }
}