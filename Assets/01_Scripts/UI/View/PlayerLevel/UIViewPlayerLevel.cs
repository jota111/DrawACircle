/*
* Copyright (c) CookApps.
* 이진호(jhlee8@cookapps.com)
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using MoreLinq.Extensions;
using OutGameCore;
using SH.Constant;
using SH.Data;
using SH.Game.Manager;
using SH.Setting;
using Sirenix.OdinInspector;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace SH.UI.View.PlayerLevel
{
    public class UIViewPlayerLevel : UIViewBase
    {
        [SerializeField, Required, ChildGameObjectsOnly]
        private Slider _sliderExp;
        
        [SerializeField, Required, ChildGameObjectsOnly]
        private Text _textLevel;
        
        [SerializeField, Required, ChildGameObjectsOnly]
        private Text _textExp;
        
        [SerializeField, Required, ChildGameObjectsOnly]
        private Text _textDesc;
        
        [SerializeField, Required, ChildGameObjectsOnly]
        private Image[] _rewards;

        [SerializeField, Required, ChildGameObjectsOnly]
        private Button _buttonLevelUp;
        
        [SerializeField, Required]
        private Sprite _energyIcon;

        [InjectOptional(Id = ZenId.RectTransformEnergy)]
        private RectTransform _transformEnergy;

        private LevelManager _levelManager;

        private CancellationTokenSource _cts;
        private GameSoundManager _soundManager;
        private EnergyManager _energyManager;
        private GetEffectManager _getEffectManager;

        private float _itemRewardInitScale;


        [Inject]
        private void Construct(LevelManager levelManager, GameSoundManager soundManager, EnergyManager energyManager, GetEffectManager getEffectManager)
        {
            _soundManager = soundManager;
            _levelManager = levelManager;
            _energyManager = energyManager;
            _getEffectManager = getEffectManager;
            _buttonLevelUp.OnClickAsObservable()
                .Subscribe(_ => OnClickLevelUp())
                .AddTo(this);

            _itemRewardInitScale = _rewards.Length > 0 ? _rewards.First().transform.localScale.x : 1;
        }

        private void OnClickLevelUp()
        {
            if (!_levelManager.PossibleLevelUp.Value)
                return;

            if (!_levelManager.ReachLevel())
                return;
            
            //Energy 충전
            if (_energyManager.Energy.Value < EnergyManager.MaxEnergy)
            {
                var value = EnergyManager.MaxEnergy - _energyManager.Energy.Value;
                _energyManager.AddEnergy(value);
                // DataCurrencies.GetEnergyIcon(value)
                _getEffectManager.Get(_energyIcon, _buttonLevelUp.transform.position, _transformEnergy);
            }

            GameSoundManager.Instance.PlaySfx(SFX.sh_common_click);
            _soundManager.PlaySfx(SFX.sh_level_up);
            EffectReward();
            SetDataAsync().Forget();
        }

        protected override void OnStartShow()
        {
            base.OnStartShow();
            SetData();
        }

        protected override void OnStartHide()
        {
            base.OnStartHide();
            _cts?.Cancel();
            _cts = null;
        }

        private void SetData()
        {
            _buttonLevelUp.interactable = _levelManager.PossibleLevelUp.Value;
            _textLevel.text = _levelManager.Level.Value.ToString();
            _textExp.text = $"{_levelManager.Exp.Value - _levelManager.StartExp.Value}/{_levelManager.NextExp.Value - _levelManager.StartExp.Value}";
            _sliderExp.value = _levelManager.LevelUpPercent.Value;
            _rewards.ForEach(item => item.gameObject.SetActive(false));
            
            if (_levelManager.IsReachMaxLevel.Value)
            {
                ReachMaxLevel();
                return;
            }

            var data = DataLevel.GetDataNextLevel(_levelManager.Level.Value);
            var rewards = new List<string>();
            
            for (var i = 0; i < rewards.Count; i++)
            {
                _rewards[i].gameObject.SetActive(true);
                _rewards[i].sprite = AtlasSetting.Instance.GetOutGameIcon(rewards[i]);
                //DoTween 진행중 팝업이 닫히면 멈춰서 scale값이 0으로 고정되는 이슈
                _rewards[i].transform.localScale = Vector3.one * _itemRewardInitScale;
            }

            SetDesc();
        }

        private void EffectReward()
        {
            var rewards = _rewards.Where(item => item.gameObject.activeSelf).ToArray();
            rewards.ForEach(item => item.gameObject.SetActive(false));
        }

        private async UniTaskVoid SetDataAsync()
        {
            _cts = new CancellationTokenSource();
            var cts = _cts.Token;

            _buttonLevelUp.interactable = false;
            if(await UniTask.Delay(TimeSpan.FromSeconds(1), cancellationToken:cts).SuppressCancellationThrow())
                return;
            
            _textLevel.text = _levelManager.Level.Value.ToString();
            _textExp.text = $"{_levelManager.Exp.Value - _levelManager.StartExp.Value}/{_levelManager.NextExp.Value - _levelManager.StartExp.Value}";
            if (await _sliderExp.DOValue(_levelManager.LevelUpPercent.Value, 0.25f).ToUniTask(cancellationToken: cts).SuppressCancellationThrow())
                return;

            if (_levelManager.IsReachMaxLevel.Value)
            {
                ReachMaxLevel();
                return;
            }
            
            var data = DataLevel.GetDataNextLevel(_levelManager.Level.Value);
            var rewards = new List<string>();
            
            for (var i = 0; i < rewards.Count; i++)
            {
                _rewards[i].gameObject.SetActive(true);
                _rewards[i].sprite = AtlasSetting.Instance.GetOutGameIcon(rewards[i]);
                _rewards[i].gameObject.SetActive(true);
                _rewards[i].transform.localScale = Vector3.zero;
            }
            
            for (var i = 0; i < rewards.Count; i++)
            {
                var item = _rewards[i];
                if (await item.transform.DOScale(Vector3.one * _itemRewardInitScale, 0.2f).ToUniTask(cancellationToken: cts).SuppressCancellationThrow())
                    return;
            }

            _buttonLevelUp.interactable = _levelManager.PossibleLevelUp.Value;
            SetDesc();
        }

        private void SetDesc()
        {
            var term = _levelManager.PossibleLevelUp.Value ? "UIPlayerLevel_CanLevelUp" : "UIPlayerLevel_CanNotLevelUp";
            GameUtils.I2Format(_textDesc, term);
        }

        private void ReachMaxLevel()
        {
            // todo max level
        }
    }
}