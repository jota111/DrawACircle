/*
* Copyright (c) CookApps.
* 이진호(jhlee8@cookapps.com)
*/

using System;
using DG.Tweening;
using SH.Game.UserData;
using Sirenix.OdinInspector;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace SH.Title
{
    [RequireComponent(typeof(Button))]
    public class ViewUserData : MonoBehaviour
    {
        [SerializeField, Required, ChildGameObjectsOnly]
        private Text _textQuest;
        [SerializeField, Required, ChildGameObjectsOnly]
        private Text _textGold;
        [SerializeField, Required, ChildGameObjectsOnly]
        private Text _textJewel;
        [SerializeField, Required, ChildGameObjectsOnly]
        private Text _textXp;
        [SerializeField, Required, ChildGameObjectsOnly]
        private Text _textDate;
        [SerializeField, Required, ChildGameObjectsOnly]
        private Toggle _toggle;
        [SerializeField, Required, ChildGameObjectsOnly]
        private Image _imgDisable;
        [SerializeField, ChildGameObjectsOnly]
        private GameObject _goInitData;
        
        public UserData Data {get; private set; }
        public bool IsSelect => _toggle.isOn;

        public IObservable<bool> OnToggleValueChanged() => _toggle.OnValueChangedAsObservable();

        private void Awake()
        {
            if(_goInitData != null)
                _goInitData.SetActive(false);
        }

        private void Start()
        {
            GetComponent<Button>()?
                .OnClickAsObservable()
                .Subscribe(_ =>
                {
                    _toggle.isOn = !_toggle.isOn;
                }).AddTo(this);

            OnToggleValueChanged()
                .Subscribe(value => Select(value, 0.15f))
                .AddTo(this);

            Select(false, 0);
        }

        public void EnableInitData()
        {
            if(_goInitData != null)
                _goInitData.SetActive(true);
        }

        public void SetData(UserData data)
        {
            Data = data;
            _textDate.text = data.DateTimeSave.ToString("g");
            _textGold.text = data.Gold.Value.ToString();
            _textJewel.text = data.Jewel.Value.ToString();
            _textXp.text = data.Exp.Value.ToString();
        }

        private void Select(bool select, float duration)
        {
            transform.DOKill();
            transform.DOScale(select ? 1.0f : 0.97f, duration);

            _imgDisable.DOKill();
            _imgDisable.DOFade(select ? 0 : 25.0f / 255.0f, duration);
        }
    }
}