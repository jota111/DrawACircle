using System;
using System.Collections.Generic;
using Facebook.Unity;
using SH.Util;
using UnityEngine;
using Zenject;
using Cookapps.Analytics;
using Cysharp.Threading.Tasks;
using Firebase;
using OutGameCore;
using SH.Game.UserData;
using SH.Game.UserManager;
using UniRx;

namespace SH.AppEvent
{
    public static partial class AppEventManager
    {
        public const bool LogFB = false;
        public const bool LogCA = true;
        public const bool LogFirebase = true;

        static Dictionary<string, object> parameters = new Dictionary<string, object>();
        private static HashSet<FirstProgress> FirstProgressSend = null;

        [RuntimeInitializeOnLoadMethod]
        public static void RuntimeInit()
        {
#if UNITY_AWS
            var token = "fdef70eedbadef9ab2fb264732e37792";
#elif UNITY_ANDROID
            var token = "a8fcfe20c81c813377f9637e8870c81c";
#else
            var token = "a331c0e316833c3f98a8ab5f3dac5716";
#endif
            
            CAppEvent.Initialize(token, SystemInfo.deviceUniqueIdentifier);
            if (LogFirebase) Init_Firebase();

            InitializedTime = TimeUtil.Now;
            ReportProgress(FirstProgress.App_Launched).Forget();
            ReportProgress(FirstProgress.Title_Screen).Forget();

//             var ABTest = "A";
// #if SUNNY_B
//             ABTest = "B";
// #endif
//             LogDBEvent(AppEventType.app, ABTest);
        }

        #region 첫실행
        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void Cleanup()
        {
            isFirstPlay = false;
        }

        public static DateTime InitializedTime;

        /// <summary>
        /// 첫 실행 여부 로컬에 저장.
        /// </summary>
        static bool isFirstPlay = false;

        public static bool IsFirstPlay()
        {
            if (isFirstPlay)
                return true;

            var localData = UserData.Instance;
            isFirstPlay = localData.IsFirstPlay == 0;
            UserData.Instance.IsFirstPlay = 1;
            if (isFirstPlay)
            {
                GameUtils.Log($"Initial Launch Starting Log");
                UserDataSaveManager.Instance.Save();
            }

            return isFirstPlay;
        }

        public enum FirstProgress
        {
            App_Launched = 0, //	앱 첫 실행 시
            Title_Screen = 1, // is Disaplyed	타이틀 스크린 출력 시
            Loading_Resources_Finished = 2, // 	리소스 로딩 완료 시
            First_Cutscene_Started = 3, // 	리소스 로딩 완료 후 첫 컷씬이 나왔을 때
            First_Cutscene_Ended = 4, // 	리소스 로딩 완료 후 첫 컷씬이 끝났을 때
            First_Choice_Started = 5, // 	첫 스크립트 분기점이 출력 되었을 때
            First_Choice_Ended = 6, // 	첫 스크립트 분기점의 출력이 종료되었을 때
            Intro_Cutscene_Ended = 7, // 	인트로 컷씬의 출력이 종료되었을 때
            First_Tutorial_Started = 8, // 	인게임 처음 진입되었을 때
            First_Merge_Finished = 9, // 	튜토리얼에서 첫 머지 완료되었을 때
            Second_Merge_Finished = 10, // 	튜토리얼에서 두번째 머지 완료되었을 때
            Third_Merge_Finished = 11, // 	튜토리얼에서 세번째 머지 완료되었을 때
            First_Generate_Finished = 12, // 	튜토리얼에서 탭 통해서 처음 생산했을 때
            First_Unlock_Finished = 13, // 	튜토리얼에서 머지 통해서 처음으로 언락 풀었을 때
            Second_Unlock_Finished = 14, // 	튜토리얼에서 머지 통해서 두번째로 언락 풀었을 때
            First_Tutorial_Finished = 15, // 	첫번째 인게임 튜토리얼을 마치고 로비로 나왔을 때(로비로 나가기 버튼을 탭했을 때)
            First_Quest_Tapped = 16, // 	로비에서 완료된 첫번째 퀘스트 버튼(v 표시버튼)을 탭했을 때. 즉 퀘스트 완료팝업 출력되었을 때
            First_Quest_Finished = 17, // 	첫번째 퀘스트 완료하였을 때
            Second_Choice_Started = 18, // 	두번째 스크립트 분기점이 출력되었을 때
            Second_Choice_Ended = 19, // 	두번째 스크립트 분기점이 완료되었을 때
            Second_Tutorial_Started = 20, // 	두번째 인게임 튜토리얼이 시작되었을 때(인게임 진입했을 때)
            Second_Generate_Finished = 21, // 	탭해서 생산 완료되었을 때 (1단계 오브젝트 3개 생산 완료되었을 때)
            Second_Tutorial_Merge = 22, // 	두번째 퀘스트 머지
            Second_Quest_Tapped = 23, // 	로비에서 완료된 두번째 퀘스트 버튼(v 표시버튼)을 탭했을 때. 즉 퀘스트 완료팝업 출력되었을 때
            Second_Quest_Finished = 24, // 	두번째 퀘스트 완료하였을 때
            Third_Generate_Finished = 25, // 	세번째 인게임 튜토리얼에서 생산 완료되었을 때
            Third_Quest_Finished = 26, // 	세번째 퀘스트 완료하였을 때

            Quest_Cleared_4 = 27, // 4	네번째 퀘스트 완료하였을 때
            Quest_Cleared_5 = 28, // 5	다섯번째 퀘스트 완료하였을 때
        }

        public static async UniTaskVoid ReportProgress(FirstProgress step)
        {
            var seconds = (long) (TimeUtil.Now - InitializedTime).TotalSeconds;
            await UniTask.WaitUntil(() => UserData.Instance != null && UserDataSaveManager.Instance != null);
            if (!IsFirstPlay())
                return;

            FirstProgressSend ??= new HashSet<FirstProgress>();
            if (FirstProgressSend.Contains(step))
                return;
            
            FirstProgressSend.Add(step);
            var iStep = (int)step;

            Debug.Log($"FirstProgress {(int) step} : {step}");

            var param = $"{iStep:00}.{step.ToEnumString()}";
            parameters.Clear();
            parameters.Add("STEP", param);
            LogEvent("INITIAL_LAUNCH_FUNNEL", parameters);

            await UniTask.WaitUntil(() => CAppEvent.IsIntialized);
            CAppEvent.ReportProgress(iStep, seconds);
        }

        public static void ReportProgressQuestFinished(int questIndex)
        {
            FirstProgress step;
            switch (questIndex)
            {
                case 0:
                    step = FirstProgress.First_Quest_Finished;
                    break;
                case 1:
                    step = FirstProgress.Second_Quest_Finished;
                    break;
                case 2:
                    step = FirstProgress.Third_Quest_Finished;
                    break;
                default:
                    step = FirstProgress.Third_Quest_Finished + (questIndex - 2);
                    break;
            }
            ReportProgress(step).Forget();
        }

        #endregion

        public static void LogEvent(string logEvent, Dictionary<string, object> parameters = null)
        {
#if !UNITY_EDITOR
            if (LogFB)
            {
                if (FB.IsInitialized)
                    LogAppEvent_FB(logEvent, parameters);
                else
                    AsyncLogEvent_FB_Clone(logEvent, parameters).Forget();
            }
                
            if (LogCA)
            {
                if(CAppEvent.IsIntialized)
                    CAppEvent.Event(logEvent, parameters);
                else
                    AsyncLogEvent_Cookapps_Clone(logEvent, parameters).Forget();
            }
      
            if (LogFirebase)
            {
                if(isInitialized_Firebase)
                    LogAppEvent_Firebase(logEvent, parameters);
                else
                    AsyncLogEvent_Firebase_Clone(logEvent, parameters).Forget();
            }      
#endif
        }

        private static async UniTaskVoid AsyncLogEvent_FB_Clone(string logEvent, Dictionary<string, object> parameters = null)
        {
            var param = parameters.CloneParameters();
            await UniTask.WaitUntil(() => FB.IsInitialized);
            LogAppEvent_FB(logEvent, param);
        }
        
        private static async UniTaskVoid AsyncLogEvent_Cookapps_Clone(string logEvent, Dictionary<string, object> parameters = null)
        {
            var param = parameters.CloneParameters();
            await UniTask.WaitUntil(() => CAppEvent.IsIntialized);
            CAppEvent.Event(logEvent, parameters);
        }
        
        private static async UniTaskVoid AsyncLogEvent_Firebase_Clone(string logEvent, Dictionary<string, object> parameters = null)
        {
            var param = parameters.CloneParameters();
            await UniTask.WaitUntil(() => isInitialized_Firebase);
            LogAppEvent_Firebase(logEvent, param);
        }

        private static Dictionary<string, object> CloneParameters(this Dictionary<string, object> inst)
        {
            if (inst == null)
                return null;

            var dict = new Dictionary<string, object>();
            foreach (var keyValuePair in inst)
            {
                dict.Add(keyValuePair.Key, keyValuePair.Value);
            }

            return dict;
        }

        public static int GetConnectedDays()
        {
            var first = GameUtils.GetFlooredTime_Day(UserData.Instance.FirstAccessDate);
            var current = GameUtils.GetFlooredGameTime_Day();
            var r = current.Subtract(first);
            return (int)r.TotalDays;
        }
        
        public static int GetConnectedDays(DateTime fromDays)
        {
            var first = GameUtils.GetFlooredTime_Day(UserData.Instance.FirstAccessDate);
            var current = GameUtils.GetFlooredTime_Day(fromDays);
            var r = current.Subtract(first);
            return (int)r.TotalDays;
        }
        
        public enum RewardAdsType
        {
            Lobby_Ads,
            Ingame_energy,
            etc
        }

        public enum JewelEarnedBy
        {
            Purchased_with_Money = 1,
            Object_Collection_Reward = 2,
            Ingame_Board = 3,
            ETC = 99
        }

        public enum JewelSpent
        {
            Energy = 1,
            Object_Bubble_Unlock = 2,
            Time_Skip = 3,
            Web_Unlock = 4,
            Shop = 5,
            ShopRefresh = 6,
            FlowerGardenFactory_Unlock = 7,
            FlowerBoardQuest_Refresh = 8,
            Etc = 99
        }

        public enum CoinEarnedBy
        {
            Purchased_with_Money = 1,
            Object_Level_Reward = 2,
            Ingame_Board = 3,
            Object_Sell = 4,
            LobbyAds = 5,
            FlowerBoardQuest_Complete = 6,
            ETC = 99
        }

        public enum CoinSpent
        {
            Inventory = 1,
            Daily_Deals = 2,
            Flash_Sale = 3,
            Boxes = 4,
            ETC = 99
        }
    }
}