using System.Collections;
using System.Collections.Generic;
using OutGameCore;
using SH.Constant;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace SH.UI.Data
{
    [RequireComponent(typeof(Text))]
    [RequireComponent(typeof(ZenjectBinding))]
    public class TextGameVersion : MonoBehaviour
    {
        [Inject]
        private void Construct()
        {
            var text = GetComponent<Text>();
            var version = StaticSet.Version;
            // GameUtils.I2Format(text, "UISettings_Versions", version);
            text.text = $"Ver.{version}";
        }
    }
}
