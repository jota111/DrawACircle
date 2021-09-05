using System.Collections;
using System.Collections.Generic;
using Unity.Linq;
using UnityEngine;

namespace SH.Game.Settings
{
    public class UIItemPlatform : MonoBehaviour
    {
        public enum PlatformTarget
        {
            Android,
            iOS,
        }

        [SerializeField] private PlatformTarget platformTarget = PlatformTarget.Android;

        private void Awake()
        {
            switch (platformTarget)
            {
                case PlatformTarget.Android:
#if !UNITY_ANDROID
                    gameObject.Destroy();            
#endif
                    break;
                case PlatformTarget.iOS:
#if !UNITY_IOS
                    gameObject.Destroy();            
#endif
                    break;
            }
        }
    }
}
