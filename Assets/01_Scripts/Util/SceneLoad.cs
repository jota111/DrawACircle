/*
* Copyright (c) CookApps.
* 이진호(jhlee8@cookapps.com)
*/

using Cysharp.Threading.Tasks;
using OutGameCore;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SH.Util
{
    public static class SceneLoad
    {
        public static async UniTask<AsyncOperation> LoadSceneAsync(string sceneName)
        {
            var async = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
            async.allowSceneActivation = false;
            await UniTask.WaitWhile(() => async.progress < 0.9f);
            return async;
        }

        public static void LoadScene(string sceneName)
        {
            SceneManager.LoadScene(sceneName);
        }
    }
}