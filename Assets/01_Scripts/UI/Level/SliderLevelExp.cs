/*
* Copyright (c) CookApps.
* 이진호(jhlee8@cookapps.com)
*/

using System;
using System.Collections.Generic;
using DG.Tweening;
using SH.Game.Manager;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace SH.UI.Level
{
    [RequireComponent(typeof(Slider))]
    public class SliderLevelExp : MonoBehaviour
    {
        [Inject]
        private void Construct(LevelManager levelManager)
        {
            var slider = GetComponent<Slider>();
            slider.value = levelManager.LevelUpPercent.Value;
            DateTime updateTime = DateTime.MinValue;
            
            Invalidate.Receive(Invalidate.TargetType.Exp)
                .Select(_ => levelManager.LevelUpPercent.Value)
                .DistinctUntilChanged()
                .Subscribe(value =>
                {
                    slider.DOValue(value, 0.25f).SetRecyclable(true);
                    updateTime = DateTime.Now;
                })
                .AddTo(this);
            
            Queue<(float, DateTime)> delayedValue = new Queue<(float, DateTime)>();
            Invalidate.DelayedReceive(Invalidate.TargetType.Exp, false)
                .Select(_ => levelManager.LevelUpPercent.Value)
                .Subscribe(x => { delayedValue.Enqueue((x, DateTime.Now)); })
                .AddTo(this);
            Invalidate.DelayedReceive(Invalidate.TargetType.Exp, true)
                .Select(_ => delayedValue)
                .Subscribe(x =>
                {
                    var value = delayedValue.Dequeue();
                    if(value.Item2 > updateTime)
                        slider.DOValue(value.Item1, 0.25f).SetRecyclable(true);
                    else slider.DOValue(levelManager.LevelUpPercent.Value, 0.25f).SetRecyclable(true);
                })
                .AddTo(this);
        }
    }
}