/*
* Copyright (c) CookApps.
* 이진호(jhlee8@cookapps.com)
*/

using System.Text;
using SH.AppEvent;
using SH.Constant;
using SH.Game.Manager;
using UniRx;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace SH.UI.Btn
{
    [RequireComponent(typeof(Button))]
    public class ButtonHelpMail : MonoBehaviour
    {
        private void Start()
        {
            GetComponent<Button>()?.OnClickAsObservable()
                .Subscribe(_ => Click()).AddTo(this);
        }

        private void Click()
        {
            GameSoundManager.Instance.PlaySfx(SFX.sh_common_click);
            HelpMail.SendMail();
        }
    }
}