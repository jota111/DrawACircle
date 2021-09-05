/*
* Copyright (c) CookApps.
* 이진호(jhlee8@cookapps.com)
*/

using SH.Constant;
using SH.Iap;
using SH.RuntimeInit;
using UnityEngine.SceneManagement;
using Zenject;

namespace SH.ProjectInstaller
{
    public sealed class ProjectInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<IapModuleManager>().AsSingle();
#if UNITY_EDITOR
            var activeScene = SceneManager.GetActiveScene();
            if(activeScene.name.Equals(SceneType.SceneGame))
                InitLoadManager.Load();
#endif
        }
    }
}