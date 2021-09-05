using System;
using System.Collections;
using System.Collections.Generic;
using OutGameCore;
using SH.Constant;
using SH.Data;
using SH.Game.Manager;
using SH.Setting;
using SH.UI.View;
using Sirenix.OdinInspector;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace SH.UI.Monetization
{
    public class UIViewGetReward : UIViewBase
    {
        public static UIViewGetReward Instance { get; private set; }
        [SerializeField] private GameObject[] goObjects;
        [SerializeField] private Image[] images;
        [SerializeField] private Text[] texts;
        [SerializeField] private Button Btn_Click;
        private DataRewardsInfo _currentRewardsInfo;
        private GetEffectManager _getEffectManager;
        private IDisposable timer;

        [Inject]
        public void Construct(GetEffectManager getEffectManager)
        {
            Instance = this;
            _getEffectManager = getEffectManager;
        }

        public void SetView(DataRewardsInfo rewardsInfo)
        {
            _currentRewardsInfo = rewardsInfo;
            SetItems();
            Show();
            StartTimer();
        }

        private void SetItems()
        {
            for (int i = 0; i < goObjects.Length; i++)
            {
                if (i >= _currentRewardsInfo.Count)
                {
                    goObjects[i].gameObject.SetActive(false);
                }
                else
                {
                    goObjects[i].gameObject.SetActive(true);
                    images[i].sprite = _currentRewardsInfo[i].Item1;
                    images[i].SetNativeSize();
                    texts[i].text = $"x{_currentRewardsInfo[i].Item2}";
                }
            }
        }

        private void StartTimer()
        {
            timer?.Dispose();
            timer = Observable.Timer(TimeSpan.FromSeconds(1.5)).Subscribe(_ => { StartEffect(); }).AddTo(this);
        }

        private void StartEffect()
        {
            Hide();
            for (int i = 0; i < _currentRewardsInfo.Count; i++)
            {
                _getEffectManager.Get(_currentRewardsInfo[i].Item1, images[i].transform.position);
            }
        }

        [Button]
        public void Test_Show()
        {
            var random = GameUtils.RandomRange(0, 2);
            switch (random)
            {
                case 0:
                    UIViewGetReward.Instance.SetView(new DataRewardsInfo("Coin", 100));
                    break;
                case 1:
                    UIViewGetReward.Instance.SetView(new DataRewardsInfo("Energy", 100));
                    break;
            }
        }
    }

    public class DataRewardsInfo
    {
        private List<Sprite> _dataSprites = new List<Sprite>();
        private List<int> _counts = new List<int>();

        public (Sprite, int) this[int index]
        {
            get
            {
                Sprite _sprite = null;
                int _count = 0;

                if (_dataSprites.Count > index)
                {
                    _sprite = _dataSprites[index];
                }

                if (_sprite == null) GameUtils.Error($"DataRewardsInfo {index} 데이터 Sprite null임!");
                if (_counts.Count > index)
                {
                    _count = _counts[index];
                }

                return (_sprite, _count);
            }
        }

        public int Count => _counts.Count;

        public DataRewardsInfo()
        {
        }

        public DataRewardsInfo(string item, int count)
        {
            Sprite sprite = null;
            if (item.Equals(Currencies.Diamond.ToString()))
            {
                sprite = DataCurrencies.GetDiamondShopIcon(count);
                Add(sprite, count);
            }
            else if (item.Equals(Currencies.Coin.ToString()))
            {
                sprite = DataCurrencies.GetCoinShopIcon(count);
                Add(sprite, count);
            }
            else if (item.Equals(Currencies.Energy.ToString()))
            {
                sprite = DataCurrencies.GetEnergyIcon(count);
                Add(sprite, count);
            }
        }

        public DataRewardsInfo(Sprite sprite, int count)
        {
            Add(sprite, count);
        }

        public DataRewardsInfo(List<Sprite> sprites, List<int> counts)
        {
            if (sprites.Count != counts.Count)
                GameUtils.Error($"DataRewardsInfo 수량이 안맞음! {sprites.Count} != {counts.Count}");
            for (int i = 0; i < sprites.Count; i++)
            {
                Add(sprites[i], counts[i]);
            }
        }

        public void Add(Sprite sprite, int count)
        {
            _dataSprites.Add(sprite);
            _counts.Add(count);
        }
    }
}