using System.Collections;
using System.Collections.Generic;
using SH.Util;
using UniRx;
using UnityEngine;


namespace SH.Game.DataLocal
{
    public sealed class ProfileDataLocal
    {
        public ReactiveProperty<string> NickName;
        public ObscuredIntReactiveProperty nickNameChangeCount;

        public ProfileDataLocal()
        {
            NickName ??= new ReactiveProperty<string>("");
            nickNameChangeCount ??= new ObscuredIntReactiveProperty(0);
        }

        public void SetNickName(string nickName)
        {
            NickName.Value = nickName;
            AddNickNameChangeCount();
        }

        /// <summary>
        /// 닉네임 변경횟수 증가
        /// </summary>
        private void AddNickNameChangeCount() => nickNameChangeCount.Value += 1;
    }
}
