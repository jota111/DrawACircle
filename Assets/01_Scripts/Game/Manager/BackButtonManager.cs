/*
* Copyright (c) CookApps.
* 이진호(jhlee8@cookapps.com)
*/

using System;
using Doozy.Engine;
using Doozy.Engine.UI;
using SH.Constant;
using SH.Game.Misc;
using SH.Ob;
using SH.UI;
using UnityEngine;
using Zenject;

namespace SH.Game.Manager
{
    public class BackButtonManager : IInitializable, IDisposable
    {
        private readonly UIViewManager _uiViewManager;
        private readonly ScreenManager _screenManager;

        public BackButtonManager(UIViewManager uiViewManager, ScreenManager screenManager)
        {
            _uiViewManager = uiViewManager;
            _screenManager = screenManager;
        }
        
        public void Initialize()
        {
            Message.AddListener<UIButtonMessage>(OnMessage);
        }

        public void Dispose()
        {
            Message.RemoveListener<UIButtonMessage>(OnMessage);
        }
        
        private void OnMessage(UIButtonMessage message)
        {
            if (!message.ButtonName.Equals(UIButton.BackButtonName))
                return;
            
            // back 버튼이라면 처리
            if (_uiViewManager.PopLastByBackButton())
                return;

            if (_screenManager.ScreenState.Value == ScreenState.InGame)
            {
                _screenManager.Transition(ScreenState.Lobby);
                return;
            }

            MoveToBack();
        }

        [System.Diagnostics.Conditional("UNITY_ANDROID")]
        private void MoveToBack()
        {
            if (!AppCheck.IsPass.Value)
                return;
            
            if (!UIView.IsViewVisible(UIView.DefaultViewCategory, UIViewName.ExitGame))
            {
                UIView.ShowView(UIViewName.ExitGame);   
            }
        }
    }
}