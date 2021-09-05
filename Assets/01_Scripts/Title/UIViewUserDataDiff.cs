/*
* Copyright (c) CookApps.
* 이진호(jhlee8@cookapps.com)
*/

using System;
using Cysharp.Threading.Tasks;
using Doozy.Engine.UI;
using SH.Game.UserData;
using SH.Game.UserManager;
using SH.Util.UniRx;
using Sirenix.OdinInspector;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace SH.Title
{
    [RequireComponent(typeof(UIView))]
    public class UIViewUserDataDiff : MonoBehaviour
    {
        [SerializeField, Required, ChildGameObjectsOnly]
        private ViewUserData _userServer;
        
        [SerializeField, Required, ChildGameObjectsOnly]
        private ViewUserData _userLocal;
        
        [SerializeField, Required, ChildGameObjectsOnly]
        private Button _buttonSelect;

        private void Awake()
        {
            _buttonSelect.interactable = false;
            _userServer.OnToggleValueChanged().CombineLatest(_userLocal.OnToggleValueChanged(), (a, b) => a || b)
                .SubscribeToInteractable(_buttonSelect)
                .AddTo(this);
        }

        public async UniTask<UserData> ShowFirst(UserData localData, UserData serverData)
        {
            _userLocal.EnableInitData();
            return await Show(localData, serverData);
        }

        public async UniTask<UserData> Show(UserData localData, UserData serverData)
        {
            _userServer.SetData(serverData);
            _userLocal.SetData(localData);
            Show();

            var obsSelect = _buttonSelect.OnClickAsObservable().Select(_ =>
            {
                if (_userServer.IsSelect)
                    return _userServer.Data;
                if (_userLocal.IsSelect)
                    return _userLocal.Data;
                return null;
            });
                

            var userData = await obsSelect.WhereNotNull().First().ToUniTask(cancellationToken:this.GetCancellationTokenOnDestroy());
            await UniTask.Delay(TimeSpan.FromSeconds(0.2f));
            Hide();
            await UniTask.Delay(TimeSpan.FromSeconds(0.5));
            
            return userData;
        }

        public void Hide()
        {
            GetComponent<UIView>()?.Hide();
        }

        private void Show()
        {
            GetComponent<UIView>()?.Show();
            GetComponent<RectTransform>()?.SetAsLastSibling();
        }
    }
}