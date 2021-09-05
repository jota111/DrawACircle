using System;
using System.Collections;
using System.Collections.Generic;
using Firebase;
using Firebase.Analytics;
using Firebase.Extensions;
using OutGameCore;
using SH.Constant;
using UnityEngine;


namespace SH.AppEvent
{
    public static partial class AppEventManager
    {
        private static bool isInitialized_Firebase = false;
        

        public static void Init_Firebase()
        {
            // FirebaseAnalytics.SetUserId("uber_user_510");
            FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
            {
                if (task.Result == DependencyStatus.Available)
                {
                    FirebaseAnalytics.SetAnalyticsCollectionEnabled(true);
                    
                    FirebaseApp app = Firebase.FirebaseApp.DefaultInstance; //Crashlytics 사용설정
                    SetCommonParameters_Firebase();
                    LogAppEvent_Firebase(FirebaseAnalytics.EventLogin);
                    isInitialized_Firebase = true;
                }
                else
                    GameUtils.Error(
                        "Could not resolve all Firebase dependencies: " + task.Result);
            });
        }

        public static void SetCommonParameters_Firebase()
        {
            FirebaseAnalytics.SetUserProperty("LANGUAGE", Application.systemLanguage.ToString());
            FirebaseAnalytics.SetUserProperty("APP_VERSION_CODE", StaticSet.Version);
            FirebaseAnalytics.SetUserProperty("PLATFORM", Application.platform.ToString());
        }

        public static void LogAppEvent_Firebase(string eventName, Dictionary<string, object> parameters = null)
        {
            try
            {
                if ((Application.platform == RuntimePlatform.Android)
                    || (Application.platform == RuntimePlatform.IPhonePlayer))
                {
                    if (parameters == null)
                    {
                        FirebaseAnalytics.LogEvent(eventName);
                    }
                    else
                    {
                        List<Parameter> eventParams = new List<Parameter>();
                        foreach (var i in parameters)
                        {
                            try
                            {
                                eventParams.Add(new Parameter(i.Key, i.Value.ToString()));
                            }
                            catch (Exception ex)
                            {
                                eventParams.Add(new Parameter(i.Key, ""));
                            }
                        }

                        FirebaseAnalytics.LogEvent(eventName, eventParams.ToArray());
                    }
                }
            }
            catch (Exception ex)
            {
                GameUtils.LogException(ex);
            }
        }
    }
}