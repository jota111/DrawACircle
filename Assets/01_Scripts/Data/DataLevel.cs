/*
* Copyright (c) CookApps.
* 이진호(jhlee8@cookapps.com)
*/

using System.Linq;
using GameDataEditor;

namespace SH.Data
{
    public static class DataLevel
    {
        public static GDELevelData GetData(int level)
        {
            return GDEDataManager.Get<GDELevelData>(level);
        }

        /// <summary>
        /// 다음 레벨을 얻는다
        /// 없으면 마지막 레벨
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        public static GDELevelData GetDataNextLevel(int level)
        {
            var data = GetData(level + 1);
            if (data == null)
            {
                data = GDEDataManager.GetAllItems<GDELevelData>().LastOrDefault();
            }

            return data;
        }

        /// <summary>
        /// 경험치 기반의 레벨
        /// 없으면 마지막 레벨
        /// </summary>
        /// <param name="exp"></param>
        /// <returns></returns>
        public static GDELevelData GetDataByExp(int exp)
        {
            var data = GDEDataManager.GetAllItems<GDELevelData>().FirstOrDefault(item => item.Exp >= exp);
            if (data == null)
            {
                data = GDEDataManager.GetAllItems<GDELevelData>().LastOrDefault();
            }
            return data;
        }
    }
}