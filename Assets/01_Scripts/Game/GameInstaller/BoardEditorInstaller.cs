/*
* Copyright (c) CookApps.
* 이진호(jhlee8@cookapps.com)
*/

using GameDataEditor;
using SH.Constant;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

namespace SH.Game.GameInstaller
{
    public sealed class BoardEditorInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            var scene = SceneManager.GetActiveScene();
            if(scene.name.Equals("BoardEditor"))
            {
                Container.BindInstance(true).WithId(ZenId.BoardEditor);
                
                var dataAsset = Resources.Load("gde_data") as TextAsset;
                var init = GDEDataManager.InitFromText(dataAsset.text);
                Resources.UnloadAsset(dataAsset);
            }
        }
    }
}