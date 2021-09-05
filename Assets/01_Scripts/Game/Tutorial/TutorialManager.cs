/*
* Copyright (c) CookApps.
* 이진호(jhlee8@cookapps.com)
*/

using System;
using System.Collections.Generic;
using System.Linq;
using SH.AppEvent;
using SH.Constant;
using SH.Data;
using SH.Game.DataLocal;
using SH.Game.InGame.Msg;
using SH.Game.Manager;
using SH.Util;
using SH.Util.UniRx;
using UniRx;
using UnityEngine;
using UnityEngine.Assertions;
using Zenject;

namespace SH.Game.Tutorial
{
    public interface IContentPossible
    {
        bool PossibleShopSale();
        IObservable<bool> ObsPossibleShopSale();
    }

    public sealed class TutorialManager : IContentPossible, IInitializable
    {
        private readonly DiContainer _container;
        private readonly SceneDisposable _sceneDisposable;
        private readonly bool _boardEditor;
        private readonly TutorialLocal _tutorialLocal;
        private IObservable<bool> _holdReachingProduct;


        public TutorialManager(UserData.UserData userData,
            DiContainer container,
            SceneDisposable sceneDisposable,
            [Inject(Id = ZenId.BoardEditor, Optional = true)]
            bool boardEditor)
        {
            _container = container;
            _sceneDisposable = sceneDisposable;
            _boardEditor = boardEditor;
            _tutorialLocal = userData.Tutorial;
        }
        
        public void Initialize()
        {
            if (_boardEditor)
                return;
            
            InitializeButton();
            
            // 튜토리얼 스텝 완료시 메세지 전달
            var arrNotFinish = _tutorialLocal.TutorialStep.Values.Where(value => !value.Value).ToArray();
            if (arrNotFinish.Length > 0)
            {
                arrNotFinish.Merge()
                    .BatchFrame()
                    .Skip(1)
                    .Subscribe(_ =>
                    {
                        MessageBroker.Default.Publish(new TutorialStepFinish());
                    })
                    .AddTo(_sceneDisposable);
            }
        }

        public bool IsFinish(TutorialStep step)
        {
            return _tutorialLocal.GetStep(step).Value;
        }
        
        private void InitializeButton()
        {
        }

        private void Instantiate<T>(TutorialStep step, params object[] args) where T : IInitializable
        {
            var boolReactiveProperty = GetStep(step);
            if (boolReactiveProperty.Value)
                return;
            var param = new object[args.Length + 2];
            param[0] = step;
            param[1] = boolReactiveProperty;
            Array.Copy(args, 0,  param, 2, args.Length);
            Instantiate<T>(param);
        }
        
        private void Instantiate<T>(params object[] args) where T : IInitializable
        {
            _container.Instantiate<T>(args).Initialize();
        }

        private BoolReactiveProperty GetStep(TutorialStep step) => _tutorialLocal.GetStep(step);

        /// <summary>
        /// 퀘스트 버튼 가능한가?
        /// </summary>
        /// <returns></returns>
        public IObservable<bool> ObsPossibleQuestButton()
        {
            // 처음부터 열자
            return Observable.Return(true);
        }
        
        private const int QuestIdShopOpen = 1130;
        public IObservable<bool> ObsPossibleShopSale()
        {
            return Observable.Return(true);
        }

        public bool PossibleShopSale()
        {
            return true;
        }

        public void ForceClearAll()
        {
            foreach (var tutorialStep in EnumHelper.AllValues<TutorialStep>())
            {
                if (tutorialStep == TutorialStep.FlowerGarden_FirstInGame)
                    break;
                
                GetStep(tutorialStep).Value = true;
            }
        }
    }
} 