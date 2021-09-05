using System;
using System.Collections.Generic;
using System.Linq;
using I2.Loc;
using OutGameCore;
using SH.Constant;
using SH.Data;
using UniRx;

namespace SH.Game.DataLocal
{
    public class SettingsLocal
    {
        public SettingsLocal()
        {
            OptionSoundEffect = new BoolReactiveProperty(true);
            OptionBGM = new BoolReactiveProperty(true);
            OptionHaptic = new BoolReactiveProperty(true);
            SelectedLanguage = "";
            OptionNotification = new BoolReactiveProperty(true);
            OptionNotification_Chest = new BoolReactiveProperty(true);
            OptionNotification_DailyDeals = new BoolReactiveProperty(true);
            OptionNotification_Energy = new BoolReactiveProperty(true);
        }

        public BoolReactiveProperty OptionSoundEffect { get; set; }
        public BoolReactiveProperty OptionBGM { get; set; }
        public BoolReactiveProperty OptionHaptic { get; set; }
        public string SelectedLanguage { get; set; }   //선택된 언어
        public BoolReactiveProperty OptionNotification { get; set; }    //알림 설정
        public BoolReactiveProperty OptionNotification_Chest { get; set; } //상자 알림
        public BoolReactiveProperty OptionNotification_DailyDeals { get; set; }  //DailyDeals 알림
        public BoolReactiveProperty OptionNotification_Energy { get; set; }  //에너지 알림
    }
}