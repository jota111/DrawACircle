/*
* Copyright (c) CookApps.
* 이진호(jhlee8@cookapps.com)
*/

using UnityEngine;
using UnityEngine.UI;

namespace SH.Util
{
    public class OnlyDev : MonoBehaviour
    {
        [SerializeField]
        private Text _text;
        private void Awake()
        {
            if (_text != null)
            {
                var dev = "DEV";
#if UNITY_AWS
                dev += "-AWS";
#endif
                _text.text = dev;
            }
#if !__DEV && !UNITY_EDITOR
            Destroy(gameObject);
#endif
        }
    }
}