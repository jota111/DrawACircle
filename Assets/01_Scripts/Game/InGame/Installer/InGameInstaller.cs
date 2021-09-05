/*
* Copyright (c) CookApps.
* 이진호(jhlee8@cookapps.com)
*/

using SH.Game.DataLocal;
using Zenject;

namespace SH.Game.InGame.Installer
{
    public static class InGameInstaller
    {
        public static void Install(DiContainer Container)
        {
            // Container.BindInterfacesAndSelfTo<FlowerGardenFactoryContainerManager>().AsSingle();
            // Container.BindInterfacesAndSelfTo<ItemCollectionManager>().AsSingle();
            // Container.BindInterfacesTo<MergeBoardManager>().AsSingle();
            // Container.BindInterfacesAndSelfTo<ItemChestManager>().AsSingle().NonLazy();
            // Container.BindInterfacesAndSelfTo<ItemSelectManager>().AsSingle();
            // Container.BindInterfacesAndSelfTo<ItemMergeHintManager>().AsSingle().NonLazy();
            // Container.BindInterfacesAndSelfTo<ItemStackManager>().AsSingle().NonLazy();
            // Container.BindInterfacesAndSelfTo<ItemInventoryManager>().AsSingle().NonLazy();
            // Container.BindInterfacesAndSelfTo<ItemSaleManager>().AsSingle().NonLazy();
        }
    }
}