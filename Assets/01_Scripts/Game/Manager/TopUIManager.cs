using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using OutGameCore;
using SH.UI.Fund;
using SH.UI.Top;
using SH.Util;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace SH.Game.Manager
{
    [Serializable]
    public class TopUIDict : UnitySerializedDictionary<TopUIManager.TopUIType, TopUIView>
    {
        
    }

    public class TopUIManager : MonoBehaviour
    {
        public static TopUIManager Instance { get; private set; }
        [SerializeField] private RectTransform heightRect;
        private RectTransform _rectTransform;
        [NonSerialized] private float initPosY;
        private bool isShowed = true;
        private Tweener tweener;
        [SerializeField] private TopUIDict _topUIView;
        [SerializeField] private TopUIDict _topUIView_Sub;
        [SerializeField] private TopUIDict _topUIPos;
        public float HidePosY => heightRect.position.y + 150;

        public static float ShowDuration = 0.3f;
        public static float HideDuration = 0.3f;
        public static float ShowDelay = 0.15f;

        [Inject]
        private void Construct()
        {
            if (GetComponent<RectTransform>() == null) return;
            _rectTransform = GetComponent<RectTransform>();
            initPosY = GetComponent<RectTransform>().position.y;
            Instance = this;
        }

        void OnDestroy()
        {
            frameObserve?.Dispose();
        }

        public void Show(float duration = 1f)
        {
            Pop("TopUI");
            return;
            if (isShowed) return;
            if (_rectTransform == null)
                _rectTransform = GetComponent<RectTransform>();
            tweener?.Kill();
            tweener = _rectTransform.DOLocalMoveY(initPosY, duration);
            isShowed = true;
        }

        public int GetCount() => uiStack.Count;

        public void Hide(float duration = 1f)
        {
            Push("TopUI", TopUIType.Empty);
            return;
            if (!isShowed) return;
            if (_rectTransform == null)
                _rectTransform = GetComponent<RectTransform>();
            tweener?.Kill();
            // var hidePosY = OutGame.Instance.cameraManager.CameraHeight / 2f + 100;
            tweener = _rectTransform.DOMoveY(heightRect.position.y + 150, duration);
            isShowed = false;
        }

        public Dictionary<string, TopUIType> uiStack = new Dictionary<string, TopUIType>();
        private IDisposable frameObserve;
        private IDisposable timeObserve;
        private TopUIType currentUIType;
        
        public void Push(string key, TopUIType topUIType)
        {
            uiStack.TryAdd(key, topUIType);
            SetView(topUIType, true);
        }

        private void SetView(TopUIType topUIType, bool hasDelay = false)
        {
            frameObserve?.Dispose();
            timeObserve?.Dispose();
            _topUIView_Sub[TopUIType.Level].OnOff(false);
            _topUIView_Sub[TopUIType.Energy].OnOff(false);
            _topUIView_Sub[TopUIType.Coin].OnOff(false);
            _topUIView_Sub[TopUIType.Jewel].OnOff(false);
            Dictionary<TopUIType, Vector3> tempPos = new Dictionary<TopUIType, Vector3>();
            if (topUIType != TopUIType.Empty)
            {
                tempPos.Add(TopUIType.Level, _topUIPos[TopUIType.Level].transform.position);
                tempPos.Add(TopUIType.Energy, _topUIPos[TopUIType.Energy].transform.position);
                tempPos.Add(TopUIType.Coin, _topUIPos[TopUIType.Coin].transform.position);
                tempPos.Add(TopUIType.Jewel, _topUIPos[TopUIType.Jewel].transform.position);
            }
            _topUIPos[TopUIType.Level].OnOff(topUIType.HasFlag(TopUIType.Level));
            _topUIPos[TopUIType.Energy].OnOff(topUIType.HasFlag(TopUIType.Energy));
            _topUIPos[TopUIType.Coin].OnOff(topUIType.HasFlag(TopUIType.Coin));
            _topUIPos[TopUIType.Jewel].OnOff(topUIType.HasFlag(TopUIType.Jewel));
            frameObserve = Observable.TimerFrame(1).Subscribe(_ =>
            {
                if (topUIType != TopUIType.Empty)
                {
                    SetSub(TopUIType.Level);
                    SetSub(TopUIType.Energy);
                    SetSub(TopUIType.Coin);
                    SetSub(TopUIType.Jewel);
                }
                _topUIView[TopUIType.Level].ShowY(topUIType.HasFlag(TopUIType.Level), _topUIPos[TopUIType.Level].transform.position, topUIType != TopUIType.Empty, hasDelay && topUIType.HasFlag(TopUIType.Level));
                _topUIView[TopUIType.Energy].ShowY(topUIType.HasFlag(TopUIType.Energy), _topUIPos[TopUIType.Energy].transform.position, topUIType != TopUIType.Empty, hasDelay && topUIType.HasFlag(TopUIType.Energy));
                _topUIView[TopUIType.Coin].ShowY(topUIType.HasFlag(TopUIType.Coin), _topUIPos[TopUIType.Coin].transform.position, topUIType != TopUIType.Empty, hasDelay && topUIType.HasFlag(TopUIType.Coin));
                _topUIView[TopUIType.Jewel].ShowY(topUIType.HasFlag(TopUIType.Jewel), _topUIPos[TopUIType.Jewel].transform.position, topUIType != TopUIType.Empty, hasDelay && topUIType.HasFlag(TopUIType.Jewel));
                currentUIType = topUIType;
            }).AddTo(this);

            async UniTask SetSub(TopUIType type)
            {
                if (!currentUIType.HasFlag(type)) return;
                _topUIView_Sub[type].OnOff(true);
                _topUIView_Sub[type].ShowY(false, tempPos[type], false);
                await UniTask.Delay(TimeSpan.FromSeconds(HideDuration *1.5f));
                _topUIView_Sub[type].OnOff(false);
            }
        }

        public void Pop(string key)
        {
            if (uiStack.ContainsKey(key))
            {
                uiStack.Remove(key);
                if (uiStack.Count == 0)
                {
                    SetView(TopUIType.All, true);
                }
                else
                {
                    var view = uiStack.LastOrDefault();
                    SetView(view.Value);
                }
            }else if (uiStack.Count == 0)
                SetView(TopUIType.All, true); 
        }

        [Flags, Serializable]
        public enum TopUIType
        {
            Empty = 0,
            Level= 1 << 1, // 0001,
            Energy= 1 << 2,  // 0010,
            Coin= 1 << 3,    // 0100,
            Jewel= 1 << 4,   // 1000,
            All = Level | Energy | Coin | Jewel,
        }
    }
}