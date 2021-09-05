/*
* Copyright (c) CookApps.
* 이진호(jhlee8@cookapps.com)
*/

using System.Linq;
using SH.AppEvent;
using SH.Data;
using SH.Util;
using SH.Util.UniRx;
using UniRx;
using UnityEngine;
using Zenject;

namespace SH.Game.Manager
{
    public sealed class LevelManager
    {
        private readonly UserData.UserData _userData;
        public IReadOnlyReactiveProperty<int> Level { get; }
        public IReadOnlyReactiveProperty<int> Exp { get; }
        public IReadOnlyReactiveProperty<int> StartExp { get; }
        public IReadOnlyReactiveProperty<int> NextExp { get; }
        public IReadOnlyReactiveProperty<float> LevelUpPercent { get; }
        
        public IReadOnlyReactiveProperty<bool> PossibleLevelUp { get; }

        public IReadOnlyReactiveProperty<bool> IsReachMaxLevel { get; }

        public LevelManager(UserData.UserData userData, SceneDisposable disposable)
        {
            _userData = userData;
            Level = userData.Level;
            Exp = userData.Exp;

            LevelUpPercent = Level.CombineLatest(Exp, (level, exp) =>
            {
                var start = DataLevel.GetData(level).Exp;
                var end = DataLevel.GetDataNextLevel(level).Exp;
                var diff = end - start;
                if (diff <= 0)
                    return 1.0f;
                var baseExp = exp - start;
                var per = baseExp / (float) diff;
                per = Mathf.Clamp01(per);
                return per;
            }).ToReadOnlyReactiveProperty();

            StartExp = Level.Select(level =>
            {
                if (level <= 1) return 0;
                var data = DataLevel.GetData(level);
                return data.Exp;
            }).ToReadOnlyReactiveProperty();

            NextExp = Level.Select(level =>
            {
                var data = DataLevel.GetDataNextLevel(level);
                return data.Exp;
            }).ToReadOnlyReactiveProperty();
            
            PossibleLevelUp = Level.CombineLatest(Exp, (level, exp) =>
            {
                var start = DataLevel.GetData(level).Exp;
                var end = DataLevel.GetDataNextLevel(level).Exp;
                var diff = end - start;
                if (diff <= 0)
                    return false;
                return exp >= end;
            }).ToReadOnlyReactiveProperty();

            IsReachMaxLevel = Level.Select(level => DataLevel.GetDataNextLevel(level).Level == level)
                .ToReadOnlyReactiveProperty();
        }

        public void AddExp(int exp) => _userData.Exp.Value += exp;

        public bool ReachLevel()
        {
            if (LevelUpPercent.Value < 1.0f)
                return false;

            var level = _userData.Level.Value + 1;
            // 다음 레벨 데이타 체크
            if (DataLevel.GetData(level) == null)
                return false;

            _userData.Level.Value = level;

            // 보상
            var data = DataLevel.GetData(level);
            
            // foreach (var itemType in rewards)
            // {
            //     _rewardManager.Value.GetReward(itemType);
            // }
            
            return true;
        }

        public static int GetLevelByExp(int exp)
        {
            for (int i = 1; i < 100; i++)
            {
                var start = DataLevel.GetData(i).Exp;
                var end = DataLevel.GetDataNextLevel(i).Exp;
                if (start <= exp && end > exp)
                    return i;
            }
            return 100;
        }
    }
}