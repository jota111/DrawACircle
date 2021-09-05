/*
* Copyright (c) CookApps.
* 이진호(jhlee8@cookapps.com)
*/

using System;
using Doozy.Engine.UI;
using SH.Constant;
using SH.Game.Misc;
using SH.Net;
using SH.Net.Pkt;
using SH.Ob;
using Sirenix.OdinInspector;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace SH.UI.View.Update
{
    [RequireComponent(typeof(UIView))]
    public class UIViewUpdate : MonoBehaviour
    {
        [SerializeField, Required, ChildGameObjectsOnly]
        private Button _buttonUpdate;

        private void Awake()
        {
            _buttonUpdate.OnClickAsObservable()
                .Subscribe(_ => MoveToStore())
                .AddTo(this);

            Observable.Interval(TimeSpan.FromMinutes(30)).StartWith(-1)
                .Delay(TimeSpan.FromSeconds(5))
                .Subscribe(_ => CheckUpdate())
                .AddTo(this);
        }

        private async void CheckUpdate()
        {
            var countryCode = await CountryCode.AsyncCountryCode();
            var recv = await NetManager.Post<RecvVersion>(new ReqVersion(countryCode));
            if (recv == null || !recv.result)
                return;

            if (recv.data == null || !recv.data.force_update)
                return;

            var targetVersion = recv.data.version;
            if (StaticSet.VersionNumber < targetVersion)
            {
                Show();
            }
        }

        private void Show()
        {
            if (gameObject == null)
                return;
#if  UNITY_EDITOR
            return;
#endif
            
            GetComponent<UIView>()?.Show();
        }

        private void MoveToStore()
        {
            StoreInfo.MoveToStore();
        }
    }
}