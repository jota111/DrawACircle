using System;
using Cysharp.Threading.Tasks;
using Doozy.Engine.UI;
using UnityEngine;
using Sirenix.OdinInspector;
using UniRx;
using UnityEngine.Serialization;
using Object = UnityEngine.Object;

namespace OutGameCore
{
    public partial class OutGame : MonoBehaviour
    {
        #region 싱글톤

        private static OutGame _instance = null;
        public static bool Exist => _instance != null;

        public static OutGame Instance
        {
            get
            {
                SetInstance();
                return _instance;
            }
        }

        private const string PathPrefab = "OutGame/OutGame";
        private static Object Prefab;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ReloadDomain()
        {
            Prefab = null;
        }

        public void Start()
        {
            Test_LunarStart();
        }

        public static void InstantiateOutGame()
        {
            var prefab = Prefab != null ? Prefab : Resources.Load(PathPrefab);
            var instance = Instantiate(prefab);
            instance.name = "OutGame";
        }

        public static async UniTask<Unit> AsyncLoadOutGame()
        {
            await UniTask.NextFrame();
            if (Prefab != null)
                return Unit.Default;
            
            var req = Resources.LoadAsync(PathPrefab);
            req.priority = -1;
            await UniTask.WaitUntil(() => !req.isDone);
            Prefab = req.asset;
            return Unit.Default;
        }

        private static void SetInstance()
        {
            if (_instance == null)
            {
                GameObject go = GameObject.Find("OutGame");
                if (go != null)
                {
                    _instance = go.GetComponent<OutGame>();
                    if (_instance == null)
                    {
                        GameUtils.Error("OutGame Instance Null");
                    }
// #if UNITY_EDITOR
//                     if (Application.isPlaying)
// #endif
//                         DontDestroyOnLoad(go);
                }
            }
        }

        void OnDestroy()
        {
            _instance = null;
            Test_LunarDestroy();
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void Cleanup()
        {
            if (_instance != null)
                Destroy(_instance.gameObject);
            _instance = null;
        }

        #endregion

        #region 매니저

        public GamePools gamePools;
        // public UserSecureDataManager userSecureDataManager;
        public Canvas OutGameCanvas;
        #endregion

        public void Init()
        {
        }

        private Action resetEvent = null;

        public void Reset()
        {
            resetEvent?.Invoke();
            resetEvent = null;
        }

        public void AddResetEvent(Action action)
        {
            resetEvent += action;
        }


        #region 테스트


        [FoldoutGroup("시간 관련 테스트 모음", 9000), LabelText("테스트 시간 적용"), PropertyOrder(3)]
        public bool test_dateTime_enabled = false;

        [FoldoutGroup("시간 관련 테스트 모음", 9000), LabelText("게임타임 변경값 (시)"), PropertyOrder(5), ShowIf("test_dateTime_enabled")]
        public double test_deltaTimeHour = 0;
        [FoldoutGroup("시간 관련 테스트 모음", 9000), LabelText("게임타임 변경값 (분)"), PropertyOrder(5), ShowIf("test_dateTime_enabled")]
        public double test_deltaTimeMinute = 0;

        [FoldoutGroup("시간 관련 테스트 모음", 9000), LabelText("테스트 시간"), PropertyOrder(4), ShowIf("test_dateTime_enabled")]
        public DateTime test_dateTime = DateTime.UtcNow;

        // [FoldoutGroup("시간 관련 테스트 모음", 9000), LabelText("게임타임 변경"), PropertyOrder(6), ShowIf("test_dateTime_enabled"),
        //  Button(ButtonSizes.Large)]
        // public void Test_DateTime_AddSec()
        // {
        //     test_dateTime = test_dateTime.AddHours(test_deltaTimeHour).AddMinutes(test_deltaTimeMinute);
        //     GameUtils.Log($"시간 테스트 / 게임타임 적용: {test_dateTime_enabled} / 게임타임: {test_dateTime}");
        // }

        // [FoldoutGroup("시간 관련 테스트 모음", 9000), LabelText("게임타임을 현재시각으로 설정 (UTC)"), PropertyOrder(7),
        //  ShowIf("test_dateTime_enabled"), Button(ButtonSizes.Large)]
        // public void Test_DateTime_ResetToUtcNow()
        // {
        //     test_dateTime = DateTime.UtcNow;
        //     GameUtils.Log($"시간 테스트 / 게임타임 적용: {test_dateTime_enabled} / 게임타임: {test_dateTime}");
        // }
#if UNITY_EDITOR
#endif

        #endregion
    }
}