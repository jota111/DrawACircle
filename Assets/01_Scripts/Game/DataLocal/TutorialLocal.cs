/*
* Copyright (c) CookApps.
* 이진호(jhlee8@cookapps.com)
*/

using System.Collections.Generic;
using System.Runtime.Serialization;
using JetBrains.Annotations;
using SH.Constant;
using UniRx;

namespace SH.Game.DataLocal
{
    public class TutorialLocal
    {
        /// <summary>
        /// 튜토리얼 스탭
        /// </summary>
        public Dictionary<TutorialStep, BoolReactiveProperty> TutorialStep { get; set; }


        [OnDeserialized]
        internal void OnDeserialized(StreamingContext context)
        {
            TutorialStep ??= new Dictionary<TutorialStep, BoolReactiveProperty>();
        }

        [NotNull]
        public BoolReactiveProperty GetStep(TutorialStep step)
        {
            if (TutorialStep.TryGetValue(step, out var result)) return result;
            result = new BoolReactiveProperty(false);            
            TutorialStep.Add(step, result);
            return result;
        }
    }
}