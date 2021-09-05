using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using OutGameCore;
using SH.Game.Manager;
using Sirenix.OdinInspector;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace SH.UI.Top
{
    [Serializable]
    public class TopUIView : MonoBehaviour
    {
        [SerializeField] private GameObject FlexibleVIew;
        [SerializeField] private List<Image> _images;
        [SerializeField] private List<Text> _texts;

        public void OnOff(bool onOff)
        {
            gameObject.SetActive(onOff);
            if (FlexibleVIew == null) return;
            FlexibleVIew.SetActive(onOff);
        }

        private bool isView = true;

        public void ShowY(bool onOff, Vector3 pos, bool forceFadeOff = false, bool hasDelayOn = false)
        {
            TryInitUI();
            timer?.Dispose();
            transform.DOKill();
            // 보여주기
            if (onOff)
            {
                if (hasDelayOn)
                {
                    OnOff(false);
                    timer = Observable.Timer(TimeSpan.FromSeconds(TopUIManager.ShowDelay)).Subscribe(y =>
                    {
                        OnOff(true);
                        _images.ForEach(x => x.SetAlpha(0));
                        _texts.ForEach(x => x.SetAlpha(0));
                        SetPos();
                        _images.ForEach(x => x.JustFade(true, TopUIManager.ShowDuration).Forget());
                        _texts.ForEach(x => x.Fade(true, TopUIManager.ShowDuration * 1.5f).Forget());
                    }).AddTo(this);
                }
                else
                {
                    OnOff(true);
                    _images.ForEach(x => x.SetAlpha(0));
                    _texts.ForEach(x => x.SetAlpha(0));
                    SetPos();
                    _images.ForEach(x => x.JustFade(true, TopUIManager.ShowDuration).Forget());
                    _texts.ForEach(x => x.Fade(true, TopUIManager.ShowDuration * 1.5f).Forget());
                }
            }
            // 감추기
            else
            {
                if (!forceFadeOff)
                {
                    _images.ForEach(x =>
                    {
                        x.DOKill();
                        x.SetAlpha(1);
                    });
                    _texts.ForEach(x =>
                    {
                        x.DOKill();
                        x.SetAlpha(1);
                    });
                    SetPos();
                    transform.DOMoveY(TopUIManager.Instance.HidePosY, TopUIManager.HideDuration);
                    _images.ForEach(x => x.JustFade(false, TopUIManager.HideDuration * 0.6f).Forget());
                    _texts.ForEach(x => x.Fade(false, TopUIManager.HideDuration * 0.3f).Forget());
                    timer = Observable.Timer(TimeSpan.FromSeconds(TopUIManager.HideDuration)).Subscribe(y => { OnOff(false); }).AddTo(this);
                }
                else
                {
                    _images.ForEach(x => x.SetAlpha(0));
                    _texts.ForEach(x => x.gameObject.SetActive(false));
                    OnOff(false);
                }
            }

            isView = onOff;

            void SetPos()
            {
                var position = transform.position;
                position = new Vector3(pos.x, pos.y, position.z);
                transform.position = position;
            }
        }

        private IDisposable timer;

        private void OnDestroy()
        {
            timer?.Dispose();
            timer = null;
        }

        public void Show(bool onOff, Vector3 pos)
        {
            TryInitUI();
            timer?.Dispose();
            if (onOff)
            {
                if (isView)
                {
                    _images.ForEach(x => x.Fade(false, TopUIManager.HideDuration).Forget());
                    _texts.ForEach(x => x.Fade(false, TopUIManager.HideDuration).Forget());
                }

                timer?.Dispose();
                timer = Observable.Timer(TimeSpan.FromSeconds(TopUIManager.HideDuration * 1.5f)).Subscribe(y =>
                {
                    SetPos();
                    _images.ForEach(x => x.Fade(true, TopUIManager.ShowDuration).Forget());
                    _texts.ForEach(x => x.Fade(true, TopUIManager.ShowDuration).Forget());
                }).AddTo(this);
            }
            // 감추기
            else
            {
                SetPos();
                _images.ForEach(x => x.Fade(false, TopUIManager.HideDuration).Forget());
                _texts.ForEach(x => x.Fade(false, TopUIManager.HideDuration).Forget());
            }

            isView = onOff;

            void SetPos()
            {
                var position = transform.position;
                position = new Vector3(pos.x, pos.y, position.z);
                transform.position = position;
            }
        }

        [Button]
        public void TryInitUI()
        {
            _images ??= new List<Image>();
            _texts ??= new List<Text>();
            if (_images.Count == 0)
                _images = transform.GetComponentsInChildren<Image>().ToList();
            if (_texts.Count == 0)
                _texts = transform.GetComponentsInChildren<Text>().ToList();
        }
    }
}