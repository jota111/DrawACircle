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
    public class ButtonPressed : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField, LabelText("첫 터치 후 딜레이")] private float OnPressFirstDelay = 0.2f;
        [SerializeField, LabelText("액션간의 딜레이")] private float OnPressDelay = 0.3f;
        private bool buttonPressed = false;
        protected Action _doWhilePressed = null;

        public virtual Action DoWhilePressed
        {
            get => _doWhilePressed;
            set => _doWhilePressed = value;
        }

        public IDisposable Updator = Disposable.Empty;

        void OnEnable()
        {
        }

        public virtual void OnPointerDown(PointerEventData eventData)
        {
            buttonPressed = true;
            DoWhilePressed();
            if (Updator != null)
            {
                Updator.Dispose();
                Updator = Disposable.Empty;
            }

            Observable.Timer(TimeSpan.FromSeconds(OnPressFirstDelay), Scheduler.MainThreadIgnoreTimeScale).TakeUntilDisable(this).Subscribe(x =>
            {
                Updator = Observable.Interval(TimeSpan.FromSeconds(OnPressDelay), Scheduler.MainThreadIgnoreTimeScale).Subscribe(_ =>
                {
                    if (buttonPressed)
                    {
                        if (DoWhilePressed != null)
                            DoWhilePressed();
                    }
                }).AddTo(gameObject);
            }).AddTo(gameObject);
        }

        public virtual void OnPointerUp(PointerEventData eventData)
        {
            buttonPressed = false;
            if (Updator != null)
            {
                Updator.Dispose();
                Updator = Disposable.Empty;
            }
        }

        public virtual void OnDisable()
        {
            buttonPressed = false;
            if (Updator != null)
            {
                Updator.Dispose();
                Updator = Disposable.Empty;
            }
        }
    }
}
