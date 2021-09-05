using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace SH.Net.Pkt
{
    /// <summary>
    /// 유저 프로필 설정 시도
    /// </summary>
    public class ReqTrySetProfile : IRequest
    {
        public string user_name; // 유저가 입력한 이름
        public int user_pic; //유저가 선택한 이미지
        public int user_age; // 유저 인풋 나이
        public int user_gender; // default: 0, male: 1, female: 2

        public ReqTrySetProfile(string name) : base("/user/valid_profile")
        {
            user_name = name;
        }
    }

    public class RecvTrySetProfile : Recv
    {
        public ProfileInfo data; //프로필정보

        public class ProfileInfo
        {
            public string input_word; // 유저가 입력한 원래 이름
            public string clean_word; // 필터링되어 저장된 이름 (이걸 쓰면 됨)
        }
    }
    
    /// <summary>
    /// 유저 프로필 설정
    /// </summary>
    public class ReqSetProfile : IRequest
    {
        public string user_name; // 유저가 입력한 이름
        public int user_pic; //유저가 선택한 이미지
        public int user_age; // 유저 인풋 나이
        public int user_gender; // default: 0, male: 1, female: 2

        public ReqSetProfile(string name) : base("/user/set_profile")
        {
            user_name = name;
        }
    }

    public class RecvSetProfile : Recv
    {
        public ProfileInfo data; //프로필정보

        public class ProfileInfo
        {
            public string input_word; // 유저가 입력한 원래 이름
            public string clean_word; // 필터링되어 저장된 이름 (이걸 쓰면 됨)
            public int input_pic; // 유저가 선택한 이미지 넘버
            public int input_age; // ""
            public int input_gender; // ""
        }
    }
}