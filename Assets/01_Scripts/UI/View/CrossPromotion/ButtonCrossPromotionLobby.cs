/*
* Copyright (c) CookApps.
* 이진호(jhlee8@cookapps.com)
*/

using System;
using Cysharp.Threading.Tasks;
using Doozy.Engine.UI;
using SH.CrossPromotion;
using SH.Game.Manager;
using SH.Util.UniRx;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace SH.UI.CrossPromotion
{
    [RequireComponent(typeof(Button))]
    public class ButtonCrossPromotionLobby : MonoBehaviour
    {
        private Button _button;
        
        [Inject]
        private void Construct(EnergyManager energyManager)
        {
            _button = GetComponent<Button>();

            _button.OnClickSoundAsObservable()
                .ThrottleFirst(TimeSpan.FromSeconds(0.5))
                .Subscribe(_ => OnClick())
                .AddTo(this);

            energyManager.Energy.CombineLatest(CrossPromotionManager.Instance.Data, (e, data) => (e, data))
                .Subscribe(x => Set(x.e, x.data))
                .AddTo(this);
        }

        private void OnClick()
        {
            if (!UIView.IsViewVisible(UIView.DefaultViewCategory, UIViewName.CrossPromotion))
            {
                UIView.ShowView(UIViewName.CrossPromotion);
            }
        }

        private void Set(int energy, CrossPromotionData data)
        {
            var isOn = (energy == 0) && data?.Data?.show_lobby_icon_on == 1;
            _button.interactable = isOn;
            _button.image.enabled = isOn;

            if (data == null)
                return;
            
            var textureIcon = data.Icon;
            if (isOn && textureIcon != _button.image.sprite?.texture)
            {
                var rect = new Rect(0, 0, textureIcon.width, textureIcon.height);
                _button.image.sprite = Sprite.Create(textureIcon, rect, new Vector2(0.5f, 0.5f));
                _button.image.SetNativeSize();
            }
        }
    }
}