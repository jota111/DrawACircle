/*
* Copyright (c) CookApps.
* 이진호(jhlee8@cookapps.com)
*/

using SH.Game.Tutorial;
using SH.UI;
using Zenject;

namespace SH.Game.Manager
{
    public static class ManagerInstaller
    {
        public static void Install(DiContainer Container)
        {
            Container.BindInterfacesAndSelfTo<ShopManager>().AsSingle();
            Container.BindInterfacesAndSelfTo<IapManager>().AsSingle();
            Container.BindInterfacesAndSelfTo<TutorialManager>().AsSingle();
            Container.BindInterfacesAndSelfTo<ScreenManager>().AsSingle();
            Container.BindInterfacesAndSelfTo<InteractableManager>().AsSingle();
            Container.BindInterfacesAndSelfTo<EnergyManager>().AsSingle();
            Container.BindInterfacesAndSelfTo<FundManager>().AsSingle();
            Container.BindInterfacesAndSelfTo<LevelManager>().AsSingle();
            Container.BindInterfacesAndSelfTo<UIViewManager>().AsSingle();
            Container.BindInterfacesAndSelfTo<GetEffectManager>().AsSingle();
            Container.BindInterfacesAndSelfTo<GameSoundManager>().AsSingle();
            Container.BindInterfacesAndSelfTo<LocalNotificationManager>().AsSingle();
            Container.BindInterfacesTo<BackButtonManager>().AsSingle();
            Container.BindInterfacesAndSelfTo<FirstViewManager>().AsSingle();
        }
    }
}