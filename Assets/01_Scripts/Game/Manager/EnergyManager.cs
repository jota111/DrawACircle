/*
* Copyright (c) CookApps.
* 이진호(jhlee8@cookapps.com)
*/

using System;
using Doozy.Engine.UI;
using SH.AppEvent;
using SH.Game.DataLocal;
using SH.UI;
using SH.Util;
using SH.Util.UniRx;
using UniRx;
using UnityEngine.Assertions;
using Zenject;

namespace SH.Game.Manager
{
    public sealed class EnergyManager : IFixedTickable
    {
        public static EnergyManager Instance { get; private set; }
        private TimeSpan ChargeTime; // = TimeSpan.FromMinutes(2);
        private TimeSpan GetChargeTime(int multi) => TimeSpan.FromMinutes((EnergyChargeTime?.Value ?? 2) * multi);

        private readonly ObscuredIntReactiveProperty _energy;
        private readonly AppEventDataLocal _appEventDataLocal;

        public const int MaxEnergy = 100;
        public const int DefaultChargeMinute = 2;

        public ObscuredIntReactiveProperty EnergyChargeTime { get; set; }
        public IReadOnlyReactiveProperty<int> Energy { get; }
        public IReadOnlyReactiveProperty<bool> PossibleChargeEnergy { get; }
        public IReadOnlyReactiveProperty<DateTime> NextEnergyChargeTime { get; }
        private ReactiveProperty<DateTime> LastEnergyChargeTime { get; }

        public EnergyManager(UserData.UserData userData, SceneDisposable disposable)
        {
            Instance = this;
            _energy = userData.Energy;
            _appEventDataLocal = userData.AppEventData;
            Energy = _energy;
            LastEnergyChargeTime = userData.LastEnergyChargeTime;
            EnergyChargeTime = new ObscuredIntReactiveProperty(DefaultChargeMinute);
            var onOff = userData.Shop.CheckEnergyBooster();
            if (onOff)
                EnergyChargeTime.Value = 1;
            EnergyChargeTime.Subscribe(_ => { ChargeTime = TimeSpan.FromMinutes(EnergyChargeTime.Value); }).AddTo(disposable);
            ChargeTime = TimeSpan.FromMinutes(EnergyChargeTime.Value);
            NextEnergyChargeTime = LastEnergyChargeTime.AsUnitObservable().Merge(EnergyChargeTime.AsUnitObservable())
                .Select(time => LastEnergyChargeTime.Value + ChargeTime).ToReadOnlyReactiveProperty();
            PossibleChargeEnergy = Energy.Select(value => value < MaxEnergy).ToReadOnlyReactiveProperty();

            // 최대 에너지에서 떨어지면 마지막 충전시간을 지금으로
            Energy.Pairwise()
                .Where(pair => pair.Previous >= MaxEnergy && pair.Current < MaxEnergy)
                .Subscribe(_ => LastEnergyChargeTime.Value = TimeUtil.Now)
                .AddTo(disposable);

            // 최대 에너지 이상이면 마지막 충전을 지금으로
            Energy.Where(value => value >= MaxEnergy)
                .Subscribe(_ => LastEnergyChargeTime.Value = TimeUtil.Now)
                .AddTo(disposable);

            CheckEnergy();
        }

        public bool EnoughEnergy()
        {
            return _energy.Value > 0;
        }

        public bool UseEnergy(int value = 1)
        {
            if (!EnoughEnergy())
            {
                UIView.ShowView(UIViewName.BuyEnergy);
                return false;
            }

            _energy.Value -= value;
            Invalidate.Publish(Invalidate.TargetType.Energy);
            
            return true;
        }

        public void AddEnergy(int energy)
        {
            Assert.IsTrue(energy > 0);
            _energy.Value += energy;
        }

        public void FixedTick()
        {
            CheckEnergy();
        }

        private void CheckEnergy()
        {
            if (!PossibleChargeEnergy.Value)
            {
                LastEnergyChargeTime.Value = TimeUtil.Now;
                return;
            }

            var now = TimeUtil.Now;
            var diff = now - LastEnergyChargeTime.Value;
            var amount = (int) Math.Floor(diff.TotalSeconds / ChargeTime.TotalSeconds);
            if (amount <= 0)
                return;

            _energy.Value = Math.Min(_energy.Value + amount, MaxEnergy);

            LastEnergyChargeTime.Value = now;
            Invalidate.Publish(Invalidate.TargetType.Energy);
        }

        public TimeSpan LeftChargeTime()
        {
            if (!PossibleChargeEnergy.Value)
                return TimeSpan.Zero;

            return NextEnergyChargeTime.Value - TimeUtil.Now;
        }

        public DateTime FullChargeTime()
        {
            if (!PossibleChargeEnergy.Value)
                return TimeUtil.Now;
            var value = LastEnergyChargeTime.Value + (GetChargeTime(MaxEnergy - Energy.Value));
            return value;
        }
    }
}