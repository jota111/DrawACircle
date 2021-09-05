/*
* Copyright (c) CookApps.
* 이진호(jhlee8@cookapps.com)
*/

using Cysharp.Threading.Tasks;
using GameDataEditor;
using UniRx;
using UnityEngine;

namespace SH.RuntimeInit
{
    public static class InitLoadManager
    {
        private static bool _initGDE { get; set; }
        

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ReloadDomain()
        {
            _initGDE = false;
        }

        public static async UniTask<Unit> AsyncLoad()
        {
            await UniTask.WhenAll(AsyncInitGDE(), AsyncInitAudioController());
            return Unit.Default;
        }

        public static void Load()
        {
            InitGDE();
            InitAudioController();
        }

        private static async UniTask<bool> AsyncInitGDE()
        {
            if(_initGDE)
                return true;

            var asset = await LoadAsync<TextAsset>(GDEDataManager.PathData);
            _initGDE = GDEDataManager.InitFromText(asset.text);
            Resources.UnloadAsset(asset);
            return _initGDE;
        } 

        private static bool InitGDE()
        {
            if (_initGDE)
                return true;
            
            var dataAsset = Resources.Load(GDEDataManager.PathData) as TextAsset;
            var init = GDEDataManager.InitFromText(dataAsset.text);
            Resources.UnloadAsset(dataAsset);
            return init;
        }

        private static bool InitAudioController()
        {
            if (AudioController.DoesInstanceExist() == null)
            {
                var go = Object.Instantiate(Resources.Load("AudioPrefab/AudioController"));
                if (go != null)
                {
                    Object.DontDestroyOnLoad(go);
                }
            }

            return true;
        }

        private static async UniTask<bool> AsyncInitAudioController()
        {
            if (AudioController.DoesInstanceExist())
                return true;

            var prefab = await LoadAsync<Object>("AudioPrefab/AudioController");
            if (prefab == null)
                return false;

            var go = Object.Instantiate(prefab);
            Object.DontDestroyOnLoad(go);
            Resources.UnloadAsset(prefab);
            return true;
        }

        private static async UniTask<T> LoadAsync<T>(string path) where T : Object
        {
            var req = Resources.LoadAsync<TextAsset>(path);
            req.priority = -1;
            await UniTask.WaitUntil(() => !req.isDone);
            return req.asset as T;
        }
    }
}