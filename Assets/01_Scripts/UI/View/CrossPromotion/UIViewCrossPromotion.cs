/*
* Copyright (c) CookApps.
* 이진호(jhlee8@cookapps.com)
*/

using System;
using SH.CrossPromotion;
using SH.UI.View;
using SH.Util.UniRx;
using Sirenix.OdinInspector;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace SH.UI.CrossPromotion
{
    public class UIViewCrossPromotion : UIViewBase
    {
        [SerializeField, Required, ChildGameObjectsOnly]
        private Button _buttonMove;

        [SerializeField, Required, ChildGameObjectsOnly]
        private Image _image;

        private void Start()
        {
            _buttonMove.OnClickSoundAsObservable()
                .ThrottleFirst(TimeSpan.FromSeconds(0.5))
                .Subscribe(_ => OnClick())
                .AddTo(this);

            CrossPromotionManager.Instance.Data.Where(x => x != null)
                .Subscribe(data => SetBanner(data.Banner))
                .AddTo(this);
        }

        private void SetBanner(Texture2D texture)
        {
            if (texture != null)
            {
                var rect = new Rect(0, 0, texture.width, texture.height);
                _image.sprite = Sprite.Create(texture, rect, new Vector2(0.5f, 0.5f));
                _image.SetNativeSize();
            }
        }

        private void OnClick()
        {
            var data = CrossPromotionManager.Instance.Data.Value;
            var url = data?.Data?.url; 
            if (!string.IsNullOrEmpty(url))
            {
                Application.OpenURL(url);
            }
            
            Hide();
        }
    }
}