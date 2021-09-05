using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace SH.UI.Btn
{
    public interface ILobbyButton
    {
        public RectTransform RectTransform { get; set; }
        public bool IsShowed { get; set; }
        public Tweener Tweener { get; set; }
        public Vector2 InitPos { get; set; }
        public LobbyHideType HideType { get; set; }
        public List<Image> _images { get; set; } 
        public List<Text> _texts { get; set; }

        void Show(float duration);
        void Hide(Vector2 pos, float duration);
        public void TryInitUI();

        enum LobbyHideType
        {
            Right,
            Left,
        }
    }
}
