using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace SH.UI.Btn
{
    public class ButtonClearUIStack : MonoBehaviour
    {
        public void Start()
        {
            GetComponent<Button>()
                .OnClickAsObservable()
                .Subscribe(_ => OnClick()).AddTo(this);
        }

        private void OnClick()
        {
            UIViewManager.Instance.ClearStack();
        }
    }
}
