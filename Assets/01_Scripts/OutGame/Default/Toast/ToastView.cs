using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using MoreLinq;
using MoreLinq.Extensions;
using SH.Util.UniRx;
using Sirenix.OdinInspector;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace OutGameCore.UI
{
    public interface IToastView
    {
        void Inc(string text);
        void Dec();
        void Clear();
    }
    
    public readonly struct ToastData : IDisposable
    {
        private readonly IToastView FX;
        public ToastData(IToastView fx, string text)
        {
            FX = fx;
            FX.Inc(text);
        }

        public void Dispose() => FX.Dec();
    }
    
    public class ToastView : MonoBehaviour, IToastView
    {
        public static ToastView Instance { get; private set; }
        private List<ToastItem> _liveToasts = new List<ToastItem>();
        private List<ToastItem> _toastItems;
        private Transform parent;
        private int currentIdx = 0;
        private int maxIdx = 0;
        private List<Vector3> posList;
        //토스트아이템
        
        public const float FadeTime = 0.5f;
        public const float MoveTime = 0.5f;
        public const float Duration = 2f;

        [Inject]
        private void Construct()
        {
            Instance = this;
            _toastItems = transform.GetComponentsInChildren<ToastItem>(true).ToList();
            posList = _toastItems.Select(x=>x.transform.position).ToList();
            transform.GetComponentInChildren<VerticalLayoutGroup>().enabled = false;
            parent = _toastItems[0].transform.parent;
            maxIdx = _toastItems.Count - 2;
        }

        public void Inc(string text)
        {
            var toast = GetNewToastItem();
            StartFX(toast, text);
            _liveToasts.Add(toast); 
        }

        public void Dec()
        {
            // if (_liveToasts.Count == 0) return;
            // var toast = _liveToasts.Dequeue();
            // toast.Fade(false, FadeTime).Forget();
        }

        public void Clear()
        {
            
        }

        public static IDisposable Toast(string text)
        {
            return new ToastData(Instance, text);
        }

        private void StartFX(ToastItem toast, string value)
        {
            transform.localScale = Vector3.one;
            gameObject.SetActive(true);
            _liveToasts = _liveToasts.OrderByDescending(x => x.StartTime).ToList();
            for(int i = 0;i<_liveToasts.Count;i++)
            {
                _liveToasts[i].Move(posList[i+1], MoveTime).Forget();
            }

            Init_Toast();
            toast.Fade(true, FadeTime).Forget();
            toast.SetFadeTimer(Observable.Timer(TimeSpan.FromSeconds(Duration)).Subscribe(_ =>
            {
                _liveToasts.Remove(toast);
                toast.Fade(false, ToastView.FadeTime).Forget();
            }));

            void Init_Toast()
            {
                toast.gameObject.SetActive(true);
                toast.transform.DOKill();
                toast.transform.position = posList[0];
                toast.Set(value);
            }
        }

        private ToastItem GetNewToastItem()
        {
            if (maxIdx < _liveToasts.Count)
            {
                var list = _liveToasts.OrderBy(x => x.StartTime).ToList();
                var toast = list.ElementAt(0);
                toast.Fade(false, FadeTime).Forget();
                _liveToasts.Remove(toast);
            }

            var item = _toastItems.Find(x => !x.IsAlive && !_liveToasts.Contains(x));
            return item;
        }

        private void OnEnable()
        {
        }

        private void OnDisable()
        {
            _toastItems.ForEach(x=>x.Kill());
        }

        private int index = 0;
        [Button]
        public void Test_Toast()
        {
            index++;
            ToastView.Toast($"{index}_{index}asdf");
        }
    }
}
