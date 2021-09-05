/*
* Copyright (c) CookApps.
* 이진호(jhlee8@cookapps.com)
*/

using Cysharp.Threading.Tasks;
using GameDataEditor;
using Lean.Pool;
using SH.Constant;
using SH.Game.Misc;
using UnityEngine;
using Zenject;

namespace SH.Game.Manager
{
    public sealed class GetEffectManager
    {
        [InjectOptional(Id = ZenId.RectTransformLevel)]
        private RectTransform _transformLevel;

        [InjectOptional(Id = ZenId.RectTransformGold)]
        private RectTransform _transformGold;

        [InjectOptional(Id = ZenId.RectTransformJewel)]
        private RectTransform _transformJewel;

        [InjectOptional(Id = ZenId.RectTransformEnergy)]
        private RectTransform _transformEnergy;

        [InjectOptional(Id = ZenId.RectTransformMainBoardButton)]
        private RectTransform _transformMainBoardButton;

        [InjectOptional(Id = ZenId.RectTransformEventBoardButton)]
        private RectTransform _transformEventBoardButton;

        [InjectOptional(Id = ZenId.RectTransformFlowerBoardButton)]
        private RectTransform _transformFlowerBoardButton;

        [InjectOptional(Id = ZenId.RectTransformEventBoardButtonInGame)]
        private RectTransform _transformEventBoardButtonInGame;

        [InjectOptional(Id = ZenId.RectTransformGetEffectParent)]
        private RectTransform _transformParent;

        [InjectOptional(Id = ZenId.RectTransformInventoryButton)]
        private RectTransform _transformInventoryButton;

        [InjectOptional(Id = ZenId.RectTransformLobbyButton)]
        private RectTransform _transformLobbyButton;

        [InjectOptional(Id = ZenId.ButtonStack)]
        private RectTransform _transformStackButton;

        public static GetEffectManager Instance { get; private set; }

        [Inject]
        public void Construct()
        {
            Instance = this;
        }

        public void Get(Sprite item, Vector3 fromPos)
        {
            var target = GetTarget(item);
            if (target != null && _transformParent != null)
            {
                var obj = Spawn();
                obj.SetEffect(item, fromPos, target).Forget();
            }
        }

        public void Get(Sprite item, Vector3 fromPos, Transform target)
        {
            var obj = Spawn();
            obj.SetEffect(item, fromPos, target).Forget();
        }

        public async UniTask GetAsync(Sprite item, Vector3 fromPos, Transform target)
        {
            var obj = Spawn();
            await obj.SetEffect(item, fromPos, target);
        }

        public void Get(Sprite item, Vector3 fromPos, Transform target, Invalidate.TargetType invalidateType)
        {
            var obj = Spawn();
            obj.AsyncSetEffect(item, fromPos, target, invalidateType).Forget();
        }

        private EffectGet Spawn()
        {
            var obj = Resources.Load<EffectGet>("Effect/EffectGet");
            var inst = LeanPool.Spawn(obj, _transformParent);
            return inst;
        }

        private Transform GetTarget(Sprite item)
        {
            if (item.name.Contains(Currencies.Coin.ToString()))
                return _transformGold;
            else if (item.name.Contains(Currencies.Diamond.ToString()))
                return _transformJewel;
            else if (item.name.Contains(Currencies.Energy.ToString()))
                return _transformEnergy;
            else if (item.name.Contains(Currencies.XP.ToString()))
                return _transformLevel;
            else return ScreenTransition.ScreenState == ScreenState.Lobby ? _transformMainBoardButton : _transformStackButton;
        }
    }
}