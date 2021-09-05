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
    public sealed class TextJewel : MonoBehaviour
    {
        public Color initColor;
        
        [Inject]
        private void Construct(FundManager fundManager)
        {
            var text = GetComponent<Text>();
            text.text = fundManager.Jewel.ToString();
            initColor = text.color;
            DateTime updateTime = DateTime.MinValue;

            Invalidate.Receive(Invalidate.TargetType.Jewel)
                .Select(_ => fundManager.Jewel.Value)
                .DistinctUntilChanged().Subscribe(x =>
                {
                    text.text = x.ToString();
                    updateTime = DateTime.Now;
                })
                .AddTo(this);
            
            Queue<(int, DateTime)> delayedValue = new Queue<(int, DateTime)>();
            Invalidate.DelayedReceive(Invalidate.TargetType.Jewel, false)
                .Select(_ => fundManager.Jewel.Value)
                .Subscribe(x => { delayedValue.Enqueue((x, DateTime.Now)); })
                .AddTo(this);
            Invalidate.DelayedReceive(Invalidate.TargetType.Jewel, true)
                .Select(_ => delayedValue)
                .Subscribe(x =>
                {
                    var value = delayedValue.Dequeue();
                    if(value.Item2 > updateTime)
                        text.text = value.Item1.ToString();
                    else
                    {
                        text.text = fundManager.Jewel.ToString();
                        updateTime = DateTime.Now;
                    }
                })
                .AddTo(this);
        }
    }
}