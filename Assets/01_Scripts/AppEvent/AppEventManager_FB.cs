using System;
using System.Collections;
using System.Collections.Generic;
using Facebook.Unity;
using UnityEngine;


namespace SH.AppEvent
{
    public static partial class AppEventManager
    {
        public static void LogAppEvent_FB(string logEvent, Dictionary<string, object> parameters = null, float? valueToSum = null)
        {
            if ((Application.platform == RuntimePlatform.Android)
                || (Application.platform == RuntimePlatform.IPhonePlayer))
            {
                try
                {
                    FB.LogAppEvent(
                        logEvent,
                        valueToSum,
                        parameters
                    );
                }
                catch (Exception ex)
                {
                    
                }
            }
        }
    }
}
