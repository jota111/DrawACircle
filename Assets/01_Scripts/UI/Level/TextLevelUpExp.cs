/*
* Copyright (c) CookApps.
* 이진호(jhlee8@cookapps.com)
*/

using SH.Game.Manager;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace SH.UI.Level
{
    [RequireComponent(typeof(Text))]
    public class TextLevelUpExp : MonoBehaviour
    {
        [Inject]
        private void Construct(LevelManager levelManager)
        {
            var text = GetComponent<Text>();
            levelManager.Exp.CombineLatest(levelManager.NextExp, (exp, next) => $"{exp - levelManager.StartExp.Value}/{next - levelManager.StartExp.Value}")
                .SubscribeToText(text)
                .AddTo(this);
        }
    }
}