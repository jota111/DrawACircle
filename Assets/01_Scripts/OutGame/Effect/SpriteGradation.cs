using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

namespace OutGameCore.Effect
{
    [RequireComponent(typeof(SpriteMask))]
    public class SpriteGradation : MonoBehaviour
    {
        [SerializeField, Required] private Transform effectPos;
        [SerializeField, Required] private SpriteMask _spriteMask;
        [SerializeField, Required] private ParticleSystem _particleSystem;
        [SerializeField] private float localValue = 3.4f;
        private ParticleSystem.ShapeModule _shapeModule;

        void Awake()
        {
            _shapeModule = _particleSystem.shape;
        }

        public async UniTask StartGradation_Show(SpriteRenderer spriteRenderer, GradationType gradationType, float duration, Action onComplete = null)
        {
            spriteRenderer.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
            SetPos(spriteRenderer, gradationType);
            transform.localScale = GetFirstScale(spriteRenderer, gradationType);
            effectPos.localEulerAngles = GetLocalAngles(gradationType);
            SetParticleRadius(spriteRenderer.size, gradationType);
            var finalScale = GetFinalScale(spriteRenderer, gradationType);
            await transform.DOScale(finalScale, duration).SetEase(Ease.Linear);
            onComplete?.Invoke();
            spriteRenderer.maskInteraction = SpriteMaskInteraction.None;
            OutGame.Instance.gamePools.Despawn(transform);
        }

        public async UniTask StartGradation_Hide(SpriteRenderer spriteRenderer, GradationType gradationType, float duration, Action onComplete = null)
        {
            spriteRenderer.maskInteraction = SpriteMaskInteraction.VisibleOutsideMask;
            SetPos(spriteRenderer, gradationType);
            transform.localScale = GetFirstScale(spriteRenderer, gradationType);
            effectPos.localEulerAngles = GetLocalAngles(gradationType);
            SetParticleRadius(spriteRenderer.size, gradationType);
            var finalScale =  GetFinalScale(spriteRenderer, gradationType);
            await transform.DOScale(finalScale, duration).SetEase(Ease.Linear);
            onComplete?.Invoke();
            spriteRenderer.maskInteraction = SpriteMaskInteraction.None;
            OutGame.Instance.gamePools.Despawn(transform);
        }

        private void SetPos(SpriteRenderer spriteRenderer, GradationType gradationType)
        {
            var pos = spriteRenderer.bounds.center; //spriteRenderer.transform.position;
            if (gradationType.HasFlag(GradationType.Up))
                pos += new Vector3(0, spriteRenderer.size.y / 2f);
            else if (gradationType.HasFlag(GradationType.Down))
                pos -= new Vector3(0, spriteRenderer.size.y / 2f);

            if (gradationType.HasFlag(GradationType.Left))
                pos -= new Vector3(spriteRenderer.size.x / 2f, 0);
            else if (gradationType.HasFlag(GradationType.Right))
                pos += new Vector3(spriteRenderer.size.x / 2f, 0);

            transform.position = pos;
        }

        private void SetParticleRadius(Vector2 size, GradationType gradationType)
        {
            var first = size * localValue;
            if (gradationType.HasFlag(GradationType.Down) || gradationType.HasFlag(GradationType.Up))
            {
                _shapeModule.radius = 0.5f * first.x / 2f;
            }
            else if (gradationType.HasFlag(GradationType.Right) || gradationType.HasFlag(GradationType.Left))
            {
                _shapeModule.radius = 0.5f * first.y / 2f;
            }
        }

        private Vector3 GetFirstScale(SpriteRenderer renderer, GradationType gradationType)
        {
            Vector3 currentScale = renderer.size;
            if (renderer.transform.localScale.x == -1)
                currentScale = new Vector3(-currentScale.x, currentScale.y, currentScale.z);
            var first = currentScale * localValue;
            if (gradationType.HasFlag(GradationType.Down) || gradationType.HasFlag(GradationType.Up))
            {
                first = new Vector3(first.x / 2f, 0, 1f);
                if (gradationType.HasFlag(GradationType.Right) || gradationType.HasFlag(GradationType.Left))
                    first = new Vector3(0, 0, 1f);
            }
            else if (gradationType.HasFlag(GradationType.Right) || gradationType.HasFlag(GradationType.Left))
            {
                first = new Vector3(0, first.y / 2f, 1f);
                if (gradationType.HasFlag(GradationType.Down) || gradationType.HasFlag(GradationType.Up))
                    first = new Vector3(0, 0, 1f);
            }

            return first;
        }

        private Vector3 GetFinalScale(SpriteRenderer renderer, GradationType gradationType)
        {
            Vector3 size = renderer.size;
            if (renderer.transform.localScale.x == -1)
                size = new Vector3(-size.x, size.y, size.z);
            var final = new Vector3(size.x * localValue, size.y * localValue, 1);
            if ((gradationType.HasFlag(GradationType.Down) || gradationType.HasFlag(GradationType.Up)) &&
                (gradationType.HasFlag(GradationType.Right) || gradationType.HasFlag(GradationType.Left)))
            {
                
            }else if(gradationType.HasFlag(GradationType.Down) || gradationType.HasFlag(GradationType.Up))
            {
                final = new Vector3(final.x / 2f, final.y, 1f);
            }
            else if (gradationType.HasFlag(GradationType.Right) || gradationType.HasFlag(GradationType.Left))
            {
                final = new Vector3(final.x, final.y / 2f, 1f);
            }

            return final;
        }

        private Vector3 GetLocalAngles(GradationType gradationType)
        {
            if (gradationType.HasFlag(GradationType.Right))
            {
                return new Vector3(0,0,-90);
            }
            else if(gradationType.HasFlag(GradationType.Left))
            {
                return new Vector3(0,0,90);
            }
            else if(gradationType.HasFlag(GradationType.Down))
            {
                return new Vector3(0,0,180);
            }
            else
            {
                return Vector3.zero;
            }
        }

        [Flags]
        public enum GradationType
        {
            Up = 1 << 0,
            Down = 1 << 1,
            Left = 1 << 2,
            Right = 1 << 3,
        }
    }
}