/*
* Copyright (c) CookApps.
* 이진호(jhlee8@cookapps.com)
*/

using Coffee.UIEffects;
using DG.Tweening;
using SH.Game.Manager;
using Sirenix.OdinInspector;
using UniRx;
using UnityEngine;
using Zenject;

namespace SH.UI.Level
{
    public class LevelUpEffect : MonoBehaviour
    {
        [SerializeField, Required, ChildGameObjectsOnly]
        private GameObject _goEffect;

        [SerializeField]
        private Transform _trIcon;

        [SerializeField]
        private UIShiny _uiShiny;
        
        [Inject]
        private void Construct(LevelManager levelManager)
        {
            levelManager.PossibleLevelUp.Subscribe(PossibleLevelUp)
                .AddTo(this);
        }
        
        private void PossibleLevelUp(bool levelup)
        {
            _goEffect.SetActive(levelup);
            if (_uiShiny != null)
            {
                _uiShiny.enabled = levelup;
                _uiShiny.effectFactor = 0;   
            }

            if (_trIcon != null)
            {
                _trIcon.DOKill();
                _trIcon.localScale = Vector3.one;
                if (levelup)
                {
                    _trIcon.DOScale(1.075f, 0.75f).SetLoops(-1, LoopType.Yoyo);
                }   
            }
        }

        private void OnDisable()
        {
            if (_trIcon != null)
            {
                _trIcon.DOKill();   
            }
        }
    }
}