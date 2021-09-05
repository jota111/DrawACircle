/*
* Copyright (c) CookApps.
* 이진호(jhlee8@cookapps.com)
*/

using Sirenix.OdinInspector;
using UnityEngine;

namespace SH.UI.View.Relaunch
{
    public class ForceRelaunch : MonoBehaviour
    {
        [SerializeField, Required]
        private UIViewRelaunch _viewRelaunch;

        [SerializeField]
        private GameObject[] _goHide;

        private void Start()
        {
            CheckNet();
        }

        private void CheckNet()
        {
            
        }
    }
}