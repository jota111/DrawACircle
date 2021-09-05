using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using EnhancedUI.EnhancedScroller;
using GameDataEditor;
using OutGameCore;
using MoreLinq;
using SH.AppEvent;
using SH.Constant;
using SH.Data;
using SH.Game.DataLocal.Shop;
using SH.Game.Manager;
using SH.UI.EnhancedHelper;
using SH.Util;
using UniRx;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace SH.Game.Shop
{
    public class UITab_Shop : EnhancedScrollerCellView
    {
        [SerializeField] private List<Sprite> tabImages = new List<Sprite>();
        [SerializeField] private Image[] Images_Tab = new Image[2];
        [SerializeField] private Text Text_Name;
        [SerializeField] private Text Text_RemainTime;
        [SerializeField] private GameObject Obj_Title;
        [SerializeField] private GameObject Obj_RemainTime;
        [SerializeField] private Transform Obj_Parent;
        [SerializeField] private Text[] Text_Refresh = new Text[2];
        [SerializeField] private Button[] Btn_Refresh = new Button[2];
        [SerializeField] private Image[] Img_RefreshAds = new Image[2];
        [SerializeField] private Image[] Img_RefreshCurrency = new Image[2];
        [SerializeField] private Text[] Text_RefreshPrice = new Text[2];
        [SerializeField] private RectTransform Rect_BottomView;
        private UIView_Shop UIShop;
        private EnhancedScrollerCellDataSet<ShopBaseData> dataSet = new EnhancedScrollerCellDataSet<ShopBaseData>(3);
        private IDisposable tabTimer = Disposable.Empty;
        private string tabKey;
        private List<UIItemRow_Shop> _rowShops = new List<UIItemRow_Shop>();
        private float ParentObjPosY = 0;

        private GameObject Obj_RemainTemp;
        private Text Text_RemainTemp;
        private DateTime _endTime;

        void OnDisable() => tabTimer?.Dispose();

        private void OnEnable()
        {
            if (ParentObjPosY != 0) return;
            ParentObjPosY = Obj_Parent.GetComponent<RectTransform>().anchoredPosition.y;
        }

        public void Start()
        {
            Btn_Refresh[1].OnClickAsObservable().Subscribe(_ => { OnRefreshBtnClicked().Forget(); }).AddTo(this);
        }

        public void SetView(UIView_Shop _scroller, EnhancedScrollerCellDataSet<ShopBaseData> dataSet)
        {
            UIShop = _scroller;
            this.dataSet = dataSet;
            tabKey = dataSet[0][0].TabID;
            _endTime = DateTime.MinValue;

            SetTabDatas(tabKey);
            SetTabImage(tabKey);

            SetRefresh();

            DespawnAllItems();
            SpawnItems();

            SetTabSubDatas(tabKey);
            SetRemainTime(_endTime);
        }

        private void SetTabSubDatas(string tab)
        {
            Text_RemainTemp = Text_RemainTime;
            Obj_RemainTemp = Obj_RemainTime;
            if (_rowShops.Count > 0 && _rowShops[0].GetItem(0) != null)
            {
                if (tab.Equals(ShopTabID.LimitedTimeSpecial.ToString()) && _rowShops[0].GetItem(0).Text_Timer != null && _rowShops[0].GetItem(0).Obj_Timer != null)
                {
                    Text_RemainTemp = _rowShops[0].GetItem(0).Text_Timer;
                    Obj_RemainTemp = _rowShops[0].GetItem(0).Obj_Timer;
                    Obj_RemainTime.SetActive(false);   
                }else if (DataShop.LimitedTimeTabKeys_WithRemainTime.Contains(tab) && _rowShops[0].GetItem(0) is UIItem_ShopLimitedTime)
                {
                    Text_RemainTemp = (_rowShops[0].GetItem(0) as UIItem_ShopLimitedTime)?.Text_Timer;
                    Obj_RemainTemp = (_rowShops[0].GetItem(0) as UIItem_ShopLimitedTime)?.Obj_Timer;
                    Obj_RemainTime.SetActive(false);
                }
            }
        }

        private void SetTabDatas(string tab)
        {
            if (DataShop.LimitedTimeTabKeys.Contains(tab))
            {
                SetLimitedDatas();
            }
            else if (tab.Equals(ShopTabID.DailyDeals.ToString()) || tab.Equals(ShopTabID.Energy.ToString()))
            {
                var data = UserData.UserData.Instance.Shop.GetDailyItems(tab);
                var endTime = data.End;
                if (data.CheckExpire())
                {
                    endTime = GameUtils.GetFlooredGameTime_Day().AddDays(1);
                    UserData.UserData.Instance.Shop.ShopItems[tab].End = endTime;
                    UserData.UserData.Instance.Shop.ShopItems[tab].Items.ForEach(x =>
                    {
                        if (DataShop.GetData(x.Key).SaleTypes.Contains(SaleType.IncreaseCost) || tab.Equals(ShopTabID.DailyDeals.ToString()))
                            x.Value.ResetPurchaseCount();
                    });

                    SetNewItem(true);
                }
                else
                    SetNewItem(false);

                _endTime = endTime;
            }
            else
            {
                _endTime = DateTime.MinValue;
            }
        }

        public void SetRemainTimeObj(bool onOff)
        {
            if (!onOff)
                tabTimer?.Dispose();
            Obj_RemainTemp.SetActive(onOff);
            Obj_Parent.GetComponent<RectTransform>().anchoredPosition = new Vector2(Obj_Parent.GetComponent<RectTransform>().anchoredPosition.x,
                ParentObjPosY + ((onOff && Obj_RemainTemp.Equals(Obj_RemainTime)) ? 0 : 50));
        }

        private void SetTabImage(string tab)
        {
            if (tab.Equals(ShopTabID.LimitedTimeSpecial.ToString()))
            {
                Obj_Title.gameObject.SetActive(false);
                return;
            }

            Obj_Title.gameObject.SetActive(true);
            var tabNameKey = $"TabNameKey_{tab}";
            if (!tab.Contains(ShopTabID.EventSale.ToString()))
            {
                var data = GDEDataManager.Get<GDEShop_BaseData>(tabNameKey);
                if (data != null)
                {
                    var value = data.Var_0;
                    GameUtils.I2Format(Text_Name, value);
                }
                else GameUtils.Error($"Tab이름이 없음! {tab}");
            }

            if (tab.Contains(ShopTabID.EventSale.ToString()))
                tab = ShopTabID.EventSale.ToString();
            var tabDatas = GDEDataManager.Get<GDEShop_BaseData>("TabImages");
            var index = tabDatas.Var_1.IndexOf(tab);
            if (index == -1) return;
            Images_Tab[0].sprite = this.tabImages[tabDatas.Var_2[index]];
        }

        private void SetNewItem(bool onOff)
        {
            for (int i = 0; i < dataSet.Count; i++)
            {
                for (int j = 0; j < dataSet[i].Count; j++)
                {
                    if (dataSet[i][j] != null)
                        dataSet[i][j].IsNewItem = onOff;
                }
            }
        }

        #region 데이터세팅

        private void SetLimitedDatas()
        {
            var tabData = UserData.UserData.Instance.Shop.GetRepeatItems(tabKey);
            var availableData = DataShop.GetAllDatas()
                .FindAll(x => x.TabID.Equals(tabKey) && DataShop.CheckCondition(x));
            var coolTimeData = availableData.FindAll(CheckCoolTimeItem);
            availableData = availableData.OrderBy(x => x.Index).ToList();
            if (tabData.Item == null)
            {
                var index = availableData[0].Index;
                SetRepeatData(index);
                StartProgressTimer();
            }
            else
            {
                var progressInfo = tabData.GetProgressInfo();
                var currentIndex = DataShop.GetData(tabData.Item.ShopKey).Index;
                switch (progressInfo)
                {
                    case ShopItemRepeatCollection.ProgressInfo.Progress:
                        StartProgressTimer();
                        SetNewItem(false);
                        break;
                    case ShopItemRepeatCollection.ProgressInfo.Cooltime:
                        if (coolTimeData.Count > 0)
                        {
                            StartCoolTimer();
                            SetNewItem(false);
                        }
                        else UIShop.ResetView();

                        break;
                    case ShopItemRepeatCollection.ProgressInfo.Expire:
                        SetRepeatData(currentIndex + 1);
                        StartProgressTimer();
                        break;
                }
            }

            void StartProgressTimer()
            {
                var endTime = UserData.UserData.Instance.Shop.RepeatItems[tabKey].End;
                _endTime = endTime;
            }

            void StartCoolTimer()
            {
                var coolTime = UserData.UserData.Instance.Shop.RepeatItems[tabKey].CoolTime;
                _endTime = coolTime;
            }

            DateTime GetEndTime(string dataKey)
            {
                var data = DataShop.GetData(dataKey);
                var endTime = GameUtils.GetGameTime().AddMinutes(data.RepeatInfo_Duration);
                return endTime;
            }

            DateTime GetCoolTime(string dataKey, DateTime endTime)
            {
                var data = DataShop.GetData(dataKey);
                var coolTime = endTime.AddMinutes(data.RepeatInfo_Cooltime);
                return coolTime;
            }

            void SetRepeatData(int index)
            {
                var data = availableData.Find(x => x.Index == index) ?? (availableData.Find(x => x.Index > index) ?? availableData[0]);
                var newKey = data.Key;
                var endTime = GetEndTime(newKey);
                var coolTime = GetCoolTime(newKey, endTime);
                dataSet[0][0] = data;
                dataSet[0][0].IsNewItem = true;

                UserData.UserData.Instance.Shop.RepeatItems[tabKey].SetItem(newKey);
                UserData.UserData.Instance.Shop.RepeatItems[tabKey].SetEndTime(endTime);
                UserData.UserData.Instance.Shop.RepeatItems[tabKey].SetCoolTime(coolTime);
                if (coolTimeData.Count > 0)
                    UserData.UserData.Instance.Shop.RepeatItems[tabKey].SetCoolTimeItem(coolTimeData.Random().Key);
            }

            bool CheckCoolTimeItem(ShopBaseData data)
            {
                return data.RepeatInfo_OpenOnCooltime;
            }
        }

        private void SetRemainTime(DateTime endTime, bool onReload = false)
        {
            if (endTime == DateTime.MinValue)
            {
                tabTimer?.Dispose();
                SetRemainTimeObj(false);
            }
            else
            {
                SetRemainTimeObj(true);
                SetRemainTime(Text_RemainTemp, endTime, onReload);
            }
        }


        private void SetRemainTime(Text text, DateTime endTime, bool onReload = false)
        {
            tabTimer?.Dispose();
            var timeSpan = endTime.Subtract(GameUtils.GetGameTime());
            var totalMin = (int)timeSpan.TotalMinutes;
            var totalSec = (int)timeSpan.TotalSeconds;
            if (totalMin < 1)
                GameUtils.I2SetFont(text, GameUtils.GetDuration_Sec(totalSec));
            else
                GameUtils.I2SetFont(text, GameUtils.GetDuration_Min(totalMin));
            if (totalSec <= 0) return;
            tabTimer = GameUtils.createCountDownObservable(totalSec).Subscribe(x =>
            {
                var totalMin2 = x / 60;
                if (totalMin2 < 1)
                    GameUtils.I2SetFont(text, GameUtils.GetDuration_Sec(x));
                else
                    GameUtils.I2SetFont(text, GameUtils.GetDuration_Min(totalMin2));
            }, () =>
            {
                if (OutGame.Instance == null) return;
                if (onReload)
                    UIShop.ResetView();
                else
                    TimerComplete();
            }).AddTo(this);
        }

        private void TimerComplete()
        {
            SetTabDatas(tabKey);

            for (int i = 0; i < Obj_Parent.childCount; i++)
            {
                Obj_Parent.GetChild(i).GetComponent<UIItemRow_Shop>().SetView(dataSet[i]);
            }
        }

        #endregion

        #region 아이템

        private void DespawnAllItems()
        {
            for (int i = Obj_Parent.childCount - 1; i >= 0; i--)
            {
                if (Obj_Parent.GetChild(i) != null)
                {
                    if (OutGame.Instance.gamePools.IsSpawned(Obj_Parent.GetChild(i)))
                        OutGame.Instance.gamePools.Despawn(Obj_Parent.GetChild(i));
                    else
                        Destroy(Obj_Parent.GetChild(i).gameObject);
                }
            }

            _rowShops.Clear();
        }

        private void SpawnItems()
        {
            var height = dataSet.Count * (UIView_Shop.ItemVerticalSize + UIView_Shop.ItemPaddingSize) - UIView_Shop.ItemPaddingSize
                         + (DataShop.LimitedTimeTabKeys_WithRemainTime.Contains(tabKey) ? UIView_Shop.AdditionalSize_Limited : 0)
                         + (tabKey.Equals(ShopTabID.LimitedTimeSpecial.ToString()) ? UIView_Shop.AdditionalSize_Limited : 0);
            Obj_Parent.GetComponent<RectTransform>().sizeDelta = new Vector2(Obj_Parent.GetComponent<RectTransform>().sizeDelta.x, height);

            for (int i = 0; i < dataSet.Count; i++)
            {
                var objName = "UIItemRow_Shop";
                if (tabKey.Equals(ShopTabID.DailyDeals.ToString()))
                    objName = "UIItemRow_Shop2";
                var item = OutGame.Instance.gamePools.Spawn<UIItemRow_Shop>(objName, true, Obj_Parent);
                item.SetView(dataSet[i]);
                _rowShops.Add(item);
            }
        }

        public UIItem_Shop GetItem(string key)
        {
            foreach (var rowShop in _rowShops)
            {
                var item = rowShop.GetItem(key);
                if (item != null) return item;
            }

            return null;
        }

        private void SetRefresh()
        {
            var data = GDEDataManager.Get<GDEShop_BaseData>($"TabRefresh_{tabKey}");
            if (data == null)
            {
                Btn_Refresh[0].gameObject.SetActive(false);
                Btn_Refresh[1].gameObject.SetActive(false);
                return;
            }

            Btn_Refresh[0].gameObject.SetActive(!CheckCanRefresh());
            Btn_Refresh[1].gameObject.SetActive(CheckCanRefresh());
            var maxData = GDEDataManager.Get<GDEShop_BaseData>($"TabRefreshCount_{tabKey}");
            if (maxData == null) return;
            var current = UserData.UserData.Instance.Shop.GetRefreshCount(tabKey);
            Text_Refresh[0].text = $"{maxData.Var_3 - current}/{maxData.Var_3}";
            Text_Refresh[1].text = $"{maxData.Var_3 - current}/{maxData.Var_3}";

            var purchaseType = data.Var_0.ToEnumString<PurchaseType>();
            switch (purchaseType)
            {
                case PurchaseType.AD:
                    Img_RefreshAds[0].gameObject.SetActive(true);
                    Img_RefreshAds[1].gameObject.SetActive(true);
                    Img_RefreshCurrency[0].gameObject.SetActive(false);
                    Img_RefreshCurrency[1].gameObject.SetActive(false);
                    Text_RefreshPrice[0].gameObject.SetActive(false);
                    Text_RefreshPrice[1].gameObject.SetActive(false);
                    break;
                case PurchaseType.Coin:
                    Img_RefreshAds[0].gameObject.SetActive(false);
                    Img_RefreshAds[1].gameObject.SetActive(false);
                    Img_RefreshCurrency[0].gameObject.SetActive(true);
                    Img_RefreshCurrency[1].gameObject.SetActive(true);
                    Text_RefreshPrice[0].gameObject.SetActive(true);
                    Text_RefreshPrice[1].gameObject.SetActive(true);
                    Text_RefreshPrice[0].text = data.Var_3.ToString();
                    Text_RefreshPrice[1].text = data.Var_3.ToString();
                    break;
                case PurchaseType.Diamond:
                    Img_RefreshAds[0].gameObject.SetActive(false);
                    Img_RefreshAds[1].gameObject.SetActive(false);
                    Img_RefreshCurrency[0].gameObject.SetActive(true);
                    Img_RefreshCurrency[1].gameObject.SetActive(true);
                    Text_RefreshPrice[0].gameObject.SetActive(true);
                    Text_RefreshPrice[1].gameObject.SetActive(true);
                    Text_RefreshPrice[0].text = data.Var_3.ToString();
                    Text_RefreshPrice[1].text = data.Var_3.ToString();
                    break;
            }
        }

        public bool CheckCanRefresh()
        {
            var maxData = GDEDataManager.Get<GDEShop_BaseData>($"TabRefreshCount_{tabKey}");
            if (maxData == null) return false;
            var current = UserData.UserData.Instance.Shop.GetRefreshCount(tabKey);
            return current < maxData.Var_3;
        }

        #endregion

        public override void RefreshCellView()
        {
            SetTabDatas(tabKey);

            for (int i = 0; i < Obj_Parent.childCount; i++)
            {
                Obj_Parent.GetChild(i).GetComponent<UIItemRow_Shop>().SetView(dataSet[i]);
            }
        }

        public async UniTask OnRefreshBtnClicked()
        {
            var data = GDEDataManager.Get<GDEShop_BaseData>($"TabRefresh_{tabKey}");
            if (data == null) return;
            if (!CheckCanRefresh()) return;
            var purchaseType = data.Var_0.ToEnumString<PurchaseType>();
            //구매시도
            var check = await ShopManager.Instance.CheckCanPurchase(purchaseType, data.Var_3);
            if (!check) return;
            //재화 감소
            ShopManager.Instance.DeductCost(purchaseType, data.Var_3, AppEventManager.JewelSpent.ShopRefresh);

            UserData.UserData.Instance.Shop.AddRefreshCount(tabKey, 1);
            SetRefresh();

            SetTabDatas(tabKey);

            DespawnAllItems();
            SpawnItems();
        }
    }
}