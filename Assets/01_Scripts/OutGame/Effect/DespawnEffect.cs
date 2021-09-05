using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace OutGameCore.Effect
{
    public class DespawnEffect : MonoBehaviour
    {
        private ParticleSystem particleSystem;
        private Animator animator;
        private List<ParticleSystemRenderer> _particleSystems;

        [SerializeField] private Type type;


        void OnEnable()
        {
            particleSystem = transform.GetComponent<ParticleSystem>() == null
                ? transform.GetComponentInChildren<ParticleSystem>()
                : transform.GetComponent<ParticleSystem>();
            animator = transform.GetComponent<Animator>() == null ? transform.GetComponentInChildren<Animator>() : transform.GetComponent<Animator>();

            if (particleSystem != null) type |= Type.Effect;
            if (animator != null) type |= Type.Animation;

            if (type == Type.None) return;
            StartDespawnTimer();
        }

        public void SetOrder(int sortingOrder)
        {
            if (_particleSystems == null)
            {
                _particleSystems = new List<ParticleSystemRenderer>();
                _particleSystems = transform.GetComponentsInChildren<ParticleSystemRenderer>().ToList();
            }

            foreach (var particle in _particleSystems)
            {
                particle.sortingOrder = sortingOrder;
            }
        }

        #region 디스폰

        private IDisposable despawnTimer = Disposable.Empty;

        public void StartDespawnTimer()
        {
            despawnTimer?.Dispose();
            despawnTimer = Disposable.Empty;
            despawnTimer = this.UpdateAsObservable().Select(x => CheckCondition() == false).DistinctUntilChanged()
                .ThrottleFrame(5).Subscribe(x =>
                {
                    if (CheckCondition())
                    {
                        DespawnSelf();
                    }
                }).AddTo(gameObject);
        }

        private bool CheckCondition()
        {
            bool checker = true;
            if (type.HasFlag(Type.Effect))
                checker &= particleSystem.IsAlive() == false;
            if (type.HasFlag(Type.Animation))
                checker &= animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1 && !animator.IsInTransition(0);
            return checker;
        }

        public void DespawnSelf()
        {
            //도착
            if (gameObject.activeSelf != true) return;
            if (OutGame.Instance.gamePools.IsSpawned(transform))
            {
                transform.SetParent(null);
                OutGame.Instance.gamePools.Despawn(transform);
            }
            else
            {
                gameObject.SetActive(false);
            }
        }

        private void OnDisable()
        {
            if (despawnTimer != null)
            {
                despawnTimer.Dispose();
                despawnTimer = null;
            }
        }

        #endregion

        [Flags]
        public enum Type
        {
            None = 0, // 0000
            Effect = 1 << 1, // 0001
            Animation = 1 << 2, // 0010
        }
    }
}