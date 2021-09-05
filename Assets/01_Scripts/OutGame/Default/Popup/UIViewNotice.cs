using System;
using System.Collections;
using System.Collections.Generic;
using Doozy.Engine.UI;
using OutGameCore;
using SH.UI.View;
using UniRx;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Community.Default
{
    public class UIViewNotice : UIViewBase
    {
        public static UIViewNotice InstanceBase { get; private set; }
        [SerializeField] protected Text Text_Description;
        [SerializeField] protected Action onCloseAction;
        [SerializeField] protected Button Btn_Close;

        void Awake()
        {
            InstanceBase = this;
            if(Btn_Close != null)
                Btn_Close.OnClickAsObservable().Subscribe(_ => OnCloseButtonClicked()).AddTo(this);
        }

        //TODO : I2로컬라이징
        public virtual void SetView(string descriptionKey, Action _onCloseAction = null)
        {
            GameUtils.I2Format(Text_Description, descriptionKey);
            onCloseAction = _onCloseAction;
            Show();
        }

        public virtual void OnCloseButtonClicked()
        {
            onCloseAction?.Invoke();
            onCloseAction = null;
            HideSelf();
        }
        
        protected void HideSelf()
        {
            Hide();
        }
    }
}