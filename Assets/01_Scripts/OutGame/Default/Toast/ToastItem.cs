using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using MoreLinq;
using Sirenix.OdinInspector;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace OutGameCore.UI
{
    public class ToastItem : MonoBehaviour
    {
        [SerializeField] private Image[] _images;
        [SerializeField] private Text _text;
        [NonSerialized] public bool IsAlive = false;
        [NonSerialized] public DateTime StartTime = DateTime.MinValue;
        private IDisposable _timer = Disposable.Empty;

        public void Set(string text)
        {
            _text.text = text;
        }
        
        public async UniTask Fade(bool onOff, float duration)
        {
            if (onOff)
            {
                IsAlive = true;
                StartTime = DateTime.Now;
            }
            else
            {
                IsAlive = false;
                _timer?.Dispose();
            }

            _images.ForEach(x=>x.FadeForce(onOff, duration).Forget());
            await _text.Fade(onOff, duration);
        }

        public async UniTask Move(Vector3 pos, float duration)
        {
            await transform.DOMove(pos, duration);
        }

        public void SetFadeTimer(IDisposable timer)
        {
            _timer?.Dispose();
            _timer = timer;
        }

        public void Kill()
        {
            _text.gameObject.SetActive(false);
            _images.ForEach(x => x.gameObject.SetActive(false));
            _images.ForEach(x=>x.DOKill());
            _text.DOKill();
            transform.DOKill();
            _timer?.Dispose();
            IsAlive = false;
        }

        [Button]
        public void Test_RegistImages()
        {
            _images = transform.GetComponentsInChildren<Image>();
        }
    }
}
