using System.Collections;
using System.Collections.Generic;
using SH.Game.Manager;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Community.Profile
{
    public class UIItem_ProfileImage : MonoBehaviour
    {
        [SerializeField] private UIViewProfileImage uiViewProfileImage;
        [SerializeField] private Image Img_Selected;
        [SerializeField] Button Btn_Selected;
        public string Key;

        public void Awake()
        {
            Btn_Selected.OnClickAsObservable().Subscribe(_ => OnImageSelected()).AddTo(this);
        }

        public void SetSelected(bool onOff)
        {
            Img_Selected.gameObject.SetActive(onOff);
        }

        public void OnImageSelected()
        {
            GameSoundManager.Instance.PlaySfx(SFX.sh_common_click);
            uiViewProfileImage.SetSelected(Key);
        }
    }
}
