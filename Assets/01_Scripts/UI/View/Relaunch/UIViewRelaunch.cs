/*
* Copyright (c) CookApps.
* 이진호(jhlee8@cookapps.com)
*/

using Doozy.Engine.UI;
using SH.Util;
using SH.Util.UniRx;
using Sirenix.OdinInspector;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace SH.UI.View.Relaunch
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(UIView))]
    public class UIViewRelaunch : MonoBehaviour
    {
        [SerializeField, Required, ChildGameObjectsOnly]
        private Text _textCode;

        [SerializeField, Required, ChildGameObjectsOnly]
        private Button _buttonCopy;

        private string _code;

        private void Awake()
        {
            
        }

        private void Start()
        {
            _buttonCopy.OnClickSoundAsObservable()
                .Subscribe(_ => OnClickCopy())
                .AddTo(this);
        }

        public void Show(string code)
        {
            _code = code;
            _textCode.text = code;
            GetComponent<UIView>().Show();
        }

        private void OnClickCopy()
        {
            GUIUtility.systemCopyBuffer = _code;
            ToastHelper.Show("Popup_text_copied");
        }
    }
}