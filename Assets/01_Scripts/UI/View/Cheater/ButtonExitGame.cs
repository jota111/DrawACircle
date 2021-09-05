/*
* Copyright (c) CookApps.
* 이진호(jhlee8@cookapps.com)
*/

using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace SH.UI.View.Cheater
{
    [RequireComponent(typeof(Button))]
    public class ButtonExitGame : MonoBehaviour
    {
        private void Start()
        {
            GetComponent<Button>().OnClickAsObservable()
                .Subscribe(_ => Application.Quit())
                .AddTo(this);
        }
    }
}