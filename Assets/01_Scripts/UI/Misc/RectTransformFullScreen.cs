/*
* Copyright (c) CookApps.
* 이진호(jhlee8@cookapps.com)
*/

using UnityEngine;

namespace SH.UI.Misc
{
    /// <summary>
    /// Screen space 화면 기준 FullScreen
    /// </summary>
    [RequireComponent(typeof(RectTransform))]
    public class RectTransformFullScreen : MonoBehaviour
    {
        private RectTransform _rectTransform;
        private RectTransform _parentRectTransform;

        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
            _parentRectTransform = _rectTransform.parent as RectTransform;
        }

        private void LateUpdate()
        {
            var w = Screen.width;
            var h = Screen.height;

            RectTransformUtility.ScreenPointToLocalPointInRectangle(_parentRectTransform, new Vector2(w * 0.5f, h * 0.5f), null, out var localPoint);
            _rectTransform.anchoredPosition  = localPoint;
            // _rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, w);
            // _rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, h);
        }
    }
}