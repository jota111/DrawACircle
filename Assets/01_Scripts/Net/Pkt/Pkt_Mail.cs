using System.Collections.Generic;

namespace SH.Net.Pkt
{
    /// <summary>
    /// 메일 받기
    /// </summary>
    public class ReqTryGetMail : IRequest
    {
        public int umid; // 메일고유ID
        public ReqTryGetMail(int _umid) : base("/message/reward")
        {
            umid = _umid;
        }
    }

    public class RecvTryGetMail : Recv
    {
        public Data data { get; private set; }
        public class Data
        {
            public List<MsgRewardInfo> rewardInfo = new List<MsgRewardInfo>();   //아이템
        }
    }

    public class MsgRewardInfo
    {
        public string id;  //아이템
        public int cnt;   //수량
    }
}