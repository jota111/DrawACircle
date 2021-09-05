using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using OutGameCore;
using SH.Game.Manager;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace SH.UI.Fund
{
    public class RectJewel : MonoBehaviour
    {
        [Inject]
        private void Construct()
        {
            var rect = GetComponent<RectTransform>();
            var textJewel = GetComponentInChildren<TextJewel>();
            Tweener moveTweener = null;
            var initPos = rect.localPosition;
            NotifyLack.Receive("Diamonds")
                .Subscribe(async _ =>
                {
                    moveTweener?.Kill();
                    rect.localPosition = initPos;
                    var text = textJewel.GetComponent<Text>();
                    moveTweener = rect.DOShakePosition(0.5f, new Vector3(17,0,0), 20, 90f);
                    var red = new Color(0.7735849f, 0.3809455f, 0f, 1f);
                    await text.DOColor(red, 0.1f);
                    await text.DOColor(textJewel.initColor, 0.1f);
                    await text.DOColor(red, 0.1f);
                    await text.DOColor(textJewel.initColor, 0.1f);
                })
                .AddTo(this);
        }
    }
}
