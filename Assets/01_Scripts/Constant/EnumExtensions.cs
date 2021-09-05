/*
* Copyright (c) CookApps.
* 이진호(jhlee8@cookapps.com)
*/

using System;

namespace SH.Constant
{
    public static class EnumExtensions
    {
        public static bool IsEventBoard(this MergeBoardIdent value) =>
            value switch
            {
                MergeBoardIdent.LemonTree => true,
                
                MergeBoardIdent.FlowerGarden => false,
                MergeBoardIdent.None => false,
                MergeBoardIdent.Main => false,
                MergeBoardIdent.Test => false,
                MergeBoardIdent.Editor1 => false,
                _ => throw new ArgumentOutOfRangeException(nameof(value), value, null)
            };

        public static bool IsFlowerGarden(this MergeBoardIdent value)
        {
            return value == MergeBoardIdent.FlowerGarden;
        }


        public static bool IsMainBoard(this MergeBoardIdent value)
        {
            return value == MergeBoardIdent.Main;
        }
    }
}