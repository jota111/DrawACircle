/*
* Copyright (c) CookApps.
* 이진호(jhlee8@cookapps.com)
*/

using System;
using UniRx;
using UnityEngine;

namespace SH.Util
{
    public static class TimeUtil
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void DomainCleanup()
        {
            _intervalDay = null;
            _intervalRemainTime = null;
        }

        public static DateTime Now => DateTime.Now;

        /// <summary>
        /// 요일이 바뀔때
        /// </summary>
        private static IObservable<Unit> _intervalDay;
            

        private static IObservable<Unit> intervalDay()
        {
            _intervalDay ??= Observable.Interval(TimeSpan.FromSeconds(1)).Select(_ => Now.DayOfWeek)
                .StartWith(Now.DayOfWeek).DistinctUntilChanged()
                .Skip(1).AsUnitObservable()
                .Share();
            return _intervalDay;
        }

        /// <summary>
        /// 남은시간 표시용(1초마다)
        /// </summary>
        private static IObservable<Unit> _intervalRemainTime;

        private static IObservable<Unit> intervalRemainTime()
        {
            _intervalRemainTime ??= Observable.Interval(TimeSpan.FromSeconds(1))
                .StartWith(-1)
                .AsUnitObservable()
                .Share();
            return _intervalRemainTime;
        }

        /// <summary>
        /// 요일이 바뀔때
        /// </summary>
        /// <returns></returns>
        public static IObservable<Unit> IntervalDay()
        {
            return intervalDay();
        }

        /// <summary>
        /// 자정 시간
        /// </summary>
        /// <param name="addDays"></param>
        /// <returns></returns>
        public static DateTime MidnightFromToday(int addDays = 0)
        {
            var now = DateTime.Now;
            return Midnight(now, addDays);
        }

        public static DateTime Midnight(DateTime date, int addDays = 0)
        {
            return new DateTime(date.Year, date.Month, date.Day, 0, 0, 0).AddDays(addDays);
        }

        /// <summary>
        /// 남은시간 표시(1초마다 반복)
        /// </summary>
        /// <returns></returns>
        public static IObservable<Unit> IntervalRemainTime()
        {
            return intervalRemainTime();
        }
        
        /// <summary>
        /// 카운트 다운
        /// </summary>
        /// <param name="CountTime"></param>
        /// <param name="ignoreTimeScale"></param>
        /// <returns></returns>
        public static IObservable<int> CountDown(int CountTime, bool ignoreTimeScale = false)
        {
            return Observable
                .Timer(TimeSpan.FromSeconds(0), TimeSpan.FromSeconds(1), ignoreTimeScale ? Scheduler.MainThreadIgnoreTimeScale : Scheduler.DefaultSchedulers.TimeBasedOperations)
                .Select(x => (int) (CountTime - x))
                .TakeWhile(x => x > 0);
        }
        
        public static string ToTimeString(this TimeSpan time)
        {
            if (time.TotalSeconds <= 0)
                return "00s";

            if (time.TotalDays >= 1)
                return string.Format(@"{0:D2}h {1:mm}m {1:ss}s", (int)time.TotalHours, time);

            if (time.TotalHours >= 1)
            {
                if(time.Seconds == 0 && time.Minutes == 0)
                    return time.ToString(@"hh'h'");
                else if(time.Seconds == 0)
                    return time.ToString(@"hh'h 'mm'm'");
                else
                    return time.ToString(@"hh'h 'mm'm 'ss's'");
            }

            if (time.TotalMinutes >= 1)
            {
                if(time.Seconds == 0)
                    return time.ToString(@"mm'm'");
                else
                    return time.ToString(@"mm'm 'ss's'");
            }

            return time.ToString(@"ss's'");
        }
    }
}