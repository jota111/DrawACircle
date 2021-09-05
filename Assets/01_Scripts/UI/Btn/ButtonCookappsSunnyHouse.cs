/*
* Copyright (c) CookApps.
* 이진호(jhlee8@cookapps.com)
*/

using SH.Util.UniRx;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace SH.UI.Btn
{
    /// <summary>
    /// 쿡앱스의 안드로이드 써니하우스 이동
    /// </summary>
    [RequireComponent(typeof(Button))]
    public class ButtonCookappsSunnyHouse : MonoBehaviour
    {
        private void Start()
        {
            GetComponent<Button>().OnClickSoundAsObservable()
                .Subscribe(_ => OnClick())
                .AddTo(this);
        }

        private void OnClick()
        {
            var url = $"https://play.google.com/store/apps/details?id=com.cookapps.sunnyhouse";
            Application.OpenURL(url);
        }
    }
}