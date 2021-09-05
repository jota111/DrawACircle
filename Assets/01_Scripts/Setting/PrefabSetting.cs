/*
* Copyright (c) CookApps.
* 이진호(jhlee8@cookapps.com)
*/

using System;
using Lean.Pool;
using SH.Constant;
using SH.Util;
using UnityEngine;
using Zenject;

namespace SH.Setting
{
    //[CreateAssetMenu(fileName = "PrefabSetting", menuName = "Installers/SH/PrefabSetting")]
    public class PrefabSetting : ScriptableObjectInstaller<AtlasSetting>
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void DomainCleanup()
        {
            Instance = null;
        }
        public static PrefabSetting Instance { get; private set; }
        [SerializeField] private PrefabDict _prefabDict;

        public GameObject GetPrefab(PrefabType prefabType)
        {
            if (!_prefabDict.TryGetValue(prefabType, out var prefab))
            {
                Debug.LogError($"not found prefab {prefabType}");
            }

            return prefab;
        }

        public T GetPrefab<T>(PrefabType prefabType) where T : Component
        {
            var prefab = GetPrefab(prefabType)?.GetComponent<T>();
            if (prefab == null)
            {
                Debug.LogError($"not found prefab {prefabType}, {typeof(T).Name}");
            }

            return prefab;
        }

        public GameObject SpawnPool(PrefabType prefabType, Transform parent, bool worldPositionStays = false)
        {
            var prefab = GetPrefab(prefabType);
            return prefab != null ? LeanPool.Spawn(prefab, parent, worldPositionStays) : null;
        }

        public T SpawnPool<T>(PrefabType prefabType, Transform parent, bool worldPositionStays = false)
            where T : Component
        {
            var prefab = GetPrefab<T>(prefabType);
            return prefab != null ? LeanPool.Spawn(prefab, parent, worldPositionStays) : null;
        }

        public GameObject SpawnPool(PrefabType prefabType, Vector3 position, Quaternion rotation,
            Transform parent = null)
        {
            var prefab = GetPrefab(prefabType);
            return prefab != null ? LeanPool.Spawn(prefab, position, rotation, parent) : null;
        }

        public T SpawnPool<T>(PrefabType prefabType, Vector3 position, Quaternion rotation, Transform parent = null)
            where T : Component
        {
            var prefab = GetPrefab<T>(prefabType);
            return prefab != null ? LeanPool.Spawn(prefab, position, rotation, parent) : null;
        }

        public override void InstallBindings()
        {
            Container.BindInstance(this).AsSingle();
            Instance = this;
        }

        [Serializable]
        public class PrefabDict : UnitySerializedDictionary<PrefabType, GameObject>
        {
        }
    }
}