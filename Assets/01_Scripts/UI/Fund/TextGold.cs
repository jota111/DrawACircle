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
    public sealed class TextGold : MonoBehaviour
    {
        public Color initColor;

        [Inject]
        private void Construct(FundManager fundManager)
        {
            var text = GetComponent<Text>();
            text.text = fundManager.Gold.ToString();
            initColor = text.color;
            DateTime updateTime = DateTime.MinValue;

            Invalidate.Receive(Invalidate.TargetType.Gold)
                .Select(_ => fundManager.Gold.Value)
                .DistinctUntilChanged().Subscribe(x =>
                {
                    text.text = x.ToString();
                    updateTime = DateTime.Now;
                })
                .AddTo(this);

            Queue<(int, DateTime)> delayedValue = new Queue<(int, DateTime)>();
            Invalidate.DelayedReceive(Invalidate.TargetType.Gold, false)
                .Select(_ => fundManager.Gold.Value)
                .Subscribe(x => { delayedValue.Enqueue((x, DateTime.Now)); })
                .AddTo(this);
            Invalidate.DelayedReceive(Invalidate.TargetType.Gold, true)
                .Select(_ => delayedValue)
                .Subscribe(x =>
                {
                    var value = delayedValue.Dequeue();
                    if(value.Item2 > updateTime)
                        text.text = value.Item1.ToString();
                    else
                    {
                        text.text = fundManager.Gold.ToString();
                        updateTime = DateTime.Now;
                    }
                })
                .AddTo(this);
        }
    }
}