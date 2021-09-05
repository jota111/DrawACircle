using System;
using System.Collections;
using System.Collections.Generic;
using OutGameCore;
using UnityEngine;

namespace SH.Game.DataLocal
{
    [Serializable]
    public class DailyDataLocal
    {
        public DateTime DailyTime;
        public int DailyCount;

        public int AddDailyCount()
        {
            if (GameUtils.GetFlooredDiffDays(DailyTime) >= 1)
            {
                DailyCount = 0;
                DailyTime = GameUtils.GetGameTime();
            }
            DailyCount++;
            return DailyCount;
        }
    }
}
