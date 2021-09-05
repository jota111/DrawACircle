/*
* Copyright (c) CookApps.
* 이진호(jhlee8@cookapps.com)
*/

using GameDataEditor;
using SH.AppEvent;
using SH.Constant;
using SH.Data;
using SH.Game.InGame.Msg;
using SH.Game.Shop;
using SH.Game.Tutorial;
using SH.UI;
using SH.UI.View.Dialog;
using SH.Util.UniRx;
using UniRx;

namespace SH.Game.Manager
{
    public class FundManager
    {
        public static FundManager Instance { get; private set; }
        private readonly UserData.UserData _userData;
        public IReadOnlyReactiveProperty<int> Jewel => _userData.Jewel;
        public IReadOnlyReactiveProperty<int> Gold => _userData.Gold;
        private IContentPossible contentPossible;

        public FundManager(UserData.UserData userData, IContentPossible contentPossible)
        {
            Instance = this;
            _userData = userData;
            this.contentPossible = contentPossible;
        }

        public static void DialogNotEnoughResource(string resource)
        {
            if (UIViewManager.CheckView(UIViewName.Shop) && !string.IsNullOrEmpty(resource))
                UIView_Shop.Instance.JumpToData(resource, 0.5f);
            else
                UIView_Shop.Instance.Show(false, resource);
                
            return;
        }

        public void AddJewel(int value, AppEventManager.JewelEarnedBy earnedBy = AppEventManager.JewelEarnedBy.ETC)
        {
            _userData.Jewel.Value += value;
        }

        public bool EnoughJewel(int value)
        {
            return Jewel.Value >= value;
        }

        public bool UseJewel(int value, AppEventManager.JewelSpent spent = AppEventManager.JewelSpent.Etc, params object[] args)
        {
            if (!EnoughJewel(value))
            {
                DialogNotEnoughResource("Diamonds");
                return false;
            }

            _userData.Jewel.Value -= value;
            Invalidate.Publish(Invalidate.TargetType.Jewel);

            return true;
        }

        public void AddGold(int value, AppEventManager.CoinEarnedBy earnedBy = AppEventManager.CoinEarnedBy.ETC)
        {
            _userData.Gold.Value += value;
        }

        public bool EnoughGold(int value)
        {
            return Gold.Value >= value;
        }

        public bool UseGold(int value, AppEventManager.CoinSpent coinSpent = AppEventManager.CoinSpent.ETC)
        {
            if (!EnoughGold(value))
            {
                DialogNotEnoughResource("Coins");
                return false;
            }

            _userData.Gold.Value -= value;
            Invalidate.Publish(Invalidate.TargetType.Gold);

            return true;
        }
        
        public void AddCurrency(Currencies type, int cost)
        {
            switch (type)
            {
                case Currencies.Coin:
                    AddGold(cost);
                    break;
                case Currencies.Diamond:
                    AddJewel(cost);
                    break;
                case Currencies.Energy:
                    EnergyManager.Instance.AddEnergy(cost);
                    break;
                default:
                    break;
            }
        }
    }
}