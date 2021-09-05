/*
* Copyright (c) CookApps.
* 이진호(jhlee8@cookapps.com)
*/

using System;
using SH.Constant;
using SH.Game.Misc;
using UniRx;

namespace SH.Game.Manager
{
    public class ScreenManager
    {
        public static ScreenManager Instance { get; private set; }
        private readonly IScreenTransitionMonoBehaviour _screenTransitionMonoBehaviour;

        private readonly ReactiveProperty<ScreenState> _screenState =
            new ReactiveProperty<ScreenState>(SH.Constant.ScreenState.Lobby);

        public IReadOnlyReactiveProperty<ScreenState> ScreenState => _screenState;

        public ScreenManager(IScreenTransitionMonoBehaviour screenTransitionMonoBehaviour)
        {
            Instance = this;
            _screenTransitionMonoBehaviour = screenTransitionMonoBehaviour;
        }
        
        public void Transition(ScreenState screen, Action action = null)
        {
            if (screen == ScreenState.Value)
            {
                action?.Invoke();
                return;
            }

            _screenTransitionMonoBehaviour.TransitionMonoBehaviour(screen, Func);
            
            //------------------------------------------------------------------------
            void Func()
            {
                action?.Invoke();
                _screenState.Value = screen;
            }
        }
    }
}