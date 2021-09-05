using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OutGameCore.Constant
{
    /// <summary>
    /// 배치하는 아이템 타입
    /// </summary>
    public enum PlaceableItemType
    {
        Grass = 1, //풀
        Tree = 2,   //나무
        Stone = 3,   //돌
        Rock = 4,   //바위
        Water = 5,  //물
        Tile = 6,   //타일
        Light = 7, //반짝임
        Machine = 8,    //기계
        Dust = 9,   //먼지
        Flower = 10,    //꽃
        Leaves = 11,    //낙엽
        Branches = 12,  //나뭇가지
        Bushes = 13,    //부쉬
        FallenTrees = 14,   //쓰러진 나무
        Soil = 15,   //흙
    }

    /// <summary>
    /// Appear / Disappear 타입 따로 추가할것
    /// </summary>
    public enum PlaceableSetterType
    {
        //더미
        UpToDown = 1, //위에서 오는것
        Scale = 2, //제자리에서
        ScaleUp = 3, //위로 자람
        DownToUp = 4,//아래에서 위
        Animation = 5,  //애니메이션으로 동작
        CutOffFromUp = 6,   //서서히 위쪽에서 등장
        CutOffFromDown = 7,  //서서히 아래쪽에서 등장
        CutOffFromLeft = 8,  //서서히 왼쪽에서 등장
        CutOffFromRight = 9, //서서히 오른쪽에서 등장
        Effect = 10,    //이펙트
        Fade = 11,  //Fade
        Immediate = 12, //즉시
        ScaleYoyo = 13,  //1 -> 1.2 -> 1 Scale
    }

    public enum CharacterMoveState
    {
        Idle = 0,
        Moving = 1,
    }
    
    public enum CharacterAction
    {
        Idle = 0,
        Speech = 1,
        Angry = 2,
    }

    public enum DialogueType
    {
        Normal = 0, //일반
        Selectable = 1, //선택형
    }

    public enum DialogueExpressionType
    {
        IDLE = 0,   //일반
        POSITIVE = 1,   //긍정적
        NEGATIVE = 2,   //부정적
    }

    public enum DialogueCharPosType
    {
        Left = 0,   //왼쪽
        Center = 1,   //중앙
        Right = 2,   //오른쪽
    }
}
