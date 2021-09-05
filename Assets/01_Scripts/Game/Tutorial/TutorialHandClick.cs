/*
* Copyright (c) CookApps.
* 이진호(jhlee8@cookapps.com)
*/

using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using JetBrains.Annotations;
using Lean.Pool;
using OutGameCore;
using SH.Game.InGame.Msg;
using SH.Game.Manager;
using Sirenix.OdinInspector;
using UniRx;
using UnityEngine;
using UnityEngine.Assertions;
using Zenject;

namespace SH.Game.Tutorial
{
    public sealed class TutorialHandClick : MonoBehaviour
    {
        private static TutorialHandClick _instance = null;
        
        [SerializeField, Required, ChildGameObjectsOnly]
        private Transform _hand;

        private Transform _target;

        private string _id = null;
        
        [Inject]
        private void Construct(ScreenManager screenManager)
        {
            screenManager.ScreenState.DistinctUntilChanged()
                .Subscribe(_ => Despawn())
                .AddTo(this);
        }

        private void Awake()
        {
            var seq = DOTween.Sequence();
            seq.Append(_hand.DOScale(new Vector3(0.8f, 0.8f, 0.8f), 0.25f).SetLoops(2, LoopType.Yoyo));
            seq.AppendInterval(0.75f);
            seq.SetLoops(-1);
        }

        private async void SetClick([NotNull]Transform trans, string id)
        {
            _target = trans;
            _hand.gameObject.SetActive(false);
            var pos = GetPos();
            await UniTask.WaitForEndOfFrame();
            _hand.gameObject.SetActive(true);
            _hand.position = pos;
            _id = id;
        }

        private void OnDisable()
        {
            _target = null;
        }

        private void LateUpdate()
        {
            if (_target != null)
            {
                if (_target.gameObject.activeInHierarchy)
                {
                    _hand.position = GetPos();
                    CalcRotate();
                }
                else
                {
                    Despawn();    
                }
            }
        }

        private Vector3 GetPos()
        {
            Assert.IsNotNull(_target);
            if (_target is RectTransform)
            {
                return _target.position;
            }
            // else if(OutGame.Exist)
            // {
            //     var camera = OutGame.Instance.cameraManager.Camera;
            //     var pos = camera.WorldToScreenPoint(_target.position);
            //     return pos;
            // }
            return Vector3.zero;
        }

        private void CalcRotate()
        {
            var pos = _hand.position;
            var angle = 0;
            if (pos.y <= 200)
            {
                angle = 70;
                var hw = Screen.width * 0.5f;
                if (pos.x > hw)
                    angle *= -1;
            }
            _hand.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            
        }

        private void OnEnable()
        {
            var obsBeginMove = MessageBroker.Default.Receive<ItemBeginMove>().AsUnitObservable();
            var obsOnTheMove = MessageBroker.Default.Receive<ItemOnTheMove>().AsUnitObservable();
            var obsViewPush = MessageBroker.Default.Receive<UIViewPush>().AsUnitObservable();
            var obsViewPop = MessageBroker.Default.Receive<UIViewPop>().AsUnitObservable();

            obsBeginMove.Merge(obsOnTheMove, obsViewPush, obsViewPop)
                .TakeUntilDisable(this)
                .Subscribe(_ => Despawn()).AddTo(this);
        }

        public static bool IsShow => _instance != null;
        public static string ShowId => _instance?._id ?? string.Empty;

        public static void Spawn([NotNull]Transform trans, string id = null)
        {
            if (_instance == null)
            {
                var prefab = Resources.Load<TutorialHandClick>("Tutorial/TutorialHandClick");
                if (prefab != null)
                {
                    _instance = LeanPool.Spawn(prefab);
                }
            }
            _instance?.SetClick(trans, id);
        }

        public static void Despawn()
        {
            if (_instance != null)
            {
                LeanPool.Despawn(_instance);
                _instance = null;
            }
        }
    }
}