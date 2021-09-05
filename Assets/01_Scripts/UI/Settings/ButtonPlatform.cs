/*
* Copyright (c) CookApps.
* 이진호(jhlee8@cookapps.com)
*/

using System;
using Cysharp.Threading.Tasks;
using OutGameCore;
using SH.Constant;
using SH.Game.Misc;
using SH.Game.UserData;
using SH.Game.UserManager;
using SH.Net;
using SH.Net.Pkt;
using SH.Platform;
using SH.Title;
using SH.UI.View.Dialog;
using SH.Util.UniRx;
using Sirenix.OdinInspector;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace SH.UI.Settings
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Button))]
    public abstract class ButtonPlatform : MonoBehaviour
    {
        [SerializeField, Required, ChildGameObjectsOnly]
        private Text _textStatus;
        
        [SerializeField] private string _termLogin;
        [SerializeField] private string _termAlreadyLogin;
        
        private IIndicator _indicator;
        private UIViewDialogPlatformLoginSuccess _viewDialogPlatformLoginSuccess;
        private UIViewUserDataDiff _viewUserDataDiff;
        private UIViewManager _viewManager;
        protected UserData UserData { get; private set; }


        [Inject]
        protected void Construct(UserData userData, IIndicator indicator,
            UIViewDialogPlatformLoginSuccess viewDialogPlatformLoginSuccess,
            UIViewUserDataDiff viewUserDataDiff,
            UIViewManager viewManager)
        {
            _viewManager = viewManager;
            _viewUserDataDiff = viewUserDataDiff;
            _viewDialogPlatformLoginSuccess = viewDialogPlatformLoginSuccess;
            _indicator = indicator;
            UserData = userData;
        }

        private void Start()
        {
            GetComponent<Button>()
                .OnClickSoundAsObservable()
                .Where(_ => !IsLogin())
                .Subscribe(_ => OnClick())
                .AddTo(this);
        }

        private void OnEnable()
        {
            var term = IsLogin() ? _termAlreadyLogin : _termLogin;
            _textStatus.text = GameUtils.I2Format(term);
        }

        protected void AlreadyLogin()
        {
            var termMSg = Msg();
            UIViewDialog.Instance.DialogForget(termMSg, "Popup_title_alreadylogin");
            
            //-----------------------------------------------------------------------------

            string Msg() =>
                GetPlatformType() switch
                {
                    PlatformLoginType.Google => "Popup_desc_alreadygoogle",
                    PlatformLoginType.Facebook => "Popup_desc_alreadyfb",
                    PlatformLoginType.Apple => "Popup_desc_alreadyapple"
                };
        }

        protected void FailedLogin()
        {
            UIViewDialog.Instance.DialogForget("Popup_desc_loginerr", "Popup_title_loginerr");
        }

        private void AsyncLoginSuccessMoveTitle()
        {
            var termMSg = Msg();
            _viewDialogPlatformLoginSuccess.Show(termMSg);
            
            //-----------------------------------------------------
            string Msg() =>
                GetPlatformType() switch
                {
                    PlatformLoginType.Google => "Popup_desc_googlesuccess",
                    PlatformLoginType.Facebook => "Popup_desc_fbsuccess",
                    PlatformLoginType.Apple => "Popup_desc_applesuccess"
                };
        }
        
        protected async UniTask<bool> AsyncServer(PlatformLoginId platformLoginId)
        {
            var recv = await NetManager.AsyncPost<RecvLogin>(new ReqLogin(platformLoginId));
            if (!recv.result)
            {
                return false;
            }

            _viewManager.HideAll();
            var uid = recv.data.myInfo.ca_uid;
            var serverJson = recv.data.saveData;
            // 서버에 데이타가 없으면 내것으로 바로 저장
            if (string.IsNullOrEmpty(serverJson))
            {
                return await AsyncSaveAndLoginSuccessMoveTitle(UserData, uid, platformLoginId);
            }

            UserData selectData = null;
            var serverData = UserDataSaveManager.Load(serverJson);
            
            // 무조건 선택 물어보기
            // var latest = UserData.LatestComparison(serverData);
            //
            // if (latest.definitive)
            // {
            //     selectData = latest.userData;
            // }
            // else
            {
                selectData = await _viewUserDataDiff.Show(UserData, serverData);   
            }

            // 선택버전이 지금의 버전보다 크면 서버에서 받은 json 문자열저장
            if (UserData.CompatibleDataVersion < selectData.DataVersion)
                return await AsyncLocalSaveAndLoginSuccessMoveTitle(serverJson, uid);
            return await AsyncSaveAndLoginSuccessMoveTitle(selectData, uid, platformLoginId);
        }

        /// <summary>
        /// 서버에 저장하고 타이틀로
        /// </summary>
        /// <param name="userData"></param>
        /// <returns></returns>
        private async UniTask<bool> AsyncSaveAndLoginSuccessMoveTitle(UserData userData, long recv_ca_uid, PlatformLoginId platformLoginId)
        {
            var uidOrg = UserIdManager.Instance.Id;
            UserIdManager.Instance.SetId(recv_ca_uid);
            UserDataSaveManager.Instance.EnableAutoSave = false;
            UserDataSaveServerManager.Instance?.SetState(false);
            if (!await NetManager.AsyncPostSuccess(new ReqSave(userData, platformLoginId)))
            {
                UserDataSaveServerManager.Instance?.SetState(true);
                UserDataSaveManager.Instance.EnableAutoSave = true;
                UserIdManager.Instance.SetId(uidOrg);
                return false;
            }
            UserDataSaveManager.Save(userData);
            AsyncLoginSuccessMoveTitle();
            return true;
        }

        /// <summary>
        /// 로컬만 저장
        /// </summary>
        /// <param name="userData"></param>
        /// <param name="recv_ca_uid"></param>
        /// <returns></returns>
        private async UniTask<bool> AsyncLocalSaveAndLoginSuccessMoveTitle(string userData, long recv_ca_uid)
        {
            UserDataSaveServerManager.Instance?.SetState(false);
            UserDataSaveManager.Instance.EnableAutoSave = false;
            UserDataSaveManager.Save(userData);
            UserIdManager.Instance.SetId(recv_ca_uid);
            AsyncLoginSuccessMoveTitle();
            return true;
        }
        
        /// <summary>
        /// 로컬만 저장
        /// </summary>
        /// <param name="userData"></param>
        /// <param name="recv_ca_uid"></param>
        /// <returns></returns>
        private async UniTask<bool> AsyncLocalSaveAndLoginSuccessMoveTitle(UserData userData, long recv_ca_uid)
        {
            UserDataSaveServerManager.Instance?.SetState(false);
            UserDataSaveManager.Instance.EnableAutoSave = false;
            UserDataSaveManager.Save(userData);
            UserIdManager.Instance.SetId(recv_ca_uid);
            AsyncLoginSuccessMoveTitle();
            return true;
        }

        protected IDisposable ScopeIndicator() => _indicator.ScopeLock();

        protected abstract void OnClick();

        protected abstract PlatformLoginType GetPlatformType();
        protected abstract bool IsLogin();
    }
}