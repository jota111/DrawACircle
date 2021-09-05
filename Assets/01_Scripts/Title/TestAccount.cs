/*
* Copyright (c) CookApps.
* 이진호(jhlee8@cookapps.com)
*/

using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace SH.Title
{
    public class TestAccount : MonoBehaviour
    {
        public static long Uid { get; private set; }
        
#if UNITY_EDITOR
        public static bool TestMode { get; private set; }
#else
        public static bool TestMode => false;
#endif

        [SerializeField] private long _uid;
        [SerializeField] private bool _testMode;

#if UNITY_EDITOR
        [Inject]
        private void Construct()
        {
            Uid = _uid;
            TestMode = _testMode && _uid != 0;
        }
#endif

        [HideInPlayMode]
        [Button(ButtonSizes.Large)]
        private void Reset()
        {
            _uid = 0;
            _testMode = false;
        }
    }
}