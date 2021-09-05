using System;
using System.Collections.Generic;
using OutGameCore;
using SH.Game.DataLocal;
using SH.Game.Manager;
using SH.Game.UserData;
using Sirenix.OdinInspector;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace SH.UI.Fund
{
    public class UIEnergyBooster : MonoBehaviour
    {
        [SerializeField, Required] private Text Text_EnergyBooster;
        [SerializeField, Required] private GameObject Icon_Energy;
        [SerializeField, Required] private GameObject FX_EnergyBooster;
        [SerializeField, Required] private GameObject Obj_EnergyBoosterTimer;
        private UserData _userData;
        private ShopLocal _shopLocal;
        private EnergyManager _energyManager;
        
        [Inject]
        private void Construct(EnergyManager energyManager, UserData userData)
        {
            _energyManager = energyManager;
            _userData = userData;
            _shopLocal = userData.Shop;
            _userData.EnergyBoosterEndTime.Subscribe(_ =>
            {
                StartObserveEnergy();
            }).AddTo(this);
        }

        private void StartObserveEnergy()
        {
            var onOff = _shopLocal.CheckEnergyBooster();
            Icon_Energy.SetActive(!onOff);
            FX_EnergyBooster.SetActive(onOff); 
            Obj_EnergyBoosterTimer.SetActive(onOff);
            if (onOff)
            {
                _energyManager.EnergyChargeTime.Value = 1;
                var time = _userData.EnergyBoosterEndTime.Value;
                var timeSpan = time.Subtract(GameUtils.GetGameTime());
                if ((int) timeSpan.TotalSeconds < 0)
                {
                    _userData.EnergyBoosterEndTime.Value = _shopLocal.GetEnergyBoosterExpiredTime();
                    timeSpan = time.Subtract(GameUtils.GetGameTime());
                }

                var totalMin = (int) timeSpan.TotalMinutes;
                var totalSec = (int) timeSpan.TotalSeconds+1;
                if (totalMin < 1)
                    GameUtils.I2SetFont(Text_EnergyBooster,  GameUtils.GetDuration_Sec(totalSec));
                else
                    GameUtils.I2SetFont(Text_EnergyBooster,  GameUtils.GetTwoDuration_Min(totalMin));
                GameUtils.createCountDownObservable((int)timeSpan.TotalSeconds).Subscribe(x =>
                {
                    var totalMin2 = x / 60;
                    if (totalMin2 < 1)
                        GameUtils.I2SetFont(Text_EnergyBooster, GameUtils.GetDuration_Sec(x));
                    else
                        GameUtils.I2SetFont(Text_EnergyBooster, GameUtils.GetTwoDuration_Min(totalMin2));
                }, StartObserveEnergy).AddTo(this);
            }
            else
            {
                _energyManager.EnergyChargeTime.Value = EnergyManager.DefaultChargeMinute;
            }
        }

        private IObservable<Unit> GetObservableEnergy()
        {
            IObservable<Unit> observable = null;
            if (_shopLocal.ShopItems.ContainsKey("Energy"))
            {
                if (_shopLocal.ShopItems["Energy"].Items.ContainsKey("EnergyBooster_0"))
                {
                    var energyObservable = _shopLocal.ShopItems["Energy"].Items["EnergyBooster_0"].PurchaseCount.AsUnitObservable();
                    observable = energyObservable;
                }
                else
                {
                    var shopItemObservable = _shopLocal.ShopItems["Energy"].Items.ObserveAdd().AsUnitObservable().Merge(
                        _shopLocal.ShopItems["Energy"].Items.ObserveReset().AsUnitObservable(),
                        _shopLocal.ShopItems["Energy"].Items.ObserveRemove().AsUnitObservable(),
                        _shopLocal.ShopItems["Energy"].Items.ObserveReplace().AsUnitObservable(),
                        _shopLocal.ShopItems["Energy"].Items.ObserveCountChanged().AsUnitObservable());
                    observable = shopItemObservable;
                }
            }
            else
            {
                var shopObservable = _shopLocal.ShopItems.ObserveAdd().AsUnitObservable()
                    .Merge(_shopLocal.ShopItems.ObserveReplace().AsUnitObservable(),
                        _shopLocal.ShopItems.ObserveReset().AsUnitObservable(),
                        _shopLocal.ShopItems.ObserveCountChanged(false).AsUnitObservable());
                observable = shopObservable;
            }

            return observable.BatchFrame();
        }
    }
}
