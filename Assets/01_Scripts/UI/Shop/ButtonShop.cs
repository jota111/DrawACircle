using System;
using Doozy.Engine.UI;
using OutGameCore;
using SH.AppEvent;
using SH.Constant;
using SH.Data;
using SH.Game.DataLocal;
using SH.Game.InGame.Msg;
using SH.Game.Manager;
using SH.Game.Tutorial;
using SH.UI;
using SH.Util;
using SH.Util.UniRx;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace SH.Game.Shop
{
    public class ButtonShop : MonoBehaviour
    {
        [SerializeField] private GameObject Obj_RedDot;
        [SerializeField] private string TabType;
        private ShopLocal _shopLocal;
        private ShopManager _shopManager;
        private SceneDisposable _disposable;
        private IDisposable checkTimer;
        private bool canClick = false;

        [Inject]
        public void Construct(IContentPossible contentPossible, UserData.UserData userData, ShopManager shopManager, SceneDisposable disposable)
        {
            GetComponent<Button>()
                .OnClickAsObservable()
                .Subscribe(_ =>
                {
                    if (canClick == false) return;
                    if (UIViewManager.CheckView(UIViewName.Shop))
                    {
                        #region AppEvent

                        if (Obj_RedDot == null)
                            SendShopAppEvent(TabType);

                        #endregion

                        UIView_Shop.Instance.JumpToData(TabType, 0.5f);
                        return;
                    }

                    UIViewManager.Instance.HideAll();
                    GameSoundManager.Instance.PlaySfx(SFX.sh_common_click);
                    if (Obj_RedDot == null)
                    {
                        #region AppEvent

                        SendShopAppEvent(TabType);

                        #endregion

                        UIView_Shop.Instance.Show(false, TabType, GetShopFrom());
                    }
                    else
                    {
                        UIView_Shop.Instance.Show(false, TabType, GetShopFrom());
                    }
                })
                .AddTo(this);

            if (Obj_RedDot != null)
            {
                contentPossible.ObsPossibleShopSale()
                    .Subscribe(x =>
                    {
                        gameObject.SetActive(x);
                        canClick = x;
                    })
                    .AddTo(this);

                _shopLocal = userData.Shop;
                _shopManager = shopManager;
                _disposable = disposable;
                if (ShopManager.Instance.GetPurchasableCount("DailyDeals_0") > 0
                    || (_shopLocal.RedDotTime != DateTime.MaxValue && _shopLocal.RedDotTime < GameUtils.GetGameTime()))
                    Obj_RedDot.SetActive(true);
                else
                    CheckRedDot();
                MessageBroker.Default.Receive<UIViewPop>()
                    .Where(x => x.UIView.ViewName.Equals("Shop"))
                    .Subscribe(y => CheckRedDot())
                    .AddTo(disposable);
            }
            else
            {
                contentPossible.ObsPossibleShopSale()
                    .Subscribe(x =>
                    {
                        GetComponent<Button>().enabled = x;
                        canClick = x;
                    })
                    .AddTo(this);
            }

            ShopFrom GetShopFrom()
            {
                return ShopFrom.Lobby;
            }
        }

        private void SendShopAppEvent(string _TabType)
        {
        }

        /// <summary>
        /// 무료상품 구매가능할때
        /// 신규 상품들 나왔을때
        /// 1. 패키지 상품 오픈
        /// 2. FlashSale 변경
        /// </summary>
        public void CheckRedDot()
        {
            checkTimer?.Dispose();
            bool onOff = false;

            var latestTime = DateTime.MaxValue;
            latestTime = GetLatestUpdateTime(latestTime);
            _shopLocal.RedDotTime = latestTime;

            if (latestTime != DateTime.MaxValue)
            {
                // 무료 상품 구매 가능할때
                bool checkPurchasable = ShopManager.Instance.GetPurchasableCount("DailyDeals_0") > 0;
                bool checkPast = GameUtils.GetGameTime() > latestTime;
                onOff = checkPurchasable || checkPast;
                if (!onOff)
                {
                    var time = latestTime - GameUtils.GetGameTime();
                    checkTimer = Observable.Timer(time).Subscribe(x => CheckRedDot()).AddTo(_disposable);
                }
            }
            else
            {
                onOff = true;
            }

            Obj_RedDot.SetActive(onOff);
        }

        private DateTime GetLatestUpdateTime(DateTime latestTime)
        {
            // 패키지 상품 오픈
            foreach (var repeat in _shopLocal.RepeatItems)
            {
                if (repeat.Value.CoolTime < latestTime)
                    latestTime = repeat.Value.CoolTime;
            }

            // FlashSale
            foreach (var board in _shopLocal.Board)
            {
                if (board.Value.End < latestTime)
                    latestTime = board.Value.End;
            }

            // DailyDeals
            if (_shopLocal.ShopItems.ContainsKey("DailyDeals"))
            {
                if (_shopLocal.ShopItems["DailyDeals"].End < latestTime)
                    latestTime = _shopLocal.ShopItems["DailyDeals"].End;
            }

            return latestTime;
        }
    }
}