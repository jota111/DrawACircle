/*
* Copyright (c) CookApps.
* 이진호(jhlee8@cookapps.com)
*/

using Cysharp.Threading.Tasks;
using Doozy.Engine.UI;
using SH.Game.Manager;
using UniRx;
using UnityEngine;
using Zenject;

namespace SH.UI.View
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(UIView))]
    public abstract class UIViewBase : MonoBehaviour
    {
        [SerializeField] private bool _useStack = true;

        [SerializeField] private bool _useBackButton = true;

        [SerializeField] private TopUIManager.TopUIType _topUIType = TopUIManager.TopUIType.All;

        public bool UseBackButton => _useBackButton;
        public void SetBackButton(bool value) => _useBackButton = value;

        private UIView View;
        private UIViewManager _viewManager;

        [Inject]
        private void ConstructBase(UIViewManager viewManager)
        {
            _viewManager = viewManager;
            View = GetComponent<UIView>();
            var rectTransform = GetComponent<RectTransform>();

            if (_useStack)
            {
                View.ShowBehavior.OnStart.Event.AsObservable()
                    .Do(_ => viewManager?.Push(View))
                    .Subscribe(_ =>
                    {
                        rectTransform?.SetAsLastSibling();
                        OnStartShow();
                    })
                    .AddTo(this);

                View.HideBehavior.OnStart.Event.AsObservable()
                    .Do(_ => viewManager?.Pop(View))
                    .Subscribe(_ => OnStartHide())
                    .AddTo(this);
                
                View.HideBehavior.OnFinished.Event.AsObservable()
                    .Subscribe(_ => OnFinishedHide())
                    .AddTo(this);
            }
        }

        protected void ClearStack()
        {
            _viewManager?.ClearStack();
        }

        public virtual void Show(bool instantAction = false, params object[] args)
        {
            if (!View.IsShowing && !View.IsVisible)
            {
                View.Show(instantAction || (_viewManager.GetStackCount >= 1 && !_viewManager.CheckCantInstant(View.ViewName)));
            }
        }

        public virtual void Hide(bool instantAction = false)
        {
            if (!View.IsHidden && !View.IsHiding)
            {
                View.Hide(instantAction || (_viewManager.GetStackCount > 1 && !_viewManager.CheckCantInstant(View.ViewName)));
            }
        }

        public void HideForce()
        {
            if (!View.IsHidden && !View.IsHiding)
            {
                View.Hide(true);
            }

            if (_viewManager.Contains(View))
                _viewManager.Remove(View);
        }

        protected virtual void OnStartShow()
        {
            isShown = true;
            if (TopUIManager.Instance != null && IsPushingTop())
                TopUIManager.Instance.Push(View.ViewName, _topUIType);
            if (LobbyUIManager.Instance != null)
                LobbyUIManager.Instance.Push(View.ViewName);
        }

        protected virtual void OnStartHide()
        {
            if (isShown)
            {
                if (TopUIManager.Instance != null)
                    TopUIManager.Instance.Pop(View.ViewName);
                if (LobbyUIManager.Instance != null)
                    LobbyUIManager.Instance.Pop(View.ViewName);
            }

            isShown = false;
        }

        protected virtual void OnFinishedHide()
        {
            // empty
        }

        private bool IsPushingTop()
        {
            return ((UIViewManager.CheckView(UIViewName.Shop) && View.ViewName.Equals(UIViewName.Shop)) ||
                    (!UIViewManager.CheckView(UIViewName.Shop)));
        }

        public bool IsShown => isShown;
        private bool isShown = false;
    }
}