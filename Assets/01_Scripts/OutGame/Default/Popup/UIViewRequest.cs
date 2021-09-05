using System;
using System.Collections;
using System.Collections.Generic;
using Doozy.Engine.UI;
using OutGameCore;
using SH.UI.View;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Community.Default
{
    public class UIViewRequest : UIViewBase
    {
        public static UIViewRequest InstanceBase { get; private set; }
        [SerializeField] private Text Text_Description;
        [SerializeField] private Action onCloseAction;
        [SerializeField] private Action onYesAction;
        [SerializeField] private Button Btn_Yes;
        [SerializeField] private Button Btn_No;
        
        void Awake()
        {
            InstanceBase = this;
            Btn_Yes.OnClickAsObservable().Subscribe(_ => OnYesButtonClicked()).AddTo(this);
            Btn_No.OnClickAsObservable().Subscribe(_ => OnCloseButtonClicked()).AddTo(this);
        }

        public void SetView(string descriptionKey, Action _onYesAction = null, Action _onCloseAction = null)
        {
            GameUtils.I2Format(Text_Description, descriptionKey);
            onYesAction = _onYesAction;
            onCloseAction = _onCloseAction;
        }

        public void OnYesButtonClicked()
        {
            onYesAction?.Invoke();
            
            HideSelf();
        }

        public void OnCloseButtonClicked()
        {
            onCloseAction?.Invoke();

            HideSelf();
        }

        private void HideSelf()
        {
            Hide();
        }
    }
}
