/*
* Copyright (c) CookApps.
* 이진호(jhlee8@cookapps.com)
*/

using System;
using System.Collections.Generic;
using SH.Game.Manager;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace SH.UI.Fund
{
    [RequireComponent(typeof(Text))]
    public sealed class TextEnergy : MonoBehaviour
    {
        [Inject]
        private void Construct(EnergyManager energyManager)
        {
            var text = GetComponent<Text>();
            text.text = energyManager.Energy.ToString();
            DateTime updateTime = DateTime.MinValue;
            
            Invalidate.Receive(Invalidate.TargetType.Energy)
                .Select(_ => energyManager.Energy.Value)
                .DistinctUntilChanged().Subscribe(x =>
                {
                    text.text = x.ToString();
                    updateTime = DateTime.Now;
                })
                .AddTo(this);
            
            Queue<(int, DateTime)> delayedValue = new Queue<(int, DateTime)>();
            Invalidate.DelayedReceive(Invalidate.TargetType.Energy, false)
                .Select(_ => energyManager.Energy.Value)
                .Subscribe(x => { delayedValue.Enqueue((x, DateTime.Now)); })
                .AddTo(this);
            Invalidate.DelayedReceive(Invalidate.TargetType.Energy, true)
                .Select(_ => delayedValue)
                .Subscribe(x =>
                {
                    var value = delayedValue.Dequeue();
                    if(value.Item2 > updateTime)
                        text.text = value.Item1.ToString();
                    else
                    {
                        text.text = energyManager.Energy.ToString();
                        updateTime = DateTime.Now;
                    }
                })
                .AddTo(this);
        }
    }
}