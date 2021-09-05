/*
* Copyright (c) CookApps.
* 이진호(jhlee8@cookapps.com)
*/

using Lean.Pool;
using UnityEngine;
using UnityEngine.Assertions;

namespace SH.Util
{
    public sealed class LeanPoolDespawnEffect : MonoBehaviour
    {
        private ParticleSystem _particle;
        private Animator _animator;

        private void Awake()
        {
            _particle = GetComponent<ParticleSystem>();
            _animator = GetComponent<Animator>();
            Assert.IsTrue(_particle != null | _animator != null);
        }

        private bool IsEnd()
        {
            if (_particle)
            {
                return _particle.time >= _particle.main.duration;
            }

            if (_animator)
            {
                return _animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1 && !_animator.IsInTransition(0);
            }

            return true;
        }

        private void Update()
        {
            if (IsEnd())
            {
                Despawn();
            }
        }

        private void Despawn()
        {
            LeanPool.Despawn(gameObject);
        }
    }
}