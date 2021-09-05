/*
* Copyright (c) CookApps.
* 이진호(jhlee8@cookapps.com)
*/

using System;
using Beebyte.Obfuscator;
using Cysharp.Threading.Tasks;
using Doozy.Engine.UI;
using Newtonsoft.Json.Linq;
using OutGameCore;
using SH.AppEvent;
using SH.Constant;
using SH.Game.Misc;
using SH.Game.UserData;
using SH.Game.UserManager;
using SH.Net;
using SH.Net.Pkt;
using SH.Ob;
using SH.RuntimeInit;
using SH.Util;
using Sirenix.OdinInspector;
using UniRx;
using UnityEngine;
using UnityEngine.Networking;

namespace SH.Title
{
    /// <summary>
    /// 타이틀 신
    /// </summary>
    public class SceneTitle : MonoBehaviour
    {
        [SerializeField, Required]
        private UIViewUpdateTitle _update;
        
        [SerializeField, Required]
        private UIViewUserDataDiff _userDataDiff;
        
        [SerializeField, Required]
        private UIViewApplePermission _applePermission;
        
        [SerializeField, Required]
        private UIView _checker;
        
        /// <summary>
        /// 최소 대기 로딩 시간
        /// </summary>
        const float MinLoadDuration = 1;

        private void Awake()
        {
            
        }

        private async void Start()
        {
            // 모든 작업 완료 기다리자
            var (loadScene, _, _, _, _, _) = await UniTask.WhenAll(
                LoadSceneGame(),
                WaitLoad(),
                VersionCheck(),
                Login(),
                OutGame.AsyncLoadOutGame(),
                InitLoadManager.AsyncLoad());

            //앱이벤트
            AppEventManager.ReportProgress(AppEventManager.FirstProgress.Loading_Resources_Finished).Forget();

            // 저장 데이터 버전 비교
            await CheckUserSaveVersion();

            // 애플 권한
            await _applePermission.CheckAndShow();

            // 불법 체크
            var check = AppCheck.IsPass.Value;
            if (!check)
            {
                _checker.Show();
                return;
            }

            // Scene 전환
            loadScene.allowSceneActivation = true;
        }

        /// <summary>
        /// 최소 대기시간
        /// </summary>
        /// <returns></returns>
        private async UniTask<Unit> WaitLoad()
        {
            await UniTask.Delay(TimeSpan.FromSeconds(MinLoadDuration));
            return Unit.Default;
        }

        /// <summary>
        /// OutGame로드
        /// </summary>
        /// <returns></returns>
        private async UniTask<Unit> LoadOutGame()
        {
            await UniTask.NextFrame();
            if (!OutGame.Exist)
            {
                OutGame.InstantiateOutGame();
            }

            OutGame.Instance.Init();
            return Unit.Default;
        }

        /// <summary>
        /// 게임신 로드
        /// </summary>
        /// <returns></returns>
        private async UniTask<AsyncOperation> LoadSceneGame()
        {
            return await SceneLoad.LoadSceneAsync(SceneType.SceneGame);
        }

        /// <summary>
        /// 로그인 & 유저 정보 받아오기
        /// </summary>
        /// <returns></returns>
        private async UniTask<Unit> Login()
        {
            await UniTask.NextFrame();
            
#if UNITY_EDITOR
            if (TestAccount.TestMode)
            {
                return await RunTestAccount();
            }
#endif
            
            var recv = await NetManager.AsyncPost<RecvLogin>(new ReqLogin());
            if (!recv.result)
                return Unit.Default;;

            var uid = recv.data.myInfo.ca_uid;
            UserIdManager.Instance.SetId(uid);

            var serverJson = recv.data.saveData;
            if (string.IsNullOrEmpty(serverJson))
                return Unit.Default;

            var serverData = UserDataSaveManager.Load(serverJson);
            if(serverData == null)
                return Unit.Default;
            // 저장된 데이타가 존재하면
            if (UserDataSaveManager.ExistSaveData())
            {
                await CheckUserSaveVersion();
                var localData = UserDataSaveManager.Load();
                var latest = localData.LatestComparison(serverData);
                // 데이타가 확정정이라면
                if (latest.definitive)
                {
                    // 기존의 로컬데이타가 아니면 저장
                    if(latest.userData != localData)
                        UserDataSaveManager.Save(latest.userData);
                    return Unit.Default;
                }
                return await UserDataDiff(localData, serverData, serverJson, false);
            }
            // 저장된 데이타가 없으면 처음부터 할것인지
            else
            {
                var localData = UserDataSaveManager.InitUserData();
                return await UserDataDiff(localData, serverData, serverJson, true);
            }
        }

#if UNITY_EDITOR
        private async UniTask<Unit> RunTestAccount()
        {
            try
            {
                var url = $"https://sunnyhouse.cookappsgames.com/api/user/{TestAccount.Uid}";
                var req = UnityWebRequest.Get(url);
                var data = await req.SendWebRequest();
                var json = data.downloadHandler.text;
                Debug.Log($"TestAccount uid ({TestAccount.Uid})\n{json}");
                UserDataSaveManager.Save(json);
                return Unit.Default;
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                Debug.LogError($"TestAccount Error uid {TestAccount.Uid}");
                await UniTask.Never(this.GetCancellationTokenOnDestroy());
                return Unit.Default; 
            }
        }
#endif

        /// <summary>
        /// 게임 버전 체크
        /// </summary>
        /// <returns></returns>
        private async UniTask<Unit> VersionCheck()
        {
            await UniTask.NextFrame();
            var countryCode = await CountryCode.AsyncCountryCode();
            var recv = await NetManager.AsyncPost<RecvVersion>(new ReqVersion(countryCode));
            if (!recv.result)
                return Unit.Default;

            if (recv.data == null || !recv.data.force_update)
                return Unit.Default;

            var targetVersion = recv.data.version;
            if (StaticSet.VersionNumber >= targetVersion) 
                return Unit.Default;
            
#if  UNITY_EDITOR
            return Unit.Default;
#endif
            ShowUpdate();
            
            // 무제한 대기
            await UniTask.Never(this.GetCancellationTokenOnDestroy());
            return Unit.Default;
        }

        private void ShowUpdate()
        {
#if __DEV
            return;
#endif
            _userDataDiff.Hide();
            _update.Show();
        }

        private async UniTask<Unit> UserDataDiff(UserData localData, UserData serverData, string serverJson, bool initData)
        {
            // 업데이트가 필요하면 그냥 리턴
            if (_update.gameObject.activeSelf)
                return Unit.Default;

            UserData userData = null;
            if(initData)
                userData = await _userDataDiff.ShowFirst(localData, serverData);
            else
                userData = await _userDataDiff.Show(localData, serverData);

            if(userData == localData)
                UserDataSaveManager.Save(userData);
            else
                UserDataSaveManager.Save(serverJson);
            
            return Unit.Default;
        }

        private async UniTask CheckUserSaveVersion()
        {
            var saveUserDataVersion = UserDataSaveManager.GetSaveUserDataVersion();
            // 게임버전이 더 높으면 진행
            if (saveUserDataVersion <= UserData.CompatibleDataVersion)
                return;

            // 아니면 업데이트
            ShowUpdate();
            // 무제한 대기
            await UniTask.Never(this.GetCancellationTokenOnDestroy());
        }
    }
}