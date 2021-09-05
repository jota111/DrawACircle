/*
* Copyright (c) CookApps.
* 이진호(jhlee8@cookapps.com)
*/

using OutGameCore;
using SH.Constant;
using SH.Util;
using Sirenix.OdinInspector;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Zenject;

namespace SH.UI.View.Dialog
{
    public class UIViewDialogPlatformLoginSuccess : UIViewBase
    {
        private UIViewManager _uiViewManager;
        
        [SerializeField, Required, ChildGameObjectsOnly]
        private Button _buttonOK;

        [SerializeField, Required, ChildGameObjectsOnly]
        private Text _textMsg;

        [SerializeField, Required, ChildGameObjectsOnly]
        private Text _textTitle;
        
        [SerializeField, Required, ChildGameObjectsOnly]
        private Text _textOK;

        [Inject]
        private void Construct(UIViewManager uiViewManager)
        {
            _uiViewManager = uiViewManager;
        }

        private void Start()
        {
            _buttonOK.OnClickAsObservable()
                .Subscribe(async _ =>
                {
                    await OutGame.DestroySelf();
                    SceneLoad.LoadScene(SceneType.SceneTitle);
                })
                .AddTo(this);
        }

        public void Show(string termMsg)
        {
            _uiViewManager.HideAll();
            _textMsg.text = GameUtils.I2Format(termMsg);
            _textTitle.text = GameUtils.I2Format("Popup_title_loginsuccess");
            _textOK.text = GameUtils.I2Format("Dialog_OK");
            Show();
        }
    }
}