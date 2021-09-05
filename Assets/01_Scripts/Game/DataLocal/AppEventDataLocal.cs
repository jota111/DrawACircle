using System;
using System.Collections;
using System.Collections.Generic;
using OutGameCore;
using SH.AppEvent;
using SH.Constant;
using UniRx;
using UnityEngine;

namespace SH.Game.DataLocal
{
    public sealed class AppEventDataLocal
    {
        public ReactiveDictionary<string, long> AppEventSumValues { get; set; } = new ReactiveDictionary<string, long>();

        public long GetCount(string eventKey)
        {
            if (AppEventSumValues.ContainsKey(eventKey) == false)
                AppEventSumValues.Add(eventKey, 0);
            AppEventSumValues[eventKey]++;
            return AppEventSumValues[eventKey];
        }
        
        #region 광고

        public DateTime LobbyRewardAdsTime;
        public int LobbyRewardAds_ExposeCount;
        public int LobbyRewardAds_ClickCount;

        public void LobbyRewardAds_AddExposeCount()
        {
            if (GameUtils.GetFlooredDiffDays(LobbyRewardAdsTime) >= 1)
            {
                LobbyRewardAds_ExposeCount = 0;
                LobbyRewardAds_ClickCount = 0;
                LobbyRewardAdsTime = GameUtils.GetGameTime();
            }
            LobbyRewardAds_ExposeCount++;
        }

        public void LobbyRewardAds_AddClickCount() => LobbyRewardAds_ClickCount++;

        #endregion
        
        #region 에너지

        public DateTime EnergyConsumeTime;
        public Dictionary<MergeBoardIdent, int> EnergyConsumeCount = new Dictionary<MergeBoardIdent, int>();

        public void AddEnergyConsumeCount(MergeBoardIdent board, int value)
        {
            if (GameUtils.GetFlooredDiffDays(EnergyConsumeTime) >= 1)
            {
                EnergyConsumeTime = GameUtils.GetGameTime();
                EnergyConsumeCount.Clear();
            }
            if (!EnergyConsumeCount.ContainsKey(board))
                EnergyConsumeCount.Add(board, 0);
            EnergyConsumeCount[board] += value;
        }

        #endregion
        
        #region 접속

        public DailyDataLocal ConnectCountData = new DailyDataLocal();

        #endregion
    }
}
