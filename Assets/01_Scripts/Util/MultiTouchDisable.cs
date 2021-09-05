/*
* Copyright (c) CookApps.
* 이진호(jhlee8@cookapps.com)
*/

using UnityEngine;

namespace SH.Util
{
    public class MultiTouchDisable : MonoBehaviour
    {
        private void OnEnable()
        {
            Input.multiTouchEnabled = false;
        }

        private void OnDisable()
        {
            Input.multiTouchEnabled = true;
        }
    }
}