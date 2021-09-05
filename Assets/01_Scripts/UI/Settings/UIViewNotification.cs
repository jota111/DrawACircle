using System.Collections;
using System.Collections.Generic;
using SH.Game.DataLocal;
using SH.Game.Manager;
using SH.UI.View;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace SH.Game.Settings
{
    public class UIViewNotification : UIViewBase
    {
        [Inject] private readonly UserData.UserData userData;
        private SettingsLocal _settingsLocal
        {
            get => userData.Settings;
            set => userData.Settings = value;
        }
        [SerializeField] private Button Btn_Notification;
        [SerializeField] private Button Btn_ChestNoti;
        [SerializeField] private Button Btn_FreeProductNoti;
        [SerializeField] private Button Btn_EnergyNoti;
        [SerializeField] private GameObject[] Obj_Notification = new GameObject[2];
        [SerializeField] private GameObject[] Obj_ChestNoti = new GameObject[2];
        [SerializeField] private GameObject[] Obj_FreeProductNoti = new GameObject[2];
        [SerializeField] private GameObject[] Obj_EnergyNoti = new GameObject[2];
        [Inject] private LocalNotificationManager _localNotificationManager;

        private void Awake()
        {
            Btn_Notification.OnClickAsObservable().Subscribe(_ => { OnNotificationButtonClicked(); }).AddTo(this);
            Btn_ChestNoti.OnClickAsObservable().Subscribe(_ => { OnChestNotiButtonClicked(); }).AddTo(this);
            Btn_FreeProductNoti.OnClickAsObservable().Subscribe(_ => { OnFreeProductNotiButtonClicked(); }).AddTo(this);
            Btn_EnergyNoti.OnClickAsObservable().Subscribe(_ => { OnEnergyNotiButtonClicked(); }).AddTo(this);
        }

        protected override void OnStartShow()
        {
            base.OnStartShow();
            SetButtons();
        }

        public void SetButtons()
        {
            Obj_Notification[0].SetActive(!_settingsLocal.OptionNotification.Value);
            Obj_Notification[1].SetActive(_settingsLocal.OptionNotification.Value);
            Obj_ChestNoti[0].SetActive(!_settingsLocal.OptionNotification_Chest.Value);
            Obj_ChestNoti[1].SetActive(_settingsLocal.OptionNotification_Chest.Value);
            Obj_FreeProductNoti[0].SetActive(!_settingsLocal.OptionNotification_DailyDeals.Value);
            Obj_FreeProductNoti[1].SetActive(_settingsLocal.OptionNotification_DailyDeals.Value);
            Obj_EnergyNoti[0].SetActive(!_settingsLocal.OptionNotification_Energy.Value);
            Obj_EnergyNoti[1].SetActive(_settingsLocal.OptionNotification_Energy.Value);
        }

        public void OnNotificationButtonClicked()
        {
            GameSoundManager.Instance.PlaySfx(SFX.sh_common_click);
            _settingsLocal.OptionNotification.Value = !_settingsLocal.OptionNotification.Value;
            SetButtons();
        }

        public void OnChestNotiButtonClicked()
        {
            GameSoundManager.Instance.PlaySfx(SFX.sh_common_click);
            _settingsLocal.OptionNotification_Chest.Value = !_settingsLocal.OptionNotification_Chest.Value;
            SetButtons();
        }

        public void OnFreeProductNotiButtonClicked()
        {
            GameSoundManager.Instance.PlaySfx(SFX.sh_common_click);
            _settingsLocal.OptionNotification_DailyDeals.Value = !_settingsLocal.OptionNotification_DailyDeals.Value;
            SetButtons();
        }

        public void OnEnergyNotiButtonClicked()
        {
            GameSoundManager.Instance.PlaySfx(SFX.sh_common_click);
            _settingsLocal.OptionNotification_Energy.Value = !_settingsLocal.OptionNotification_Energy.Value;
            SetButtons();
        }
    }
}