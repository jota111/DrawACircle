/*
* Copyright (c) CookApps.
* 이진호(jhlee8@cookapps.com)
*/

using Sirenix.OdinInspector;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace SH.UI.View.ExitGame
{
    public class UIViewExitGame : UIViewBase
    {
        [SerializeField, Required, ChildGameObjectsOnly]
        private Button _buttonYes;

        private void Start()
        {
            _buttonYes.OnClickAsObservable()
                .Subscribe(_ => Application.Quit())
                .AddTo(this);
        }
    }
}