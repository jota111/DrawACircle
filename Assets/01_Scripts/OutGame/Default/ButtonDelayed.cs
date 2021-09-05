using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace OutGameCore.UI
{
    [RequireComponent(typeof(Button))]
    public class ButtonDelayed : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField, LabelText("첫 터치 후 딜레이")]
        private float OnPressFirstDelay = 0.2f;

        private bool buttonPressed = false;
        protected Action _doWhenPressed = null;

        public virtual Action DoWhenPressed
        {
            get => _doWhenPressed;
            set => _doWhenPressed = value;
        }

        public IDisposable Timer = Disposable.Empty;

        public virtual void OnPointerDown(PointerEventData eventData)
        {
            buttonPressed = true;
            if (Timer != null)
            {
                Timer.Dispose();
                Timer = Disposable.Empty;
            }

            Timer = Observable.Timer(TimeSpan.FromSeconds(OnPressFirstDelay), Scheduler.MainThreadIgnoreTimeScale).TakeUntilDisable(this).Subscribe(
                x =>
                {
                    if (buttonPressed)
                    {
                        if (DoWhenPressed != null)
                            DoWhenPressed();
                    }
                }).AddTo(gameObject);
        }

        public virtual void OnPointerUp(PointerEventData eventData)
        {
            buttonPressed = false;
            if (Timer != null)
            {
                Timer.Dispose();
                Timer = Disposable.Empty;
            }
        }

        public virtual void OnDisable()
        {
            buttonPressed = false;
            if (Timer != null)
            {
                Timer.Dispose();
                Timer = Disposable.Empty;
            }
        }
    }
}