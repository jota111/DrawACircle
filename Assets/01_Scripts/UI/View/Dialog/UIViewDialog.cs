/*
* Copyright (c) CookApps.
* 이진호(jhlee8@cookapps.com)
*/

using System;
using OutGameCore;
using Sirenix.OdinInspector;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace SH.UI.View.Dialog
{
    public class UIViewDialog : UIViewBase
    {
        public static UIViewDialog Instance { get; private set; }
        
        [SerializeField, Required, ChildGameObjectsOnly]
        private Button _buttonOK;

        [SerializeField, Required, ChildGameObjectsOnly]
        private Text _textMsg;

        [SerializeField, Required, ChildGameObjectsOnly]
        private Text _textTitle;
        
        [SerializeField, Required, ChildGameObjectsOnly]
        private Text _textOK;

        private UIViewManager _uiViewManager;

        [Inject]
        private void Construct(UIViewManager uiViewManager)
        {
            _uiViewManager = uiViewManager;
            Instance = this;
        }

        public void DialogForget(string termMsg, string termTitle, string termOK = "Dialog_OK",
            bool buttonEnable = true, bool clearUiStack = true)
        {
            DialogObservable(termMsg, termTitle, termOK, buttonEnable, clearUiStack).Subscribe();
        }
        
        public IObservable<Unit> DialogObservable(string termMsg, string termTitle, string termOK = "Dialog_OK", bool buttonEnable = true, bool clearUiStack = true)
        {
            if(GameUtils.CheckI2Key(termMsg))
                GameUtils.I2Format(_textMsg, termMsg);
            else GameUtils.I2SetFont(_textMsg, termMsg);
            GameUtils.I2Format(_textTitle, termTitle);
            GameUtils.I2Format(_textOK, termOK);
            _buttonOK.gameObject.SetActive(buttonEnable);
            
            Show();
            
            return _buttonOK.OnClickAsObservable()
                .First()
                .Do(_ =>
                {
                    if(clearUiStack)
                        _uiViewManager.ClearStack();
                    Hide();
                })
                .TakeUntilDisable(this);
        }
    }
}