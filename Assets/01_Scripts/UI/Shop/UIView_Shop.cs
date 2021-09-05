using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using EnhancedUI.EnhancedScroller;
using Firebase.Crashlytics;
using GameDataEditor;
using OutGameCore;
using SH.AppEvent;
using SH.Data;
using SH.Game.DataLocal.Shop;
using SH.Game.Manager;
using SH.UI.EnhancedHelper;
using SH.UI.View;
using UniRx;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Zenject;

namespace SH.Game.Shop
{
    public class UIView_Shop : UIViewBase, IEnhancedScrollerDelegate
    {
        [SerializeField] private UITab_Shop cellViewPrefab;
        [SerializeField] private UITab_Shop cellViewPrefab_NoTitle;
        [SerializeField] private EnhancedScroller _scroller;
        public List<ShopBaseData> _baseDatas = new List<ShopBaseData>();

        private Dictionary<string, EnhancedScrollerCellDataSet<ShopBaseData>> dataSet =
            new Dictionary<string, EnhancedScrollerCellDataSet<ShopBaseData>>();

        private Dictionary<string, UITab_Shop> tabShops =
            new Dictionary<string, UITab_Shop>();

        public static float TabTitleSize = 100f;
        public static float RemainTimeSize = 50f;
        public static float RefreshSize = 130f;
        public static float ItemVerticalSize = 434f;

        public static float ItemPaddingSize = 20f;

        // public static float TopPaddingSize = 10f;
        public static float BottomPaddingSize = 50f;

        public static float AdditionalSize_InApp = 0f;
        public static float AdditionalSize_Limited = 0f;
        public static float AdditionalSize_LimitedSpecial = -140f;

        public static UIView_Shop Instance { get; private set; }
        private ShopManager shopManager;
        private string tabTarget;
        private ShopFrom shopFrom = ShopFrom.Lobby;
        private Tweener scrollerTweener;

        private void Awake()
        {
            _scroller.Delegate = this;
        }

        [Inject]
        private void Construct(ShopManager _shopManager)
        {
            shopManager = _shopManager;
            Instance = this;
        }

        public override void Show(bool instantAction = false, params object[] args)
        {
            if (args.Length > 0)
                tabTarget = args[0].ToString();
            else tabTarget = null;
            if (args.Length > 1)
                shopFrom = (ShopFrom)Enum.Parse(typeof(ShopFrom), args[1].ToString());
            else shopFrom = ShopFrom.Lobby;

            base.Show(instantAction);
        }

        protected override void OnStartShow()
        {
            base.OnStartShow();
            SetDatas();
            ReloadData().Forget();
        }

        protected override void OnStartHide()
        {
            base.OnStartHide();
            tabTarget = null;
            scrollerTweener?.Kill();
        }

        private void OnDestroy()
        {
            scrollerTweener?.Kill();
        }

        public void ShowTab(string tab)
        {
            tabTarget = tab;
            SetDatas();
            ReloadData().Forget();
        }

        public void ResetView()
        {
            tabTarget = null;
            SetDatas();
            ReloadData().Forget();
        }

        private void SetDatas()
        {
            _baseDatas = DataShop.GetAppearedDatas();
            dataSet.Clear();
            var tabOrder = GetTabOrder();
            foreach (var tab in tabOrder.Var_1)
            {
                dataSet.Add(tab, new EnhancedScrollerCellDataSet<ShopBaseData>(3));
            }

            foreach (var data in _baseDatas)
            {
                var checkTab = data.TabID;
                if (checkTab.Contains(ShopTabID.EventSale.ToString()))
                    checkTab = ShopTabID.EventSale.ToString();
                if (dataSet.ContainsKey(checkTab) == false)
                {
                    GameUtils.Log($"GDEShop_BaseData DB에서 Shop_Base_TabOrder 데이터에 {data.TabID}가 포함안되어있음!");
                    // dataSet.Add(data.TabID, new EnhancedScrollerCellDataSet<ShopBaseData>(3));
                    continue;
                }

                var datas = dataSet[checkTab];
                datas.SetLastOrAdd(data, data.ShopItemSize);
            }

            for (int i = dataSet.Count - 1; i >= 0; i--)
            {
                var data = dataSet.ElementAt(i);
                if (data.Value.Count == 0)
                    dataSet.Remove(data.Key);
            }

            GDEShop_BaseData GetTabOrder()
            {
                if (shopFrom == ShopFrom.InGameEvent)
                    return GDEDataManager.Get<GDEShop_BaseData>("TabOrder_Event");
                else if (shopFrom == ShopFrom.InGameFlower)
                    return GDEDataManager.Get<GDEShop_BaseData>("TabOrder_Flower");
                else return GDEDataManager.Get<GDEShop_BaseData>("TabOrder");
            }
        }

        public UIItem_Shop GetItem(string shopKey)
        {
            var data = DataShop.GetData(shopKey);
            // var obj = transform.Find($"UIItem_{data.Key}");
            // return obj.GetComponent<UIItem_Shop>();
            UIItem_Shop item = null;
            var targetTab = data.TabID;
            if (targetTab.Contains(ShopTabID.EventSale.ToString()))
                targetTab = ShopTabID.EventSale.ToString();
            if (tabShops.ContainsKey(targetTab))
                item = tabShops[targetTab].GetItem(shopKey);
            return item;
        }

        private float GetShopItemSizes(EnhancedScrollerCellRow<ShopBaseData> list)
        {
            float size = 0;
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i] != null)
                    size += list[i].ShopItemSize;
            }

            return size;
        }

        public async UniTask ReloadData()
        {
            tabShops.Clear();
            if (string.IsNullOrEmpty(tabTarget) == false)
            {
                if (_scroller.ScrollSize > 0)
                {
                    var pos = GetScrollPositionForDataIndexRate(tabTarget);
                    _scroller.ReloadData(pos);
                }
                else
                {
                    _scroller.ReloadData();
                    await UniTask.WaitUntil(() => _scroller.ScrollSize > 0 || _scroller == null);
                    JumpToData(tabTarget);
                }
            }
            else
            {
                _scroller.ReloadData();
            }
        }

        public void RefreshActiveCellViews()
        {
            _scroller.RefreshActiveCellViews();
        }

        public void JumpToData(string tabType, float time = 0f)
        {
            if (_scroller == null || string.IsNullOrEmpty(tabType)) return;
            Crashlytics.SetCustomKey("UIVIew_Shop JumpToData Flow", "0");
            var index = 0;
            if (tabType.Contains(ShopTabID.EventSale.ToString()))
                tabType = ShopTabID.EventSale.ToString();
            Crashlytics.SetCustomKey("UIVIew_Shop JumpToData Flow", "1");
            for (int i = 0; i < dataSet.Count; i++)
                if (!string.IsNullOrEmpty(dataSet.ElementAt(i).Key) && dataSet.ElementAt(i).Key.Equals(tabType))
                    index = i;
            Crashlytics.SetCustomKey("UIVIew_Shop JumpToData Flow", "2");
            if (time == 0)
            {
                _scroller.JumpToDataIndex(index);
                Crashlytics.SetCustomKey("UIVIew_Shop JumpToData Flow", "3");
            }
            else
            {
                Crashlytics.SetCustomKey("UIVIew_Shop JumpToData Flow", "4");
                var pos = GetScrollPositionForDataIndex(tabType);
                scrollerTweener?.Kill();
                scrollerTweener = DOVirtual.Float(_scroller.ScrollPosition, pos, time, x =>
                {
                    Crashlytics.SetCustomKey("UIVIew_Shop JumpToData Flow", "5");
                    if (_scroller != null)
                        _scroller.ScrollPosition = x;
                }).SetEase(Ease.OutCubic);
            }

            // _scroller.JumpToDataIndex(index, 0 , 0, true, EnhancedScroller.TweenType.easeInOutCirc, time);
        }

        private float GetScrollPositionForDataIndex(string tabType)
        {
            var index = 0;
            if (tabType.Contains(ShopTabID.EventSale.ToString()))
                tabType = ShopTabID.EventSale.ToString();
            for (int i = 0; i < dataSet.Count; i++)
                if (dataSet.ElementAt(i).Key.Equals(tabType))
                    index = i;
            var value = 0f;
            if (index != dataSet.Count - 1)
                value = _scroller.GetScrollPositionForCellViewIndex(index - 1, EnhancedScroller.CellViewPositionEnum.After);
            else
                value = _scroller.ScrollSize;
            return value;
        }

        private float GetScrollPositionForDataIndexRate(string tabType)
        {
            var index = 0;
            if (tabType.Contains(ShopTabID.EventSale.ToString()))
                tabType = ShopTabID.EventSale.ToString();
            for (int i = 0; i < dataSet.Count; i++)
                if (dataSet.ElementAt(i).Key.Equals(tabType))
                    index = i;
            var value = 0f;
            if (index != dataSet.Count - 1)
                value = _scroller.GetScrollPositionForCellViewIndex(index - 1, EnhancedScroller.CellViewPositionEnum.After);
            else
                value = _scroller.ScrollSize;
            return value / _scroller.ScrollSize;
        }

        public int GetNumberOfCells(EnhancedScroller scroller)
        {
            return dataSet.Count;
        }

        public float GetCellViewSize(EnhancedScroller scroller, int dataIndex)
        {
            var data = dataSet.ElementAt(dataIndex);
            // (아이템 크기 + 패딩값 ) Row 개수 + 타이틀 크기, Top 배치 , Botton 배치
            var additional = GetAdditionalHeight(data);
            return (ItemVerticalSize + ItemPaddingSize) * data.Value.Count + TabTitleSize + additional + BottomPaddingSize; //  + TopPaddingSize
        }

        public float GetAdditionalHeight(KeyValuePair<string, EnhancedScrollerCellDataSet<ShopBaseData>> data)
        {
            var refreshHeight = GDEDataManager.Get<GDEShop_BaseData>($"TabRefresh_{data.Key}") != null ? RefreshSize : 0;
            if (DataShop.LimitedTimeTabKeys_WithRemainTime.Contains(data.Key)) return AdditionalSize_Limited + refreshHeight;
            if (data.Key.Equals(ShopTabID.LimitedTimeSpecial.ToString())) return AdditionalSize_LimitedSpecial + refreshHeight;
            if (data.Key.Equals(ShopTabID.Energy.ToString())) return RemainTimeSize + refreshHeight;
            if (data.Value[0][0].SaleTypes.Contains(SaleType.TimeRefresh) ||
                data.Value[0][0].SaleTypes.Contains(SaleType.LimitedRepeat) ||
                data.Value[0][0].SaleTypes.Contains(SaleType.EventSale)) return RemainTimeSize + refreshHeight;
            if (data.Key.Equals(ShopTabID.Coins.ToString()) || data.Key.Equals(ShopTabID.Diamonds.ToString())) return AdditionalSize_InApp;
            return 0;
        }

        public EnhancedScrollerCellView GetCellView(EnhancedScroller scroller, int dataIndex, int cellIndex)
        {
            UITab_Shop cellView = null;
            var data = dataSet.ElementAt(dataIndex);

            if (data.Key.Equals(ShopTabID.LimitedTimeSpecial.ToString()))
                cellView = scroller.GetCellView(cellViewPrefab_NoTitle) as UITab_Shop;
            else
                cellView = scroller.GetCellView(cellViewPrefab) as UITab_Shop;

            cellView.name = "UITab_Shop_" + data.Key;
            cellView.SetView(this, data.Value);
            if (tabShops.ContainsKey(data.Key))
                tabShops[data.Key] = cellView;
            else tabShops.Add(data.Key, cellView);

            return cellView;
        }
    }
}