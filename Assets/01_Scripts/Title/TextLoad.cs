/*
* Copyright (c) CookApps.
* 이진호(jhlee8@cookapps.com)
*/

using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using OutGameCore;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace SH.Title
{
    [RequireComponent(typeof(Text))]
    public sealed class TextLoad : MonoBehaviour
    {
        [SerializeField]
        private float _interval = 2;
        
        [SerializeField]
        private string[] _terms;

        private Text _text;

        private int _index;

        private void Start()
        {
            _text = GetComponent<Text>();
            _text.text = GameUtils.I2Format(_terms[_index]);
            
            Observable.Interval(TimeSpan.FromSeconds(_interval))
                .Subscribe(_ => SetText())
                .AddTo(this);
        }

        private async void SetText()
        {
            _index++;

            if (_index >= _terms.Length)
                return;

            await _text.DOFade(0, 0.25f).WithCancellation(this.GetCancellationTokenOnDestroy());
            _text.text = GameUtils.I2Format(_terms[_index]);
            await _text.DOFade(1, 0.25f).WithCancellation(this.GetCancellationTokenOnDestroy());
        }

        private void OnDestroy()
        {
            _text.DOKill();
        }
    }
}