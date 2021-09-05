/*
* Copyright (c) CookApps.
* 이진호(jhlee8@cookapps.com)
*/

using UnityEngine;
using UnityEngine.Assertions;
using Zenject;

namespace SH.ProjectInstaller
{
    public class AudioControllerInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            if (AudioController.DoesInstanceExist() == null)
            {
                var prefab = Resources.Load("AudioPrefab/AudioController");
                Assert.IsNotNull(prefab);
                var go = Object.Instantiate(prefab);
                if (go != null)
                {
                    Object.DontDestroyOnLoad(go);
                }
            }
        }
    }
}