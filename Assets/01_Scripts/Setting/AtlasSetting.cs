/*
* Copyright (c) CookApps.
* 이진호(jhlee8@cookapps.com)
*/

using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using SH.Constant;
using SH.Data;
using SH.Util;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;
using Zenject;

namespace SH.Setting
{
    //[CreateAssetMenu(fileName = "AtlasSetting", menuName = "Installers/SH/AtlasSetting")]
    public class AtlasSetting : ScriptableObjectInstaller<AtlasSetting>
    {
        // ReSharper disable once MemberCanBePrivate.Global
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public static AtlasSetting Instance { get; private set; }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void DomainCleanup()
        {
            Instance = null;
        }
        
        [SerializeField, ValidateInput(nameof(Valid))] private AtlasDict _atlas;
        private Dictionary<AtlasType, SpriteAtlasCache> _dictAtlasSpriteCache;

        private bool Valid(AtlasDict dict) => ProjectValid.Valid(dict);

        public SpriteAtlasCache GetAtlas(AtlasType type)
        {
            if (_dictAtlasSpriteCache.TryGetValue(type, out var atlasCache))
                return atlasCache;

            if (_atlas.TryGetValue(type, out var atlas))
            {
                atlasCache = new SpriteAtlasCache(atlas);
                _dictAtlasSpriteCache.Add(type, atlasCache);   
            }
            return atlasCache;
        }

        public Sprite GetSprite(AtlasType typeEnum, string spriteName)
        {
            return string.IsNullOrEmpty(spriteName) ? null : GetAtlas(typeEnum)?.GetSprite(spriteName);
        }

        public Sprite GetShopIcon(string key)
        {
            var sprite = GetSprite(AtlasType.Shop, key);
            return sprite;
        }

        public Sprite GetEventCustomer(string key)
        {
            var sprite = GetSprite(AtlasType.EventCustomer, key);
            return sprite;
        }

        public Sprite GetOutGameIcon(string key)
        {
            var sprite = GetSprite(AtlasType.OutGame, key);
            return sprite;
        }

        public Sprite GetLobbyObject(string key)
        {
            var sprite = GetSprite(AtlasType.Lobby, key);
            return sprite;
        }

        public override void InstallBindings()
        {
            Container.BindInstance(this).AsSingle();
            _dictAtlasSpriteCache = new Dictionary<AtlasType, SpriteAtlasCache>();
            Instance = this;
        }

        
        [System.Diagnostics.Conditional("UNITY_EDITOR"), System.Diagnostics.Conditional("__DEV")]
        public void ClearBeforeGDEValid()
        {
            _dictAtlasSpriteCache = new Dictionary<AtlasType, SpriteAtlasCache>();
        }

        [Serializable]
        public class AtlasDict : UnitySerializedDictionary<AtlasType, SpriteAtlas>
        {
          
        }
    }
}