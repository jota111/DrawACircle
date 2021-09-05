/*
* Copyright (c) CookApps.
* 이진호(jhlee8@cookapps.com)
*/

using System;
using Cysharp.Threading.Tasks;
using Doozy.Engine.UI;
using SH.Game.Misc;
using SH.Ob;
using SH.Util.UniRx;
using UniRx;
using UnityEngine;

namespace SH.UI.View.Cheater
{
    [RequireComponent(typeof(UIView))]
    public class UIViewCheater : MonoBehaviour
    {
        [SerializeField]
        private bool _autoDetection = true;

        private void Awake()
        {
            if (_autoDetection)
            {
                var view = GetComponent<UIView>();
                AppCheck.IsPass.WhereFalse()
                    .DelayFrame(3)
                    .Subscribe(_ => view.Show())
                    .AddTo(this);
            }
        }

        private async void Start()
        {
            await UniTask.NextFrame();
            
            var view = GetComponent<UIView>();
            view.HideBehavior.OnStart.Event.AsObservable()
                .Subscribe(_ => Application.Quit());
        }
    }
}