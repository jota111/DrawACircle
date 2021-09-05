/*
* Copyright (c) CookApps.
* 이진호(jhlee8@cookapps.com)
*/

using System;
using SH.Game.Manager;
using SH.Util;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace SH.UI.Fund
{
    [RequireComponent(typeof(Text))]
    public sealed class TextEnergyChargeTime : MonoBehaviour
    {
        [Inject]
        private void Construct(EnergyManager energyManager)
        {
            var text = GetComponent<Text>();

            TimeUtil.IntervalRemainTime().Merge(energyManager.PossibleChargeEnergy.AsUnitObservable())
                .Where(_ => energyManager.PossibleChargeEnergy.Value)
                .Select(_ => TimeString(energyManager.LeftChargeTime()))
                .SubscribeToText(text)
                .AddTo(this);

            energyManager.PossibleChargeEnergy
                .Subscribe(gameObject.SetActive)
                .AddTo(this);
        }

        private string TimeString(TimeSpan time)
        {
            return $"{time.Minutes}:{time.Seconds:00}";
        }
    }
}