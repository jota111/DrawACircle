/*
* Copyright (c) CookApps.
* 이진호(jhlee8@cookapps.com)
*/

using Doozy.Engine.UI;
using Doozy.Engine.UI.Internal;
using SH.AppEvent;
using SH.Constant;
using SH.Game.Manager;
using SH.Util;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.UI;

namespace SH.UI.Btn
{
    public sealed class PointerClickShowView : MonoBehaviour
    {
        [SerializeField]
        private UIViewCategoryName _name;
        
        private void Start()
        {
            gameObject.GetComponentOrAdd<ObservableEventTrigger>()
                .OnPointerClickAsObservable()
                .Subscribe(_ => OnClick())
                .AddTo(this);
        }

        private void OnClick()
        {
            var selectable = gameObject.GetComponent<Selectable>();
            if (selectable != null && !selectable.interactable)
                return;
            
            GameSoundManager.Instance.PlaySfx(SFX.sh_common_click);
            if (UIViewManager.CheckView(_name.Name))
                return;
            if (_name.Name.Equals(UIViewName.PlayerLevel))
            {
                UIViewManager.Instance.HideAll();
            }
            UIView.ShowView(_name.Category, _name.Name);
        }
    }
}