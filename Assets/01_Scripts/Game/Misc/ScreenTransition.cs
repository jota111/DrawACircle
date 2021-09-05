/*
* Copyright (c) CookApps.
* 이진호(jhlee8@cookapps.com)
*/

using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using OutGameCore;
using MoreLinq.Extensions;
using SH.Constant;
using SH.Game.InGame.Msg;
using SH.Game.Manager;
using Sirenix.OdinInspector;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace SH.Game.Misc
{
    public interface IScreenTransitionMonoBehaviour
    {
        void TransitionMonoBehaviour(ScreenState screen, Action action = null);
    }
    
    [RequireComponent(typeof(Canvas))]
    public sealed class ScreenTransition : MonoBehaviour, IScreenTransitionMonoBehaviour
    {
        [SerializeField] private float _duration = 0.5f;
        [SerializeField, Required, ChildGameObjectsOnly] private RectTransform _transitionUp;
        [SerializeField, Required, ChildGameObjectsOnly] private RectTransform _transitionDown;
        [SerializeField] private List<GameObject> _goLobby;
        [SerializeField] private List<GameObject> _goInGame;
        [Inject] private GameSoundManager _gameSoundManager;

        [Inject(Id = ZenId.BoardEditor, Optional = true)]
        private bool _boardEditor;
        private Canvas _canvas;
        private ScreenState _screenState;
        private RectTransform _rectTransform;
        public static ScreenTransition Instance { get; private set; }
        public static ScreenState ScreenState => Instance._screenState;

        private void Awake()
        {
            Instance = this;
            _canvas = GetComponent<Canvas>();
            _rectTransform = GetComponent<RectTransform>();
            _canvas.enabled = false;
            
            // OutGame go 추가
            if (!_boardEditor)
            {
                _goLobby.Add(OutGame.Instance.gameObject);
                // 기본 로비로 설정
                ScreenLobby();
            }
        }

        private async UniTask Transition(Action action1, Action action2)
        {
            _canvas.enabled = true;
            _transitionUp.sizeDelta = Vector2.zero;
            _transitionDown.sizeDelta = Vector2.zero;

            _gameSoundManager.PlaySfx(SFX.sh_transition);
            var size = new Vector2(0, _rectTransform.rect.height * 0.5f);
            var tweenCloseUp  = _transitionUp.DOSizeDelta(size, _duration / 2.0f)
                .SetEase(Ease.OutSine)
                .SetRecyclable(true);
            
            var tweenCloseDown  = _transitionDown.DOSizeDelta(size, _duration / 2.0f)
                .SetEase(Ease.OutSine)
                .SetRecyclable(true);

            await UniTask.WhenAll(tweenCloseUp.ToUniTask(), tweenCloseDown.ToUniTask());

            await UniTask.DelayFrame(1);
            action1?.Invoke();
            await UniTask.DelayFrame(1);
            action2?.Invoke();
            await UniTask.DelayFrame(1);
            
            var tweenOpenUp =  _transitionUp.DOSizeDelta(Vector2.zero, _duration / 2.0f)
                .SetEase(Ease.InSine)
                .SetRecyclable(true);
            
            var tweenOpenDown =  _transitionDown.DOSizeDelta(Vector2.zero, _duration / 2.0f)
                .SetEase(Ease.InSine)
                .SetRecyclable(true);

            await UniTask.WhenAll(tweenOpenUp.ToUniTask(), tweenOpenDown.ToUniTask());
            
            _canvas.enabled = false;
        }

        public void TransitionMonoBehaviour(ScreenState screen, Action action = null)
        {
            switch (screen)
            {
                case ScreenState.Lobby:
                    TransitionLobby(action);
                    break;
                
                case ScreenState.InGame:
                    TransitionInGame(action);
                    break;
                
                default: throw new ArgumentException(nameof(screen));
            }
        }

        private void TransitionLobby(Action action = null)
        {
            Transition(ScreenLobby, action).Forget();
        }
        
        private void TransitionInGame(Action action = null)
        {
            Transition(ScreenInGame, action).Forget();
        }

        [Button, HideInEditorMode]
        private async void TestTransition()
        {
            Debug.Log("Start Screen Transition");
            await Transition(null, null);
            Debug.Log("End Screen Transition");
        }

        private void ScreenLobby()
        {
            _screenState = ScreenState.Lobby;
            MessageBroker.Default.Publish(new ClearInGame());
            MessageBroker.Default.Publish(new EnterScreen(ScreenState.Lobby));

            if (OutGame.Exist)
            {
                OutGame.Instance.gameObject.SetActive(true);
                OutGame.Instance.Reset();   
            }

            _goLobby.ForEach(go => go.SetActive(true));
            _goInGame.ForEach(go => go.SetActive(false));
        }

        private void ScreenInGame()
        {
            _screenState = ScreenState.InGame;
            MessageBroker.Default.Publish(new ClearLobby());
            MessageBroker.Default.Publish(new EnterScreen(ScreenState.InGame));

            if (OutGame.Exist)
            {
                OutGame.Instance.gameObject.SetActive(false);   
            }

            _goLobby.ForEach(go => go.SetActive(false));
            _goInGame.ForEach(go => go.SetActive(true));
        }
    }
}