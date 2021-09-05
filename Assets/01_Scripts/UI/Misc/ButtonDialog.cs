/*
* Copyright (c) CookApps.
* 이진호(jhlee8@cookapps.com)
*/

using System;
using SH.UI.View.Dialog;
using SH.Util.UniRx;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace SH.UI.Misc
{
    [RequireComponent(typeof(Button))]
    public class ButtonDialog : MonoBehaviour
    {
        [SerializeField] private string _termTitle;
        [SerializeField] private string _termMsg;

        private void Start()
        {
            GetComponent<Button>()
                .OnClickSoundAsObservable()
                .ThrottleFirst(TimeSpan.FromSeconds(0.5f))
                .Subscribe(_ => OnClick())
                .AddTo(this);
        }

        private void OnClick()
        {
            UIViewDialog.Instance?.DialogForget(_termMsg, _termTitle);
        }
    }
}