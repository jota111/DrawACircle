/*
* Copyright (c) CookApps.
* 이진호(jhlee8@cookapps.com)
*/

using UnityEngine;

namespace SH.RuntimeInit
{
    public static class RuntimeInitCalendarCrashFix
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void RuntimeInit()
        {
            var strTwoLetterISOLanguage = System.Threading.Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName;
            if (strTwoLetterISOLanguage == "ar")
            {
                new System.Globalization.UmAlQuraCalendar();
            }

            if (strTwoLetterISOLanguage == "th")
            {
                new System.Globalization.ThaiBuddhistCalendar();
            }
        }
    }
}