using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using OutGameCore;
using UnityEngine;

namespace SH.RuntimeInit
{
    public class RuntimeInitSingular
    {
        [RuntimeInitializeOnLoadMethod]
        private static void RuntimeInit()
        {
            InitSingular().Forget();
        }

        private static async UniTask InitSingular()
        {
            await UniTask.WaitUntil(() => SingularSDK.CheckInstance);
            SingularSDK.SetCustomUserId(SystemInfo.deviceUniqueIdentifier);
            SingularSDK.SkanRegisterAppForAdNetworkAttribution();
            SingularSDK.InitializeSingularSDK();
            Debug.Log($"SingularSDK Initialized");
        }
    }
}