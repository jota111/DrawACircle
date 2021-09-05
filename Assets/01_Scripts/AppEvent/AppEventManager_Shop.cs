using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Facebook.Unity;
using SH.Util;
using UnityEngine;
using Zenject;
using Cookapps.Analytics;
using OutGameCore;
using SH.Constant;
using SH.Game.DataLocal;
using SH.Game.Manager;
using SH.Game.UserData;
using SH.Game.UserManager;

namespace SH.AppEvent
{
    public static partial class AppEventManager
    {
        public static void LogEvent_CHARGE_ENERGE(MergeBoardIdent boardIdent, string key)
        {
            GameUtils.Log($"AppEventManager LogEvent_CHARGE_ENERGE {boardIdent}");
            var logEvent = "CHARGE_ENERGE";

            parameters.Clear();
            parameters.Add("valueToSum", UserData.Instance.AppEventData.GetCount(logEvent));
            parameters.Add("USER_LEVEL", UserData.Instance.Level.Value);
            parameters.Add("MAIN_BOARD", boardIdent == MergeBoardIdent.Main ? "YES" : "NO");
            parameters.Add("DID_PLAYER_EVER_PAY", UserData.Instance.Shop.IsPayUser ? "YES" : "NO");
            parameters.Add("CATEGORY", key);
            parameters.Add("CONNECTED_DAYS", GetConnectedDays());

            LogEvent(logEvent, parameters);
        }
    }
}