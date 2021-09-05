using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using OutGameCore;
using SH.Constant;
using SH.Game.Settings;
using SH.UI.Btn;
using Sirenix.OdinInspector;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace SH.Game.Manager
{
    public class LobbyUIManager : MonoBehaviour
    {
        public static LobbyUIManager Instance { get; private set; }
        public static float CanvasScale = 1f;
        private List<LobbyButton> _lobbyButtons = new List<LobbyButton>();
        private LobbyButton.LobbyBtnType _currentBtnType = LobbyButton.LobbyBtnType.Default;
        [SerializeField] private VerticalLayoutGroup _verticalLayoutGroup;

        [Inject]
        private void Construct()
        {
            Instance = this;
            _verticalLayoutGroup.enabled = true;
            CanvasScale = GetComponent<RectTransform>().localScale.x;
            _lobbyButtons = GetComponentsInChildren<LobbyButton>(true).ToList();
            Show();
        }

        [Button]
        public void Show(LobbyButton.LobbyBtnType btnType = LobbyButton.LobbyBtnType.Default, float duration = 0.5f)
        {
            var rect = GetComponent<RectTransform>();
            var rightPos = rect.sizeDelta.x / 2f;
            var leftPos = -rect.sizeDelta.x / 2f;
            
            _lobbyButtons.ForEach(x =>
            {
                if(x.GetLobbyBtnType == btnType)
                    x.Show(duration);
                else
                {
                    if (x.HideType == ILobbyButton.LobbyHideType.Right)
                    {
                        x.Hide(new Vector2(rightPos + x.RectTransform.sizeDelta.x, x.InitPos.y), duration);
                    }
                    else
                    {
                        x.Hide(new Vector2(leftPos - x.RectTransform.sizeDelta.x, x.InitPos.y), duration);
                    }
                }
            });
            _currentBtnType = btnType;
        }
        
        [Button]
        public void Hide(float duration = 0.3f)
        {
            var rect = GetComponent<RectTransform>();
            var rightPos = rect.sizeDelta.x / 2f;
            var leftPos = -rect.sizeDelta.x / 2f;
            
            _lobbyButtons.ForEach(x =>
            {
                if (x.HideType == ILobbyButton.LobbyHideType.Right)
                {
                    x.Hide(new Vector2(rightPos + x.RectTransform.sizeDelta.x, x.InitPos.y), duration);
                }
                else
                {
                    x.Hide(new Vector2(leftPos - x.RectTransform.sizeDelta.x, x.InitPos.y), duration);
                }
            });
            _currentBtnType = LobbyButton.LobbyBtnType.Default;
        }

        public async void UpdateInGameBtnPos()
        {
            _verticalLayoutGroup.SetLayoutVertical();
            await UniTask.DelayFrame(1);
            _lobbyButtons.FindAll(x =>
                x.GetLobbyBtnTutorialType == LobbyButton.LobbyBtnTutorialType.InGameBtn ||
                x.GetLobbyBtnTutorialType == LobbyButton.LobbyBtnTutorialType.EventBtn).ForEach(x=>x.UpdateInitPos());
        }

        private IDisposable timer;
        private Dictionary<string, LobbyButton.LobbyBtnType> uiStack = new Dictionary<string, LobbyButton.LobbyBtnType>();
        public void Push(string key, LobbyButton.LobbyBtnType btnType = LobbyButton.LobbyBtnType.Default)
        {
            timer?.Dispose();
            // 현재 뷰랑 들어온 뷰랑 다르면 적용
            if (btnType == LobbyButton.LobbyBtnType.SelectShow)
            {
                if (_currentBtnType != btnType)
                    Show(btnType);
            }
            //여태까지 카운트 0이었으면 
            else if (uiStack.Count == 0)
                Hide();
            uiStack.TryAdd(key, btnType);
        }
        
        public void Pop(string key)
        {
            timer?.Dispose();
            uiStack.Remove(key);

            if (uiStack.Count > 0)
            {
                if (_currentBtnType == LobbyButton.LobbyBtnType.SelectShow && uiStack.Last().Value != _currentBtnType)
                    Hide();
            }

            if (uiStack.Count == 0)
            {
                timer = Observable.Timer(TimeSpan.FromSeconds(TopUIManager.ShowDelay)).Subscribe(_ => { Show();}).AddTo(this);
            }
        }

        [Button]
        public void Test_Push(LobbyButton.LobbyBtnType type)
        {
            Push($"{uiStack.Count}", type);
        }

        [Button]
        public void Test_Pop()
        {
            if (uiStack.Count > 0)
                Pop(uiStack.Last().Key);
        }

        private void OnDestroy()
        {
            timer?.Dispose();
            timer = null;
        }
    }
}
