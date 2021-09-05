using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
using UniRx;

namespace SH.Game.Manager
{
    /// <summary>
    /// 무효화 갱신
    /// </summary>
    public readonly struct NotifyLack
    {
        public readonly string lackType;

        public NotifyLack(string type)
        {
            lackType = type;
        }

        public static void Publish(string type) => MessageBroker.Default.Publish(new NotifyLack(type));
        public static IObservable<Unit> Receive(string type) => MessageBroker.Default.Receive<NotifyLack>()
            .Where(x=>x.lackType.Equals(type)).AsUnitObservable().BatchFrame();
    }
}