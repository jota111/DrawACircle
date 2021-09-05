using System.Collections;
using System.Collections.Generic;
using OutGameCore;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Community.Profile
{
    public class UIButtonSetNickname : MonoBehaviour
    {
        [Inject]
        public void Construct()
        {
            GetComponent<Button>().OnClickAsObservable().Subscribe(_ =>
            {
                UIViewSetNickname.Instance.SetView();
            }).AddTo(this);
        }
    }
}