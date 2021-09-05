using System.Collections;
using System.Collections.Generic;
using SH.Game.Manager;
using SH.UI.View;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace SH.Game.Settings
{
    public class UIViewTransferStart : UIViewBase
    {
        [SerializeField] private InputField Input_Code;
        [SerializeField] private Button Btn_Submit;

        [Inject] private GameSoundManager _gameSoundManager;

        private void Awake()
        {
            Btn_Submit.OnClickAsObservable().Subscribe(_ => { OnSubmitButtonClicked(); }).AddTo(this);
        }

        protected override void OnStartShow()
        {
            base.OnStartShow();
            Input_Code.text = "";
        }

        public void OnSubmitButtonClicked()
        {
            _gameSoundManager.PlaySfx(SFX.sh_common_click);
            
            var code = Input_Code.text;
            
        }
    }
}
