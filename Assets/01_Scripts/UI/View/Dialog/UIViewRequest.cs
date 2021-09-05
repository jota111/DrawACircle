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
    public class UIViewRequest : UIViewBase
    {
        public static UIViewRequest Instance { get; private set; }
        
        [SerializeField, Required, ChildGameObjectsOnly]
        private Button _buttonOK;

        [SerializeField, Required, ChildGameObjectsOnly]
        private Text _textMsg;

        [SerializeField, Required, ChildGameObjectsOnly]
        private Text _textTitle;
        
        [SerializeField, Required, ChildGameObjectsOnly]
        private Text _textOK;
        
        [SerializeField, Required, ChildGameObjectsOnly]
        private Text _textCancel;

        private UIViewManager _uiViewManager;

        [Inject]
        private void Construct(UIViewManager uiViewManager)
        {
            _uiViewManager = uiViewManager;
            Instance = this;
        }

        public void DialogForget(string termMsg, string termTitle, string termOK = "Dialog_OK", string termNO = "Text_Cancel", Action onOK = null,
            bool buttonEnable = true, bool clearUiStack = false)
        {
            DialogObservable(termMsg, termTitle, termOK, termNO, onOK, buttonEnable, clearUiStack).Subscribe();
        }
        
        public IObservable<Unit> DialogObservable(string termMsg, string termTitle, string termOK = "Dialog_OK", string termNO = "Text_Cancel"
            , Action onOK = null, bool buttonEnable = true, bool clearUiStack = false)
        {
            if(GameUtils.CheckI2Key(termMsg))
                GameUtils.I2Format(_textMsg, termMsg);
            else GameUtils.I2SetFont(_textMsg, termMsg);
            GameUtils.I2Format(_textTitle, termTitle);
            GameUtils.I2Format(_textOK, termOK);
            GameUtils.I2Format(_textCancel, termNO);
            _buttonOK.gameObject.SetActive(buttonEnable);
            
            Show();
            
            return _buttonOK.OnClickAsObservable()
                .First()
                .Do(_ =>
                {
                    if(clearUiStack)
                        _uiViewManager.ClearStack();
                    Hide();
                    onOK?.Invoke();
                })
                .TakeUntilDisable(this);
        }
    }
}