/*
* Copyright (c) CookApps.
* 이진호(jhlee8@cookapps.com)
*/

namespace SH.Constant
{
    public enum Currencies
    {
        None,
        Coin = 1,
        Diamond = 2,
        XP = 3,
        Energy = 4,
        EventCoin = 5,
    }

    public enum MergeBoardIdent
    {
        None,
        Main,
        LemonTree,
        FlowerGarden,
        Test = 101,
        Editor1 = 1001,
    }
    
    public enum ItemVisibility
    {
        Visible,
        HiddenWithVisibleInside
    }

    public class ZenId
    {
        public const string InGame = "InGame";
        public const string Board = "Board";
        public const string Facade = "Facade";
        public const string BoardEffectParent = "BoardEffectParent";
        public const string MergeItemParent = "MergeItemParent";
        public const string MergeItemMoveParent = "MergeItemMoveParent";
        public const string TestRunnerEditMode = "TestRunnerEditMode";
        public const string ButtonStack = "ButtonStack";
        public const string RectTransformLevel = "RectTransformLevel";
        public const string RectTransformGold = "RectTransformGold";
        public const string RectTransformJewel = "RectTransformJewel";
        public const string RectTransformEnergy = "RectTransformEnergy";
        public const string RectTransformMainBoardButton = "RectTransformMainBoardButton";
        public const string RectTransformEventBoardButton = "RectTransformEventBoardButton";
        public const string RectTransformFlowerBoardButton = "RectTransformFlowerBoardButton";
        public const string RectTransformLobbyButton = "RectTransformLobbyButton";
        public const string RectTransformGetEffectParent = "RectTransformGetEffectParent";
        public const string RectTransformOpenChest = "RectTransformOpenChest";
        public const string RectTransformQuestButton = "RectTransformQuestButton";
        public const string RectTransformQuestButtonInGame = "RectTransformQuestButtonInGame";
        public const string RectTransformInventoryButton = "RectTransformInventoryButton";
        public const string RectTransformQuestRewardCompleteButton = "RectTransformQuestRewardCompleteButton";
        public const string BoardEditor = "BoardEditor";
        public const string TopUI = "TopUI";
        public const string RectTransformSettingButton = "RectTransformSettingButton";
        public const string RectTransformMailboxButton = "RectTransformMailboxButton";
        public const string RectTransformEventBoardButtonInGame = "RectTransformEventBoardButtonInGame";
        public const string RectTransformEventCoinInGame = "RectTransformEventCoinInGame";
        public const string RectTransformFriendsButton = "RectTransformFriendsButton";
        public const string FriendsSlider = "FriendsSlider";
        public const string FriendsMailBox = "FriendsMailBox";
        public const string RectTransformFlowerQuestButtonInGame = "RectTransformFlowerQuestButtonInGame";
    }

    public enum AtlasType
    {
        InGame,
        Shop,
        EventCustomer,
        OutGame,
        Lobby,
    }

    public enum AreaType
    {
        NearHorizontalAndVertical,
        Near1x1,
        Near3x3
    }

    public enum ItemSpawnType
    {
        /// <summary>
        /// 기본
        /// </summary>
        Exist,
        /// <summary>
        /// 머지 생성
        /// </summary>
        Merge,
        /// <summary>
        /// 가방
        /// </summary>
        Inventory,
        /// <summary>
        /// 특정 슬롯으로 부터
        /// </summary>
        Coord,
        /// <summary>
        /// 대기열로 부터
        /// </summary>
        Stack,
        /// <summary>
        /// 버블
        /// </summary>
        Bubble,
        /// <summary>
        /// 판매 복원
        /// </summary>
        UndoSale,
        /// <summary>
        /// 일회용 생산물 소모
        /// </summary>
        Consumed,
        /// <summary>
        /// 숨겨져있다 보인
        /// </summary>
        Unhidden,
        Unlock,
        SelfLevelUp,
        /// <summary>
        /// 플라워 공장
        /// </summary>
        FlowerFactory,
    }

    public enum ChestState
    {
        NotOpen,
        Opening,
        Opened,
    }

    public enum ScreenState
    {
        Lobby,
        InGame,
    }
    
    public enum TutorialStep
    {
        Merge_1_GardenToolBox01 = 1,
        Merge_1_GardenToolBox02,
        Merge_1_GardenToolBox03,
        Merge_1_GardenGloves01,
        Merge_1_GardenGloves02,
        
        Merge_2_GardenTool01,
        Merge_2_GardenTool02,
        

        Merge_3_GardenTool01,
        Merge_3_GardenTool02,
        Merge_3_GardenTool03,

        OpenChest,
        ProductItem1,
        ProductItem2,
        ProductItem3,
        Quest1000,
        Quest1020,
        Quest1030,
        QuestClear1000,
        QuestClear1020,
        QuestClear1030,
        StackItemBlueChest01,
        LevelUp,
        UnlockShopSale,
        UnlockShopInven,
        
        FlowerGarden_FirstInGame, // 첫 인게임 진입
        FlowerGarden_FactoryFirstQuestReady, // 공장 첫 퀘스트 준비상태
        FlowerGarden_FactoryFirstWork, // 공장 첫 가동
        FlowerGarden_SpawnMaxFlower, // 최종 단계 꽃 생성
        FlowerGarden_FirstInventory, // 인벤토리 처음 사용
        
    }

    public enum PrefabType
    {
        // Effect
        Effect_MergeAble = 101,
        /// <summary>
        /// 머지 아이템 파되 효과
        /// </summary>
        Effect_DestroyMergeItem,
    }
}