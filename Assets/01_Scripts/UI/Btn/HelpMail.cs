/*
* Copyright (c) CookApps.
* 이진호(jhlee8@cookapps.com)
*/

using System.Text;
using SH.Game.UserData;
using SH.Game.UserManager;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Networking;

namespace SH.UI.Btn
{
    public class HelpMail : MonoBehaviour, IPointerClickHandler
    {
        public static void SendMail()
        {
            string mailto = "sunnyhouse_support@cookapps.com";
            string subject = EscapeURL("[SunnyHouse] Inquiry Mail");
        
            StringBuilder bodyText = new StringBuilder();
            bodyText.AppendLine("Please fill in here\n\n\n\n\n\n\nSystem Info");
            bodyText.AppendFormat
            (
                "Ver.{0} \nOs.{1}\nApp.{2}\nDevice:{3} {4}\nLanguage:{5} \nDeviceID:{6} \nUserID:{7}\n\n\n",
                Application.version,
                Application.platform.ToString(),
                Application.productName,
                SystemInfo.deviceModel,
                SystemInfo.deviceName,
                Application.systemLanguage,
                SystemInfo.deviceUniqueIdentifier,
                UserIdManager.Instance.Id
            );
        
            string body = EscapeURL(bodyText.ToString());
            string gull = "mailto:" + mailto + "?subject=" + subject + "&body=" + body;
            Application.OpenURL(gull);
        }

        private static string EscapeURL(string url)
        {
            return UnityWebRequest.EscapeURL(url, Encoding.UTF8).Replace("+", "%20");
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            SendMail();
        }
    }
}