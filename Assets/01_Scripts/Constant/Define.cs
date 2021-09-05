/*
* Copyright (c) CookApps.
* 이진호(jhlee8@cookapps.com)
*/

namespace SH.Constant
{
    public static class Define
    {
        /// <summary>
        /// 보드 열(세로)
        /// </summary>
        public const int BoardCol = 7;
        /// <summary>
        /// 보드 행(가로)
        /// </summary>
        public const int BoardRow = 8;
        /// <summary>
        /// 메인 보드판 확장시
        /// </summary>
        public const int BoardRowExpand = 9;
        
        /// <summary>
        /// 이벤트 보드판 기본
        /// </summary>
        public const int BoardEventRow = 7;
        
        /// <summary>
        /// 플라워가든 보드판 기본
        /// </summary>
        public const int BoardFlowerGardenRow = 7;
        
        /// <summary>
        /// 이벤트 보드판 확장시
        /// </summary>
        public const int BoardEventRowExpand = 8;
        
        /// <summary>
        /// 보드 행(가로) 최대
        /// </summary>
        public const int BoardRowMax = 9;

        /// <summary>
        /// 플라워 가든 인벤 열(세로)
        /// </summary>
        public const int FlowerGardenInventoryCow = 4;
        /// <summary>
        /// 플라워 가든 인벤 행(가로)
        /// </summary>
        public const int FlowerGardenInventoryRow = 6;

        /// <summary>
        /// 플라워 가든 아이템 최대 
        /// </summary>
        public const int FlowerGardenInventoryItemMax = 10;

        public const float ItemMoveSpeed = 500;
        public const float ItemMoveDuration = 0.4f;
        public const float ItemJumpMoveDuration = 0.6f;
        public const float ItemJumpMoveDurationMin = 0.56875f;
        public const float ItemJumpMoveDurationMax = 1.25f;
        public const float ItemJumpMoveDurationSecond = 0.95f;  //0.575
        public const float ItemWidth = 140;
        public const float ItemHeight = 140;
        public const int BagDefaultAmount = 5;
        /// <summary>
        /// 처음 퀘스트 id
        /// </summary>
        public const int FirstQuestId = 1000;
        
#if UNITY_AWS
        public const string Platform = "aws";
#elif UNITY_ANDROID
        public const string Platform = "android";
#elif UNITY_IOS
        public const string Platform = "ios";
#else
        public const string Platform = "etc";
#endif
    }
}