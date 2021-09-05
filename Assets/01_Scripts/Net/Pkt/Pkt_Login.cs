/*
* Copyright (c) CookApps.
* 이진호(jhlee8@cookapps.com)
*/

// ReSharper disable UnusedAutoPropertyAccessor.Local
// ReSharper disable UnassignedGetOnlyAutoProperty

using System;
using System.Collections.Generic;
using OutGameCore;
using SH.Game.UserData;
using SH.Platform;

namespace SH.Net.Pkt
{
    public class ReqLogin : IRequest
    {
        public ReqLogin(PlatformLoginId platformLoginId = null) : base("auth/login", platformLoginId)
        {
        }
    }

    public class RecvLogin : Recv
    {
        public class MyInfo
        {
            public long ca_uid { get; private set; }
        }

        public class SaveData
        {
            public long ca_uid { get; private set; }
            public string json_data { get; private set; }
        }

        public class Data
        {
            public MyInfo myInfo { get; private set; }
            public string saveData { get; private set; }
            public List<MessageInfo> messageInfo { get; private set; }
        }

        public class MessageInfo
        {
            public int umid { get; protected set; } // Primary key. 메시지 삭제, 보상 받기 시 umid 값을 서버에 보내야 함.
            public int sender { get; protected set; } // 보낸 사람. 1: pm, 2: gm...아직 의미 없음.
            public int expire_date { get; protected set; } // 만료까지 남은 시간(초)
            public int message_type { get; protected set; } // 0: 일반, 1: 공지, 2: 보상
            public int isRead { get; protected set; } // 읽음 여부 1: true, 0: false
            public int mid { get; protected set; } // 메시지 index. 서버용도.
            public string subject { get; protected set; } // 제목
            public string message { get; protected set; } // 내용
            public List<MsgRewardInfo> rewardInfo { get; protected set; } //보상 정보
        }

        public Data data { get; private set; }

        #region 더미데이터

        public class DummyMessageInfo : MessageInfo
        {
            public void SetDummyData()
            {
                message_type = 2;
                umid = -1;
                sender = 1;
                expire_date = 604800;
                isRead = 0;
                mid = 1;
                subject = "일반 데이터"; // 제목
                message = "일반 보상 우편"; // 내용
                rewardInfo = new List<MsgRewardInfo>()
                {
                    // 보상 정보. 두개 이상일 수 있음
                    new MsgRewardInfo()
                    {
                        id = "1001", // 보상 아이디 1000: 코인, 1001: 캐시
                        cnt = 100, // 보상 갯수
                    },
                };
            }

            public void SetDummyData2()
            {
                message_type = 1;
                umid = -2;
                sender = 1;
                expire_date = 604800;
                isRead = 0;
                mid = 1;
                subject = "보상 없는 공지"; // 제목
                message = "<p>마이크 테스트</p>\n\n<p>코인 1000</p>\n\n<p>캐시 100</p>\n"; // 내용
                // rewardInfo = new List<MsgRewardInfo>()
                // {
                //     // 보상 정보. 두개 이상일 수 있음
                //     new MsgRewardInfo()
                //     {
                //         id = 1001, // 보상 아이디 1000: 코인, 1001: 캐시
                //         cnt = 100, // 보상 갯수
                //     },
                //     new MsgRewardInfo()
                //     {
                //         id = 1000, // 보상 아이디 1000: 코인, 1001: 캐시
                //         cnt = 10000, // 보상 갯수
                //     },
                //     new MsgRewardInfo()
                //     {
                //         id = 1001, // 보상 아이디 1000: 코인, 1001: 캐시
                //         cnt = 200, // 보상 갯수
                //     },
                // };
            }

            public void SetDummyData3()
            {
                message_type = 1;
                umid = -3;
                sender = 1;
                expire_date = 604800;
                isRead = 0;
                mid = 1;
                subject = "보상 있는 공지"; // 제목
                message = "<p>마이크 테스트</p>\n\n<p>코인 1000</p>\n\n<p>캐시 100</p>\n"; // 내용
                rewardInfo = new List<MsgRewardInfo>()
                {
                    // 보상 정보. 두개 이상일 수 있음
                    new MsgRewardInfo()
                    {
                        id = "1001", // 보상 아이디 1000: 코인, 1001: 캐시
                        cnt = 100, // 보상 갯수
                    },
                    new MsgRewardInfo()
                    {
                        id = "1000", // 보상 아이디 1000: 코인, 1001: 캐시
                        cnt = 10000, // 보상 갯수
                    },
                    new MsgRewardInfo()
                    {
                        id = "1001", // 보상 아이디 1000: 코인, 1001: 캐시
                        cnt = 200, // 보상 갯수
                    },
                };
            }

            public void SetDummyData4()
            {
                message_type = 2;
                umid = -5;
                sender = 1;
                expire_date = 604800;
                isRead = 0;
                mid = 1;
                subject = "일반 데이터"; // 제목
                message = "일반 보상 우편"; // 내용
                rewardInfo = new List<MsgRewardInfo>()
                {
                    // 보상 정보. 두개 이상일 수 있음
                    new MsgRewardInfo()
                    {
                        id = "BlueChest_02", // 보상 아이디 1000: 코인, 1001: 캐시
                        cnt = 1, // 보상 갯수
                    },
                };
            }

            public void SetUpdateData()
            {
                message_type = 1;
                umid = -4;
                sender = 1;
                expire_date = 0;
                isRead = 0;
                mid = 1;
                subject = GameUtils.I2Format("MailBox_Firstmail_title"); // 제목
                message = GameUtils.I2Format("MailBox_Firstmail_desc"); // 내용
                // rewardInfo = new List<MsgRewardInfo>()
                // {
                //     // 보상 정보. 두개 이상일 수 있음
                //     new MsgRewardInfo()
                //     {
                //         id = 1001, // 보상 아이디 1000: 코인, 1001: 캐시
                //         cnt = 100, // 보상 갯수
                //     },
                //     new MsgRewardInfo()
                //     {
                //         id = 1000, // 보상 아이디 1000: 코인, 1001: 캐시
                //         cnt = 10000, // 보상 갯수
                //     },
                //     new MsgRewardInfo()
                //     {
                //         id = 1001, // 보상 아이디 1000: 코인, 1001: 캐시
                //         cnt = 200, // 보상 갯수
                //     },
                // };
            }
        }

        #endregion
    }
    
    public class ReqPurchaseHistory : IRequest
    {
        public ReqPurchaseHistory() : base($"user/purchase_list")///{UserIdManager.Instance.Id}")
        {
            
        }
    }

    public class RecvPurchaseHistory : Recv
    {
        public List<PurchaseHistoryData> data { get; private set; }
        
        public class PurchaseHistoryData
        {
            public string product_id { get; private set; }
            public DateTime wdate { get; private set; }
        }
    }
}