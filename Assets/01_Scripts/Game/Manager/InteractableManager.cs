/*
* Copyright (c) CookApps.
* 이진호(jhlee8@cookapps.com)
*/

using System;
using OutGameCore;
using SH.Util.UniRx;
using UniRx;

namespace SH.Game.Manager
{
    public sealed class InteractableManager : IDisposable
    {
        public static InteractableManager Instance { get; private set; }
        private readonly IntReactiveProperty _interactableUI = new IntReactiveProperty(0);
        public IReadOnlyReactiveProperty<bool> InteractableUI { get; }
        public bool IsInteractableUI() => _interactableUI.Value == 0;

        private readonly IntReactiveProperty _interactablePanAndZoom = new IntReactiveProperty(0);
        public IReadOnlyReactiveProperty<bool> InteractablePanAndZoom { get; }
        public bool IsInteractablePanAndZoom() => _interactablePanAndZoom.Value == 0;

        public InteractableManager(SceneDisposable disposable)
        {
            Instance = this;
            InteractableUI = _interactableUI.Select(value => value == 0).ToReadOnlyReactiveProperty();
            InteractablePanAndZoom = _interactablePanAndZoom.Select(value => value == 0).ToReadOnlyReactiveProperty();
            InteractablePanAndZoom
                .Subscribe(enable =>
                {
                    if (OutGame.Exist)
                    {
                        // OutGame.Instance.cameraManager.AllowPanAndZoom("Interactable", enable);   
                    }
                })
                .AddTo(disposable);
        }

        public ScopeLockUI LockUI()
        {
            return new ScopeLockUI(this);
        }

        public ScopeLockPanAndZoom LockPanAndZoom()
        {
            return new ScopeLockPanAndZoom(this);
        }

        public void SetInteractableUI(bool value)
        {
            var num = value ? -1 : 1;
            _interactableUI.Value += num;
        }

        public void SetInteractablePanAndZoom(bool value)
        {
            var num = value ? -1 : 1;
            _interactablePanAndZoom.Value += num;
        }
        
        public void Dispose()
        {
            Instance = null;
        }

        //------------------------------------------------------------------------------------------------
        
        public readonly struct ScopeLockUI : IDisposable
        {
            private readonly InteractableManager InteractableManager;
            public ScopeLockUI(InteractableManager interactableManager)
            {
                InteractableManager = interactableManager;
                InteractableManager.SetInteractableUI(false);
            }

            public void Dispose() => InteractableManager.SetInteractableUI(true);

        }
        
        public readonly struct ScopeLockPanAndZoom : IDisposable
        {
            private readonly InteractableManager InteractableManager;
            public ScopeLockPanAndZoom(InteractableManager interactableManager)
            {
                InteractableManager = interactableManager;
                InteractableManager.SetInteractablePanAndZoom(false);
            }

            public void Dispose() => InteractableManager.SetInteractablePanAndZoom(true);
        }
    }
}