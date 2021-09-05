using I2.Loc;
using OutGameCore;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace SH.Game.Settings
{
    public class UIItemLanguage : MonoBehaviour
    {
        public Button Btn_Language;
        [SerializeField] private Text Text_Name;
        public Button Btn_LanguageOff;
        [SerializeField] private Text Text_NameOff;

        public void SetLanguage(int index, string language)
        {
            GameUtils.I2Format(Text_Name, $"UILanguage_Name{language}");
            GameUtils.I2Format(Text_NameOff, $"UILanguage_Name{language}");
        }

        public void SetOnOff(bool onOff)
        {
            Btn_Language.gameObject.SetActive(onOff);
            Btn_LanguageOff.gameObject.SetActive(!onOff);
        }
    }
}
