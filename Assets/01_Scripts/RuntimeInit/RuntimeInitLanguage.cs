/*
* Copyright (c) CookApps.
* 이진호(jhlee8@cookapps.com)
*/

using I2.Loc;
using UnityEngine;

namespace SH.RuntimeInit
{
    public static class RuntimeInitLanguage
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void RuntimeInit()
        {
            var systemLanguage = Application.systemLanguage;
            PersistentStorage.DeleteSetting("I2 Language");
            
            // 기본언어로 설정
            //var systemLanguage = Application.systemLanguage;
            // var language = LanguageCode(systemLanguage);
            // LocalizationManager.CurrentLanguageCode = language;
            //
            // //---------------------------------------------------------------------------
            // string LanguageCode(SystemLanguage code) =>
            //     code switch
            //     {
            //         SystemLanguage.Korean => "ko",
            //         _ => "en"
            //     };
        }
    }
}