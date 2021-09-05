/*
* Copyright (c) CookApps.
* 이진호(jhlee8@cookapps.com)
*/

using System;
using System.Collections.Generic;
using SH.Game.Manager;
using UniRx;
using UnityEngine.UI;

namespace SH.Util.UniRx
{
    public static class UniRxExtensions
    {
        public static IObservable<bool> WhereTrue(this IObservable<bool> source)
        {
            return source.Where(value => value);
        }
        
        public static IObservable<bool> WhereFalse(this IObservable<bool> source)
        {
            return source.Where(value => !value);
        }

        public static IObservable<T> WhereNotNull<T>(this IObservable<T> source)
        {
            return source.Where(value => value != null);
        }
        
        public static IObservable<Unit> OnClickSoundAsObservable(this Button button)
        {
            return button.onClick.AsObservable().Do(_ => GameSoundManager.Instance?.PlaySfx(SFX.sh_common_click));
        }

        public static IObservable<IEnumerable<T>> ObserveChange<T>(this IReadOnlyReactiveCollection<T> source, bool notifyCurrent = false)
        {
            var observable = source.ObserveAdd().AsUnitObservable()
                .Merge(source.ObserveMove().AsUnitObservable(),
                    source.ObserveReplace().AsUnitObservable(),
                    source.ObserveReset().AsUnitObservable(),
                    source.ObserveCountChanged(notifyCurrent).AsUnitObservable());

            return observable.BatchFrame().Select(_ => source);
        }

        public static IObservable<Unit> ObserveChange<TKey, TValue>(this ReactiveDictionary<TKey, TValue> source, bool notifyCurrent = false)
        {
            var observable = source.ObserveAdd().AsUnitObservable()
                .Merge(source.ObserveReplace().AsUnitObservable(),
                    source.ObserveReset().AsUnitObservable(),
                    source.ObserveCountChanged(notifyCurrent).AsUnitObservable());

            return observable.BatchFrame();
        }
        
        public static IDisposable SubscribeToSlider(this IObservable<float> source, Slider slider)
        {
            return source.SubscribeWithState(slider, (x, s) => s.value = x);
        }
    }
}