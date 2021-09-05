using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using OutGameCore;
using SH.Game.Manager;
using SH.Game.Tutorial;
using SH.Game.UserData;
using Sirenix.OdinInspector;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace SH.UI.Btn
{
    public class LobbyButton : MonoBehaviour, ILobbyButton
    {
        public RectTransform RectTransform { get; set; }
        public bool IsShowed { get; set; } = true;
        public Tweener Tweener { get; set; }
        public Vector2 InitPos { get; set; }
        public ILobbyButton.LobbyHideType HideType { get; set; }
        public List<Image> _images { get; set; }
        public List<Text> _texts { get; set; }
        public List<Image> Images = new List<Image>();
        public List<Text> Texts = new List<Text>();
        [SerializeField] private ILobbyButton.LobbyHideType hideType;
        [SerializeField] private LobbyBtnTutorialType lobbyBtnTutorialType = LobbyBtnTutorialType.None;
        [SerializeField] private LobbyBtnType lobbyBtnType = LobbyBtnType.Default;
        public LobbyBtnType GetLobbyBtnType => lobbyBtnType;
        public LobbyBtnTutorialType GetLobbyBtnTutorialType => lobbyBtnTutorialType;

        private IContentPossible _contentPossible;
        private UserData _userData;

        [Inject]
        private void Construct(UserData userData, IContentPossible contentPossible)
        {
            RectTransform = GetComponent<RectTransform>();
            InitPos = RectTransform.localPosition;
            HideType = hideType;
            _userData = userData;
            _contentPossible = contentPossible;
        }

        public void Show(float duration)
        {
            if (IsShowed) return;
            IsShowed = true;
            TryInitUI();
            if (CheckTutorial())
            {
                _images.ForEach(x =>
                {
                    x.SetAlpha(1);
                    if (!ReferenceEquals(x.gameObject, gameObject))
                        x.gameObject.SetActive(true);
                });
                _texts.ForEach(x => x.SetAlpha(1));
                return;
            }
            Tweener?.Kill();
            // Tweener = RectTransform.DOLocalMove(InitPos, duration);
            RectTransform.localPosition = InitPos;
            _images.ForEach(x => x.Fade(true, duration).Forget());
            _texts.ForEach(x => x.Fade(true, duration).Forget());
        }

        public void Hide(Vector2 pos, float duration)
        {
            if (!IsShowed) return;
            IsShowed = false;
            TryInitUI();
            Tweener?.Kill();
            Tweener = RectTransform.DOLocalMove(pos, duration);
            _images.ForEach(x => x.Fade(false, duration).Forget());
            _texts.ForEach(x => x.Fade(false, duration).Forget());
        }

        public void UpdateInitPos()
        {
            InitPos = RectTransform.localPosition;
        }

        public bool CheckTutorial()
        {
            if (lobbyBtnTutorialType == LobbyBtnTutorialType.None) return false;

            return false;
        }

        public void TryInitUI()
        {
            if (_images == null)
            {
                _images = Images;
            }

            if (_texts == null)
            {
                _texts = Texts;
            }
        }

        [Button]
        public void SetButtons()
        {
            Images = GetComponentsInChildren<Image>().ToList();
            Texts = GetComponentsInChildren<Text>().ToList();
        }

        public enum LobbyBtnTutorialType
        {
            None = 0,
            Friends = 1,
            EventBtn = 2,
            MailBox = 3,
            StarterPack = 4,
            InGameBtn = 5,
        }

        public enum LobbyBtnType
        {
            Default = 0,
            SelectShow = 1,
        }
    }
}