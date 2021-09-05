using System;
using Cysharp.Threading.Tasks;
using LunarConsolePlugin;
using SH.Constant;
using SH.Data;
using SH.Game.DataLocal;
using SH.Game.Manager;
using SH.Game.UserData;
using SH.UI;
using SH.Util;
using Sirenix.OdinInspector;
using UniRx;
using UnityEngine;
using Zenject;

namespace OutGameCore
{
    public partial class OutGame : MonoBehaviour
    {
        public async UniTask LoadTitleScene()
        {
            await DestroySelf();
            SceneLoad.LoadScene(SceneType.SceneTitle);
        }

        public static async UniTask DestroySelf()
        {
            if (Instance == null) return;
            Destroy(Instance.gameObject);
            await UniTask.NextFrame();
        }
        
        #region 테스트

        [Button(ButtonSizes.Medium), LabelText("모든 데이터 삭제")]
        public void RemoveAllDatas()
        {
            try
            {
                System.IO.Directory.Delete(Application.persistentDataPath, true); // 데이터 삭제
                PlayerPrefs.DeleteAll();
            }
            catch (Exception ex)
            {
                GameUtils.LogException(ex);
                //Firebase.Crashlytics.Crashlytics.LogException(ex);
            }
        }

        public void Test_LunarStart()
        {

            LunarConsole.RegisterAction("상자 로컬 푸시 추가",
                () => { UserData.Instance.Notification.AddNotification(LocalPushType.Chest, TimeUtil.Now.AddMinutes(1)); });
            LunarConsole.RegisterAction("에너지 1 사용", () => { EnergyManager.Instance.UseEnergy(); });
        }

        public void Test_LunarDestroy()
        {
            LunarConsole.UnregisterAllActions(this);
        }

        #endregion
    }

    [CVarContainer]
    public static class LunarVariables
    {
        public static readonly CVar questVar = new CVar("해당 퀘스트까지 모두 클리어", 1000);
        public static readonly CVar timeHourVar = new CVar("시간값(시간)", 0);
        public static readonly CVar timeMinuteVar = new CVar("시간값(분)", 0);
    }
}