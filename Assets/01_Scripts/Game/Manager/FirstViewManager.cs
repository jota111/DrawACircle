using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Doozy.Engine.UI;
using GameDataEditor;
using OutGameCore;
using SH.UI;
using SH.Util.UniRx;
using UniRx;
using Zenject;

namespace SH.Game.Manager
{
    public sealed class FirstViewManager : IInitializable
    {
        private UserData.UserData _userData;
        private SceneDisposable _disposable;

        public FirstViewManager(UserData.UserData userData, SceneDisposable disposable)
        {
            _userData = userData;
            _disposable = disposable;
        }

        public void Initialize()
        {
            StartInitializeAsync().Forget();
        }

        private async UniTask StartInitializeAsync()
        {
            await UniTask.WaitUntil(() => OutGame.Instance != null);
            
        }
    }
}