/*
* Copyright (c) CookApps.
* 이진호(jhlee8@cookapps.com)
*/

using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using Beebyte.Obfuscator;
using CodeStage.AntiCheat.Detectors;
using Cysharp.Threading.Tasks;
using SH.Util.UniRx;
using UniRx;
using UnityEngine;

namespace SH.Ob
{
    [ReplaceLiteralsWithName]
    public static class AppCheck
    {
        public static IReadOnlyReactiveProperty<bool> IsPass => _isPass;
        private static readonly BoolReactiveProperty _isPass = new BoolReactiveProperty(true);
        private static TimeCheatingDetector timeCheatingDetector;
        
        // 아마존 - 43:03:5f:1b:3d:2a:10:26:80:f7:fa:7b:83:33:80:ee:67:4e:51:b7 (1793566593)
        // 구글 배포 - cb:66:64:28:7d:b8:21:fd:5c:04:b4:1f:be:ac:61:c4:e1:9f:48:68 (1550517005)
        // 개발 빌드 - 55:88:3f:3a:7d:2f:69:5c:5a:69:d9:93:a3:ec:67:5f:2f:ad:97:99 (-958283578)
        private static readonly List<int> SValue = new List<int>{1793566593, 1550517005, -958283578};

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ReloadDomain()
        {
            _isPass.Value = true;
            timeCheatingDetector = null;
        }

        [RuntimeInitializeOnLoadMethod]
        private static async void RuntimeInit()
        {
            await UniTask.DelayFrame(5);
            
            _isPass.Value &= InitHCheck();
            ObscuredCheatingDetector.StartDetection(() => _isPass.Value = false);
            SpeedHackDetector.StartDetection(() => _isPass.Value = false);
            timeCheatingDetector = TimeCheatingDetector.StartDetection(0, (result, error) => { });
            _isPass.Subscribe(value => Debug.Log($"AppCheck : {value}"));

            // 시간체크 - 앱 활성화시, 5분단위로 체크
            Observable.EveryApplicationPause().WhereFalse().AsUnitObservable()
                .Merge(Observable.Interval(TimeSpan.FromMinutes(5), Scheduler.MainThreadIgnoreTimeScale).AsUnitObservable())
                .StartWith(Unit.Default)
                .BatchFrame()
                .ThrottleFirst(TimeSpan.FromMinutes(2))
                .Subscribe(_ => ForceTimeCheck());
        }

        public static void ForceTimeCheck()
        {
            TimeCheck().Forget();
        }

        private static async UniTaskVoid TimeCheck()
        {
            if (timeCheatingDetector == null)
                return;
            
            var startTime = DateTime.Now;
            var result = await timeCheatingDetector.ForceCheckTask();
            var now = DateTime.Now;
            var diff = now - startTime;
            if (diff.TotalMinutes >= 1)
                return;
            
            if (result == TimeCheatingDetector.CheckResult.CheatDetected || result == TimeCheatingDetector.CheckResult.WrongTimeDetected)
                _isPass.Value = false;
        }

        /// <summary>
        /// 불법 프로그램 체크
        /// </summary>
        /// <returns></returns>
        private static bool InitHCheck()
        {
            if (Debug.isDebugBuild)
                return true;
            
#if UNITY_ANDROID && !UNITY_EDITOR
            var value = GetIntValue(Cookapps.H.HH());
            if (!SValue.Contains(value))
                return false;
            
            // 루트 체크 (비활성함)
            // var root = Cookapps.Checker.RCheck();
            // if (root)
            //     return false;
#endif
            return true;
            
            //---------------------------------------------------
            int GetIntValue(string h)
            {
                var lower = h.ToLower(); 
                var md5Hasher = MD5.Create();
                var hashed = md5Hasher.ComputeHash(Encoding.UTF8.GetBytes(lower));
                var value = BitConverter.ToInt32(hashed, 0);
                return value;
            }
        }
    }
}