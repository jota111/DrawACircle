/*
* Copyright (c) CookApps.
* 이진호(jhlee8@cookapps.com)
*/

using System.Diagnostics;
using CodeStage.AdvancedFPSCounter;
using CodeStage.AdvancedFPSCounter.Labels;
using Cysharp.Threading.Tasks;
using OutGameCore;
using SH.AppEvent;
using SH.Constant;
using SH.Game.InGame.Installer;
using SH.Game.Manager;
using SH.Game.Misc;
using SH.Game.UserData;
using SH.Game.UserManager;
using SH.Util.UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

namespace SH.Game.GameInstaller
{
    public sealed class GameInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<SceneDisposable>().AsSingle();
            Container.BindInstance(UserDataSaveManager.Load()).AsSingle();
            Container.BindInterfacesTo<UserDataSaveManager>().AsSingle();
            Container.BindInterfacesTo<UserDataSaveServerManager>().AsSingle();
            Container.BindInterfacesTo<AppFocusCheck>().AsSingle();
            ManagerInstaller.Install(Container);
            InGameInstaller.Install(Container);

            CheckOutGame();
            FPSCounter();
            UnloadUnusedAssets().Forget();
        }

        [Conditional("__DEV"), Conditional("UNITY_EDITOR")]
        private void FPSCounter()
        {
            var fps = AFPSCounter.AddToScene(false);
            if (fps != null)
            {
                fps.AutoScale = true;
                fps.circleGesture = true;
                fps.fpsCounter.Anchor = LabelAnchor.UpperRight;
                fps.fpsCounter.MinMax = false;
                fps.memoryCounter.Anchor = LabelAnchor.UpperRight;
                fps.memoryCounter.Gfx = false;
                fps.deviceInfoCounter.Enabled = false;
                fps.OperationMode = OperationMode.Disabled;
            }
        }

        // [Conditional("UNITY_EDITOR")]
        private void CheckOutGame()
        {
            var scene = SceneManager.GetActiveScene();
            if (!scene.name.Equals(SceneType.SceneGame))
                return;

            if (!OutGame.Exist)
            {
                OutGame.InstantiateOutGame();
                OutGame.Instance.Init();
            }
        }

        private async UniTaskVoid UnloadUnusedAssets()
        {
            await UniTask.NextFrame();
            await Resources.UnloadUnusedAssets();
            Debug.Log("UnloadUnusedAssets");
        }
    }
}