using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using I2.Loc;
using OutGameCore;
using SH.Game.DataLocal;
using SH.Game.Manager;
using SH.Game.UserData;
using SH.Game.UserManager;
using SH.UI;
using SH.UI.View;
using SH.UI.View.Dialog;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace SH.Game.Settings
{
    public class UIViewLanguage : UIViewBase
    {
        [Inject] private readonly UserData.UserData userData;

        private SettingsLocal _settingsLocal
        {
            get => userData.Settings;
            set => userData.Settings = value;
        }

        [SerializeField] private Transform ItemParent;
        [SerializeField] private List<UIItemLanguage> Btn_Languages = new List<UIItemLanguage>();
        private bool initialized = false;

        private void Awake()
        {
        }

        protected override void OnStartShow()
        {
            base.OnStartShow();
            if (initialized) return;
            initialized = true;

            var languages = LocalizationManager.GetAllLanguages();

            for (int i = 0; i < Btn_Languages.Count; i++)
            {
                var index = i;
                if (languages.Count > i)
                {
                    Btn_Languages[i].gameObject.SetActive(true);
                    Btn_Languages[i].SetLanguage(i, languages[i]);
                    Btn_Languages[i].SetOnOff(languages[i] != LocalizationManager.CurrentLanguage);
                    Btn_Languages[i].Btn_Language.OnClickAsObservable().Subscribe(_ =>
                    {
                        UIViewRequest.Instance.DialogForget(
                            GameUtils.I2Format("UILanguage_ChangeDesc", GameUtils.I2Format($"UILanguage_Name{languages[index]}"))
                            , "UINameKey_Language", "Dialog_OK", "Text_Cancel", async () =>
                            {
                                UIViewManager.Instance.HideAll();
                                OnButtonClicked(languages[index]);
                            }, true, false);
                    }).AddTo(this);
                }
                else
                    Btn_Languages[i].gameObject.SetActive(false);
            }

            var cnt = Btn_Languages.FindAll(x => x.gameObject.activeSelf).Count;
            var count = cnt / 2 + cnt % 2;

            var size = ItemParent.GetComponent<RectTransform>().sizeDelta;
            ItemParent.GetComponent<RectTransform>().sizeDelta = new Vector2(size.x, 160 * count - 10);
        }

        // public void SpawnItems(List<string> languages)
        // {
        //     for (int i = 0; i < languages.Count; i++)
        //     {
        //         var item = OutGame.Instance.gamePools.Spawn<UIItemLanguage>("UIItemLanguage", true, ItemParent);
        //         Btn_Languages.Add(item);
        //     }
        // }

        public void OnButtonClicked(string language)
        {
            GameSoundManager.Instance.PlaySfx(SFX.sh_common_click);
            LocalizationManager.CurrentLanguage = language;
            _settingsLocal.SelectedLanguage = language;
            UserDataSaveManager.Instance.Save();
            UserDataSaveServerManager.Instance?.RequestAsync();
            OutGame.Instance.LoadTitleScene().Forget();
        }
    }
}