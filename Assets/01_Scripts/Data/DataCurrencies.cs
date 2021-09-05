using System.Collections;
using System.Collections.Generic;
using OutGameCore;
using SH.Constant;
using SH.Setting;
using UnityEngine;

namespace SH.Data
{
    public static class DataCurrencies
    {
        #region 기타

        public static string GetLocalize(Currencies currencies)
        {
            return currencies switch
            {
                Currencies.Coin => GameUtils.I2Format("CurrencyName_Gold"),
                Currencies.Diamond => GameUtils.I2Format("CurrencyName_Jewel"),
                Currencies.XP => GameUtils.I2Format("CurrencyName_XP"),
                Currencies.Energy => GameUtils.I2Format("CurrencyName_Energy"),
                Currencies.EventCoin => GameUtils.I2Format("CurrencyName_EventCoin"),
                _ => null
            };
        }

        public static Sprite GetCurrencyIcon(Currencies currencies, int count)
        {
            return currencies switch
            {
                Currencies.Coin => GetCoinShopIcon(count),
                Currencies.Diamond => GetDiamondShopIcon(count),
                Currencies.XP => GetXPIcon(count),
                Currencies.Energy => GetEnergyIcon(count),
                Currencies.EventCoin => GetEventCoinShopIcon(count),
                _ => null
            };
        }

        public static Sprite GetCoinShopIcon(int count)
        {
            if (count > 10625)
                return AtlasSetting.Instance.GetShopIcon("Coins_Tier6");
            else if (count <= 10625 && count > 7820)
                return AtlasSetting.Instance.GetShopIcon("Coins_Tier5");
            else if (count <= 7820 && count > 5340)
                return AtlasSetting.Instance.GetShopIcon("Coins_Tier4");
            else if (count <= 5340 && count > 3250)
                return AtlasSetting.Instance.GetShopIcon("Coins_Tier3");
            else if (count <= 3250 && count > 1590)
                return AtlasSetting.Instance.GetShopIcon("Coins_Tier2");
            else
                return AtlasSetting.Instance.GetShopIcon("Coins_Tier1");
        }

        public static Sprite GetEventCoinShopIcon(int count)
        {
            return AtlasSetting.Instance.GetShopIcon("EventCoin_tier1");
        }

        public static Sprite GetDiamondShopIcon(int count)
        {
            if (count > 3100)
                return AtlasSetting.Instance.GetShopIcon("Diamonds_tier6");
            else if (count <= 3100 && count > 1660)
                return AtlasSetting.Instance.GetShopIcon("Diamonds_tier5");
            else if (count <= 1660 && count > 800)
                return AtlasSetting.Instance.GetShopIcon("Diamonds_tier4");
            else if (count <= 800 && count > 330)
                return AtlasSetting.Instance.GetShopIcon("Diamonds_tier3");
            else if (count <= 330 && count > 125)
                return AtlasSetting.Instance.GetShopIcon("Diamonds_tier2");
            else
                return AtlasSetting.Instance.GetShopIcon("Diamonds_tier1");
        }

        public static Sprite GetEnergyIcon(int count)
        {
            if (count > 3100)
                return AtlasSetting.Instance.GetShopIcon("Diamonds_tier6");
            else if (count <= 3100 && count > 1660)
                return AtlasSetting.Instance.GetShopIcon("Diamonds_tier5");
            else if (count <= 1660 && count > 800)
                return AtlasSetting.Instance.GetShopIcon("Diamonds_tier4");
            else if (count <= 800 && count > 330)
                return AtlasSetting.Instance.GetShopIcon("Diamonds_tier3");
            else if (count <= 330 && count > 125)
                return AtlasSetting.Instance.GetShopIcon("Diamonds_tier2");
            else
                return AtlasSetting.Instance.GetShopIcon("Diamonds_tier1");
        }
        
        public static Sprite GetXPIcon(int count)
        {
            if (count > 3100)
                return AtlasSetting.Instance.GetShopIcon("Diamonds_tier6");
            else if (count <= 3100 && count > 1660)
                return AtlasSetting.Instance.GetShopIcon("Diamonds_tier5");
            else if (count <= 1660 && count > 800)
                return AtlasSetting.Instance.GetShopIcon("Diamonds_tier4");
            else if (count <= 800 && count > 330)
                return AtlasSetting.Instance.GetShopIcon("Diamonds_tier3");
            else if (count <= 330 && count > 125)
                return AtlasSetting.Instance.GetShopIcon("Diamonds_tier2");
            else
                return AtlasSetting.Instance.GetShopIcon("Diamonds_tier1");
        }
        
        public static bool CheckCurrency(string key)
        {
            return (key.Equals(Currencies.Diamond.ToString())) || (key.Equals(Currencies.Coin.ToString())) ||
                   (key.Equals(Currencies.Energy.ToString()));
        }

        #endregion
    }
}
