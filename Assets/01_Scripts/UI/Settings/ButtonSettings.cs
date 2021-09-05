using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Doozy.Engine.UI;
using SH.AppEvent;
using SH.Constant;
using SH.Game.Manager;
using SH.UI.Btn;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace SH.Game.Settings
{
    public class ButtonSettings : MonoBehaviour
    {
        public void Awake()
        {
            GetComponent<Button>()
                .OnClickAsObservable()
                .Subscribe(_ => OnClick()).AddTo(this);
        }

        private void OnClick()
        {
            GameSoundManager.Instance.PlaySfx(SFX.sh_common_click);
            UIView.ShowView("Settings");
        }
    }
}
