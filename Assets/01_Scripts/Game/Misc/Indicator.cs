/*
* Copyright (c) CookApps.
* 이진호(jhlee8@cookapps.com)
*/

using System;
using DG.Tweening;
using UniRx;
using UnityEngine;

namespace SH.Game.Misc
{
    public interface IIndicator
    {
        void Inc();
        void Dec();
        void Clear();
        IDisposable ScopeLock();
    }
    
    public readonly struct ScopeLockIndicator : IDisposable
    {
        private readonly IIndicator Indicator;
        public ScopeLockIndicator(IIndicator indicator)
        {
            Indicator = indicator;
            Indicator.Inc();
        }

        public void Dispose() => Indicator.Dec();

    }
    
    [RequireComponent(typeof(CanvasGroup))]
    public class Indicator : MonoBehaviour, IIndicator
    {
        private readonly IntReactiveProperty _count = new IntReactiveProperty(0);
        private CanvasGroup _canvasGroup;

        private void Awake()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
            _canvasGroup.alpha = 0;
            
            _count.Select(value => value > 0)
                .Subscribe(gameObject.SetActive)
                .AddTo(this);
        }

        public void Inc()
        {
            _count.Value++;
        }

        public void Dec()
        {
            _count.Value--;
        }

        public void Clear()
        {
            _count.Value = 0;
        }

        public IDisposable ScopeLock()
        {
            return new ScopeLockIndicator(this);
        }

        private void OnEnable()
        {
            _canvasGroup.alpha = 0;
            _canvasGroup.DOFade(1, 0.2f);
        }

        private void OnDisable()
        {
            _canvasGroup.DOKill();
        }
    }
}