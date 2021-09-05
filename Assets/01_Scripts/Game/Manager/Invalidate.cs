/*
* Copyright (c) CookApps.
* 이진호(jhlee8@cookapps.com)
*/

using System;
using SH.Constant;
using UniRx;
using UnityEngine;

namespace SH.Game.Manager
{
    /// <summary>
    /// 무효화 갱신
    /// </summary>
    public readonly struct Invalidate
    {
        public readonly TargetType targetType;
        public readonly InvalidateType invalidateType;

        public Invalidate(TargetType type, InvalidateType inType)
        {
            targetType = type;
            invalidateType = inType;
        }

        public static void Publish(TargetType type) => MessageBroker.Default.Publish(new Invalidate(type, InvalidateType.Immediate));
        public static IObservable<Unit> Receive(TargetType type)
            => MessageBroker.Default.Receive<Invalidate>()
                .Where(x => (x.targetType == type || x.targetType == TargetType.All) && x.invalidateType == InvalidateType.Immediate)
                .AsUnitObservable().BatchFrame();
        public static void DelayedPublish(TargetType type, bool onOff) =>
            MessageBroker.Default.Publish(new Invalidate(type, (onOff ? InvalidateType.DelayedAfter : InvalidateType.DelayedBefore)));
        public static IObservable<Unit> DelayedReceive(TargetType type, bool onOff)
            => MessageBroker.Default.Receive<Invalidate>()
                .Where(x => (x.targetType == type || x.targetType == TargetType.All) &&
                            x.invalidateType == (onOff ? InvalidateType.DelayedAfter : InvalidateType.DelayedBefore)).AsUnitObservable().BatchFrame();
        
        // 특정 타겟만 반응
        public static IObservable<Unit> DelayedReceiveSpecificTarget(TargetType type, bool onOff)
            => MessageBroker.Default.Receive<Invalidate>()
                .Where(x => (x.targetType == type) &&
                            x.invalidateType == (onOff ? InvalidateType.DelayedAfter : InvalidateType.DelayedBefore)).AsUnitObservable().BatchFrame();

        public enum TargetType
        {
            Exp,
            Energy,
            Gold,
            Jewel,
            EventCoin,
            All,
        }
        
        public static TargetType GetTargetType(Sprite sprite)
        {
            if (sprite.name.Contains(Currencies.Coin.ToString()))
                return TargetType.Gold;
            else if (sprite.name.Contains(Currencies.Diamond.ToString()))
                return TargetType.Jewel;
            else if (sprite.name.Contains(Currencies.XP.ToString()))
                return TargetType.Exp;
            else if (sprite.name.Contains("Energy"))
                return TargetType.Energy;
            else
                return TargetType.All;
        }

        public enum InvalidateType
        {
            Immediate,
            DelayedBefore,
            DelayedAfter
        }
    }
}