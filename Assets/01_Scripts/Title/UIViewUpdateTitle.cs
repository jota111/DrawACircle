/*
* Copyright (c) CookApps.
* 이진호(jhlee8@cookapps.com)
*/

using Doozy.Engine.UI;
using SH.Constant;
using Sirenix.OdinInspector;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace SH.Title
{
    [RequireComponent(typeof(UIView))]
    public class UIViewUpdateTitle : MonoBehaviour
    {
        [SerializeField, Required, ChildGameObjectsOnly]
        private Button _buttonUpdate;

        private void Start()
        {
            _buttonUpdate.OnClickAsObservable()
                .Subscribe(_ => MoveToStore())
                .AddTo(this);
        }

        public void Show()
        {
            if (gameObject == null)
                return;
            
            GetComponent<UIView>()?.Show();
        }
        
        private void MoveToStore()
        {
            StoreInfo.MoveToStore();
        }
    }
}