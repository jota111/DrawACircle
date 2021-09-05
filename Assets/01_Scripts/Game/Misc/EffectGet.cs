/*
* Copyright (c) CookApps.
* 이진호(jhlee8@cookapps.com)
*/

using System;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Lean.Pool;
using SH.Constant;
using SH.Game.Manager;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace SH.Game.Misc
{
    public class EffectGet : MonoBehaviour
    {
        public const float TotalDuration = 0.8f;

        [SerializeField, Required, ChildGameObjectsOnly]
        private Image _image;

        private Transform _transform;

        private void Awake()
        {
            _transform = transform;
        }

        private void SetItem(Sprite sprite)
        {
            var rectTransform = _image.GetComponent<RectTransform>();
            _image.sprite = sprite;
            _image.SetNativeSize();
            _image.color = Color.white;
            if (sprite != null)
            {
                var size = sprite.rect.size;
                rectTransform.pivot = sprite.pivot / size;
                   
            }
            rectTransform.anchoredPosition = Vector2.zero;
        }

#pragma warning disable CS4014
        
        public async UniTask SetEffect(Sprite item, Vector3 fromPos, Transform target, bool targetScale = true)
        {
            SetItem(item);
            _transform.position = fromPos;
            _transform.localScale = Vector3.one;
            var targetType = Invalidate.GetTargetType(item);
            await StartSetEffect(target, targetScale, targetType);
        }

        public async UniTask AsyncSetEffect(Sprite item, Vector3 fromPos, Transform target, Invalidate.TargetType targetType, bool targetScale = true)
        {
            SetItem(item);
            _transform.position = fromPos;
            _transform.localScale = Vector3.one;
            await StartSetEffect(target, targetScale, targetType);
        }

        private async Task StartSetEffect(Transform target, bool targetScale, Invalidate.TargetType targetType)
        {
            Invalidate.DelayedPublish(targetType, false);

            var ct = this.GetCancellationTokenOnDestroy();
            await UniTask.Delay(TimeSpan.FromSeconds(0.1f), cancellationToken: ct);
            await _transform.DOScale(1.15f, 0.15f).WithCancellation(ct);
            await UniTask.Delay(TimeSpan.FromSeconds(0.1f), cancellationToken: ct);
            await UniTask.WhenAll(
                _transform.DOMove(target.position, 0.5f).SetEase(Ease.InOutSine).WithCancellation(ct),
                _transform.DOScale(Vector3.one * 0.5f, 0.5f).SetEase(Ease.InBack).WithCancellation(ct)
            );

            LeanPool.Despawn(this);

            Invalidate.DelayedPublish(targetType, true);

            if (targetScale)
            {
                target.DOScale(1.2f, 0.15f).SetLoops(2, LoopType.Yoyo)
                    .OnComplete(() => target.localScale = Vector3.one);
            }
        }
        
#pragma warning restore CS4014
    }
}