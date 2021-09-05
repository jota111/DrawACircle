using System.Collections;
using System.Collections.Generic;
using Doozy.Engine.UI;
using SH.AppEvent;
using SH.Constant;
using SH.Game.Manager;
using SH.UI;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace SH.Game.Shop
{
    public class ButtonBuyEnergy : MonoBehaviour
    {
        public void Start()
        {
            GetComponent<Button>()
                .OnClickAsObservable()
                .Subscribe(_ => OnClick()).AddTo(this);
        }

        private void OnClick()
        {
            GameSoundManager.Instance.PlaySfx(SFX.sh_common_click);
            if (UIViewManager.CheckView(UIViewName.Shop))
            {
                UIView_Shop.Instance.JumpToData("Energy", 0.5f);
                return;
            }

            if (UIViewManager.CheckView(UIViewName.BuyEnergy))
                return;
            UIViewManager.Instance.HideAll();
            UIView.ShowView(UIViewName.BuyEnergy);
        }
    }
}
