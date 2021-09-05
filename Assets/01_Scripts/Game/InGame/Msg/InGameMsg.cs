/*
* Copyright (c) CookApps.
* 이진호(jhlee8@cookapps.com)
*/

using Doozy.Engine.UI;
using SH.Constant;
using UnityEngine;

namespace SH.Game.InGame.Msg
{
    /// <summary>
    /// 아이템 제거
    /// </summary>
    public readonly struct ItemRemove
    {
        
    }

    /// <summary>
    /// 아이템 이동됨
    /// </summary>
    public readonly struct ItemMove
    {
        
    }
    
    /// <summary>
    /// 무브 시작됨
    /// </summary>
    public readonly struct ItemBeginMove
    {
        
    }
    
    /// <summary>
    /// 무브 진행중
    /// </summary>
    public readonly struct ItemOnTheMove
    {
        
    }
    
    /// <summary>
    /// 아이템 이동 복원
    /// </summary>
    public readonly struct ItemMoveRestore
    {
        
    }
    
    /// <summary>
    /// 아이템 이동 에니메니션 완료
    /// </summary>
    public readonly struct ItemMoveTweenComplete
    {
        
    }

    /// <summary>
    /// 인게임 클리어
    /// </summary>
    public readonly struct ClearInGame
    {
    }

    /// <summary>
    /// 로비 클리어
    /// </summary>
    public readonly struct ClearLobby
    {
    }

    public readonly struct EnterScreen
    {
        public readonly ScreenState Screen;

        public EnterScreen(ScreenState screen)
        {
            Screen = screen;
        }
    }

    public readonly struct BoardChanged
    {
        public readonly MergeBoardIdent Ident;

        public BoardChanged(MergeBoardIdent ident)
        {
            Ident = ident;
        }
    }

    public readonly struct ScreenTransitionEnd
    {
        
    }

    public readonly struct QuestOpenEventEnd
    {
        public readonly int QuestId;

        public QuestOpenEventEnd(int questId)
        {
            QuestId = questId;
        }
    }
    
    public readonly struct UIViewPush
    {
        public readonly UIView UIView;

        public UIViewPush(UIView uiView)
        {
            UIView = uiView;
        }
    }
    
    public readonly struct UIViewPop
    {
        public readonly UIView UIView;

        public UIViewPop(UIView uiView)
        {
            UIView = uiView;
        }
    }

    public readonly struct TutorialStepFinish
    {
        
    }
    public readonly struct QuestMarkSpawned
    {
        
    }

    public readonly struct FlowerBoardUpdate
    {
        public readonly UpdateType Type;

        public FlowerBoardUpdate(UpdateType _Type)
        {
            Type = _Type;
        }

        public enum UpdateType
        {
            NewLevel = 0,
            Open = 1,
            Complete = 2,
        }
    }

    public readonly struct FlowerFactoryQuestReady
    {
        public readonly Transform QuestOwner;

        public FlowerFactoryQuestReady(Transform questOwner)
        {
            QuestOwner = questOwner;
        }
    }
    
    public readonly struct FlowerFactoryStartWork
    {
    }

    public readonly struct FlowerGardenInventoryPush
    {
        
    }
}