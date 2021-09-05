/*
* Copyright (c) CookApps.
* 이진호(jhlee8@cookapps.com)
*/

using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using SH.Constant;

namespace SH.Game.DataLocal
{
    public class QuestPassRateDataLocal
    {
        [JsonProperty(PropertyName = "1")]
        private Dictionary<MergeBoardIdent, int> CategoryRepeatCount { get; set; } = new Dictionary<MergeBoardIdent, int>();
        
        [JsonProperty(PropertyName = "2")]
        private List<Data> Datas { get; set; } = new List<Data>();
        

        public void IncCategoryCount(MergeBoardIdent ident)
        {
            if (ident == MergeBoardIdent.Main)
                return;

            CategoryRepeatCount.TryGetValue(ident, out var count);
            count++;
            CategoryRepeatCount[ident] = count;

            // 해당 퀘스트 카테고리 제거
            Datas.RemoveAll(value => value.Category == ident);
        }

        private int GetCategoryCount(MergeBoardIdent ident)
        {
            if (ident == MergeBoardIdent.Main)
                return 1;
            
            CategoryRepeatCount.TryGetValue(ident, out var count);
            return count;
        }

        public void IncConnectCount()
        {
            foreach (var data in Datas.Where(value => value.Status == Status.Start))
            {
                data.ConnectCount++;
            }
        }

        public (int CategoryCount, Data Data) SetStatus(MergeBoardIdent ident, int qid, Status status)
        {
            var key = GetKey(ident, qid);
            var data = Datas.FirstOrDefault(value => value.Category == ident && value.Id == qid);
            if (data == null)
            {
                data = new Data {Id = qid, Category = ident, ConnectCount = 1};
                Datas.Add(data);
            }

            data.Status = status;
            if (status == Status.Start)
                data.RepeatCount++;

            var categoryCount = GetCategoryCount(ident);
            return (categoryCount, data);
        }

        private static long GetKey(MergeBoardIdent ident, int qid)
        {
            long hi = (long)ident;
            long low = qid;
            var key = (hi << 32 | low);
            return key;
        }
        
        //-----------------------------------------------------------------------------------------------------------
        
        public enum Status { Start = 1, Clear = 2 }

        public class Data
        {
            [JsonProperty(PropertyName = "1")] public int Id;
            [JsonProperty(PropertyName = "2")] public Status Status;
            [JsonProperty(PropertyName = "3")] public MergeBoardIdent Category;
            [JsonProperty(PropertyName = "4")] public int RepeatCount; // 동일퀘스트 반복 횟스
            [JsonProperty(PropertyName = "5")] public int ConnectCount; // 퀘스트 클리어시까지 접속 횟수
        }
    }
}