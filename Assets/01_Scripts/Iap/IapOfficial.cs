/*
* Copyright (c) CookApps.
* 이진호(jhlee8@cookapps.com)
*/

using UniRx;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Purchasing;

namespace SH.Iap
{
    internal class IapOfficial : IIap, IStoreListener
    {
        private readonly ConfigurationBuilder builder;
        private IapSuccess iapSuccess = null;
        private IapResult iapResult = null;
        private VerifyReceiptOfficial verifyreceipt;
        private IStoreController controller = null;
        private BoolReactiveProperty isInit = new BoolReactiveProperty(false);
        private IExtensionProvider extensions = null;
        public IapType IapType() => SH.Iap.IapType.Official;

        internal IapOfficial()
        {
            var module = StandardPurchasingModule.Instance();
#if UNITY_EDITOR
            module.useFakeStoreUIMode = FakeStoreUIMode.StandardUser;
#endif
            builder = ConfigurationBuilder.Instance(module);
        }

        private void PurchaseResult(Product product, bool result)
        {
            if (product != null)
            {
                if (iapSuccess != null && result)
                {
#if UNITY_EDITOR
                    Observable.TimerFrame(10)
                        .Subscribe(_ => iapSuccess.Invoke(product.definition.id));
#else
                    iapSuccess.Invoke(product.definition.id);
#endif
                }

                if (iapResult != null)
                {
#if UNITY_EDITOR
                    Observable.TimerFrame(10)
                        .Subscribe(_ =>
                        {
                            iapResult.Invoke(result);
                            iapResult = null;
                        });
#else
                    iapResult.Invoke(result);
                    iapResult = null;
#endif
                }
            }
        }

        private void CallbackVerifyReceipt(Product product, bool result)
        {
            if (isInit.Value && result && product.definition.type == ProductType.Consumable)
            {
                controller.ConfirmPendingPurchase(product);
            }
            PurchaseResult(product, result);
        }

        private void AmazonSandboxMode()
        {
            var con = builder.Configure<IAmazonConfiguration>();
            if (con != null)
            {
                con.WriteSandboxJSON(builder.products);
            }
        }

        private Product GetProduct(string id)
        {
            if (!isInit.Value)
                return null;

            return controller.products.WithID(id);
        }

        /// <summary>
        /// 결제 정보 설정
        /// </summary>
        /// <param name="info"></param>
        public void SetInfo(IIapInfo info)
        {
            var infoOfficial = info as IapInfoOfficial;
            Assert.IsNotNull(infoOfficial);

            iapSuccess = infoOfficial.Success;
            verifyreceipt = infoOfficial.VerifyReceipt;

            foreach (var product in infoOfficial.Products)
            {
                Debug.Log("IapOfficial AddProduct : " + product.id);
                builder.AddProduct(product.id, product.type, product.ids);
            }

            if (infoOfficial.AmazonSandboxMode)
            {
                AmazonSandboxMode();
            }
        }

        /// <summary>
        /// 초기화
        /// </summary>
        public void Init()
        {
            if(!IsInit())
                UnityPurchasing.Initialize(this, builder);
        }

        /// <summary>
        /// 결제
        /// </summary>
        /// <param name="id"></param>
        /// <param name="callback"></param>
        public void Purchase(string id, IapResult callback = null)
        {
            var product = GetProduct(id);
            if (product != null)
            {
                iapResult = callback;
                controller.InitiatePurchase(product);
            }
            else if (callback != null)
            {
                Observable.NextFrame()
                    .Subscribe(_ => callback.Invoke(false));
            }
        }

        /// <summary>
        /// 초기화 되었는가?
        /// </summary>
        /// <returns></returns>
        public bool IsInit()
        {
            return isInit.Value;
        }

        public string LocalizedPrice(string id)
        {
            var product = GetProduct(id);
            return product != null ? product.metadata.localizedPriceString : string.Empty;
        }

        /*
         * IStoreListener
         * */

        public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
        {
            this.controller = controller;
            this.extensions = extensions;
            isInit.Value = true;
            Debug.Log("IapOfficial Init Success");
        }

        public void OnInitializeFailed(InitializationFailureReason error)
        {
            isInit.Value = false;
            Debug.LogWarningFormat("IapOfficial Init Failed {0}", error);
        }

        public void OnPurchaseFailed(Product i, PurchaseFailureReason p)
        {
            Debug.LogFormat("IapOfficial Purchase Failed {0}", p);
            PurchaseResult(i, false);
        }

        public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs e)
        {
            Product product = e.purchasedProduct;
            var type = product.definition.type;
            var result = type == ProductType.Consumable ? PurchaseProcessingResult.Complete : PurchaseProcessingResult.Pending;

// #if UNITY_EDITOR
//              PurchaseResult(product, true);
//              return result;
// #endif

            // 추가 검증이 필요없다
            if (verifyreceipt == null || verifyreceipt.GetInvocationList().Length == 0)
            {
                PurchaseResult(product, true);
                return result;
            }
            // 추가 검증이 필요하다(서버검증등)
            else if (verifyreceipt != null && verifyreceipt.GetInvocationList().Length > 0)
            {
                result = PurchaseProcessingResult.Pending;
                verifyreceipt.Invoke(e, product, CallbackVerifyReceipt);
            }
            return result;
        }

        public IReadOnlyReactiveProperty<string> ObsLocalizedPrice(string id)
        {
            return Observable.Create<string>(
                obs =>
                {
                    isInit.Subscribe(value =>
                    {
                        if (value) obs.OnNext(LocalizedPrice(id));
                        else obs.OnNext("Loading"); //string.Empty);
                    });
                    return Disposable.Empty;
                }
                ).DistinctUntilChanged().ToReadOnlyReactiveProperty();
        }

        public void Restore()
        {
            if (extensions != null)
            {
                if (Application.platform == RuntimePlatform.IPhonePlayer ||
                    Application.platform == RuntimePlatform.OSXPlayer)
                {
                    // 애플은 초기화 이후에 복원해야함
                    var iap = extensions.GetExtension<IAppleExtensions>();
                    if (iap != null)
                    {
                        iap.RestoreTransactions(result =>
                        {
                            Debug.Log("IapOfficial Restore " + result);
                            MessageBroker.Default.Publish(new MsgIapRestore
                            {
                                result = result
                            });
                        });
                    }
                }
            }
        }
    }
}