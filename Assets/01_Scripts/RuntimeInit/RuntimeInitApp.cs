/*
* Copyright (c) CookApps.
* 이진호(jhlee8@cookapps.com)
*/

using DG.Tweening;
using UnityEngine;

namespace SH.RuntimeInit
{
    public class RuntimeInitApp
    {
        [RuntimeInitializeOnLoadMethod]
        private static void RuntimeInit()
        {
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = 60;
        }
    }
}