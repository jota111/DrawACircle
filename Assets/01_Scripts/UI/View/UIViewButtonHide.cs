/*
* Copyright (c) CookApps.
* 이진호(jhlee8@cookapps.com)
*/

using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Doozy.Engine.UI;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace SH.UI.View
{
    
    [DisallowMultipleComponent]
    [RequireComponent(typeof(UIView))]
    [RequireComponent(typeof(Button))]
    public class UIViewButtonHide : MonoBehaviour
    {
        private const float ReleaseTime = 0.75f;
        private CancellationTokenSource _cts = new CancellationTokenSource();

        private Button _button;
        private UIView _uiView;
        private UIViewBase _uiViewBase;

        private void Awake()
        {
            _button = GetComponent<Button>();
            _uiView = GetComponent<UIView>();
            _uiViewBase = GetComponent<UIViewBase>();
            
            _button
                .OnClickAsObservable()
                .ThrottleFirst(TimeSpan.FromSeconds(0.5f))
                .Subscribe(_ =>
                {
                    if (_uiViewBase)
                        _uiViewBase.Hide((UIViewManager.Instance.GetStackCount > 1 && !UIViewManager.Instance.CheckCantInstant(_uiView.ViewName)));
                    else
                        _uiView.Hide();
                })
                .AddTo(this);
        }

        private async void OnEnable()
        {
            if(_button != null) _button.interactable = false;
            _cts?.Dispose();
            _cts = new CancellationTokenSource();
            await UniTask.Delay(TimeSpan.FromSeconds(ReleaseTime), cancellationToken: _cts.Token).SuppressCancellationThrow();
            if(_button != null) _button.interactable = true;
        }

        private void OnDisable()
        {
            _cts.Cancel();
        }

        private void OnDestroy()
        {
            _cts.Cancel();
            _cts.Dispose();
        }
    }
}