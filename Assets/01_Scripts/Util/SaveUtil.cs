/*
* Copyright (c) CookApps.
* 이진호(jhlee8@cookapps.com)
*/

using System;
using OutGameCore;

namespace SH.Util
{
    public static class SaveUtil
    {
        public static bool Save<T>(string key, T value)
        {
            try
            {
                ES3.Save(key, value);
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                var msg = GameUtils.I2Format("Text_NotEnoughStorage");
                ToastHelper.Show(msg);
                return false;
            }
        }
    }
}