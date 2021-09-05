using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using MagneticScrollView;
using OutGameCore;
using SH.Game.Manager;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Community
{
    public class MagneticScrollTabController : MonoBehaviour
    {
        [SerializeField] private MagneticScrollRect magneticScrollRect;
        [SerializeField] private GameObject Obj_TabMove;
        [SerializeField] private GameObject[] Obj_TabBase;
        [SerializeField] private GameObject[] Obj_TabBaseOn;
        [SerializeField] private GameObject[] Obj_TabBaseOff;
        [SerializeField] private float TabOnScaleValue = 10f;
        private Tweener movingTweener;

        public void OnSelectionChange()
        {
            GameUtils.Log($"MagneticScrollTabContrller OnSelectionChange {magneticScrollRect.CurrentSelectedIndex}");
            MovingTabComplete(magneticScrollRect.CurrentSelectedIndex);
        }

        public void OnScrolling()
        {
            // GameUtils.Log($"MagneticScrollTabController : {magneticScrollRect.NormalizedScrollAngle * (-magneticScrollRect.Elements.Length)} / {magneticScrollRect.ScrollAngle}");
            var value = magneticScrollRect.NormalizedScrollAngle * (-magneticScrollRect.Elements.Length);
            GameUtils.Log($"MagneticScrollTabContrller OnScrolling {value}");
            MovingTab(value);
        }

        public void MovingTab(float value)
        {
            int tab = (int) value;
            float moving = value - tab;
            var pos = Obj_TabMove.transform.position;
            if (Obj_TabBase.Length > tab + 1)
                pos = (Obj_TabBase[tab].transform.position * (1 - moving)) + (Obj_TabBase[tab + 1].transform.position * moving);
            else pos = Obj_TabBase[tab].transform.position;
            Obj_TabMove.transform.position = pos;
        }

        public void MovingTabComplete(int currentTab)
        {
            for (int i = 0; i < Obj_TabBase.Length; i++)
            {
                if (i == currentTab)
                {
                    Obj_TabBaseOn[i]?.SetActive(true);
                    Obj_TabBaseOff[i]?.SetActive(false);
                    Obj_TabBase[currentTab].GetComponent<LayoutElement>().flexibleWidth = TabOnScaleValue;
                    Obj_TabBase[currentTab].GetComponent<LayoutElement>().preferredWidth = Obj_TabBase[currentTab].GetComponent<LayoutElement>().minWidth * 10;
                }
                else
                {
                    Obj_TabBaseOn[i]?.SetActive(false);
                    Obj_TabBaseOff[i]?.SetActive(true);
                    Obj_TabBase[i].GetComponent<LayoutElement>().flexibleWidth = 0;
                    Obj_TabBase[i].GetComponent<LayoutElement>().preferredWidth = 0;
                }
            }

            Observable.TimerFrame(1).Subscribe(x => { Obj_TabMove.transform.position = Obj_TabBase[currentTab].transform.position; }).AddTo(this);
        }

        // public void MovingTab(int target)
        // {
        //     movingTweener?.Kill();
        //     movingTweener = Obj_TabMove.transform.DOMoveX(Obj_TabBaseOff[target].transform.position.x, 1f).OnComplete(() =>
        //     {
        //         MovingTabComplete(target);
        //     });
        // }

        public void OnTabClicked(int tabIndex)
        {
            GameUtils.Log($"MagneticScrollTabContrller OnTabClicked {tabIndex}");
            GameSoundManager.Instance.PlaySfx(SFX.sh_common_click);
            magneticScrollRect.ScrollTo(tabIndex);
        }
    }
}