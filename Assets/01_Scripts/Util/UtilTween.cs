/*
* Copyright (c) CookApps.
* 이진호(jhlee8@cookapps.com)
*/

using DG.Tweening;
using UnityEngine;

namespace SH.Util
{
    public static class UtilTween
    {
        public static void ScaleOnOff(this Transform trans, bool on, float duration = 0.3f, float onScale = 1.0f)
        {
            trans.DOKill();
            var scale = on ? onScale : 0;
            trans.DOScale(scale, duration);
        }
    }
}