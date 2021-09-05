using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SH.UI.EnhancedHelper
{
    public class EnhancedScrollerCellDataSet<T> where T : class
    {
        private readonly List<EnhancedScrollerCellRow<T>> _datas = new List<EnhancedScrollerCellRow<T>>();
        private readonly int _rowCount;
    
        public EnhancedScrollerCellDataSet(int rowCount)
        {
            _rowCount = rowCount;
        }
    
        public EnhancedScrollerCellRow<T> this[int index] => _datas[index];
        public int Count=> _datas.Count;
        public void Clear() => _datas.Clear();

        /// <summary>
        /// 마지막 row를 가져오거나, 마지막 row가 가득 찼으면 새 row 생성해서 반환
        /// </summary>
        public EnhancedScrollerCellRow<T> GetLastOrAdd()
        {
            for (int i = _datas.Count - 1; i >= 0; i--)
            {
                if (_datas[i].Count!= _rowCount)
                    return _datas[i];
            }

            EnhancedScrollerCellRow<T> data = new EnhancedScrollerCellRow<T>();
            _datas.Add(data);
            return data;
        }

        public void Add()
        {
            EnhancedScrollerCellRow<T> data = new EnhancedScrollerCellRow<T>();
            _datas.Add(data);
        }

        public void SetLastOrAdd(T data, float dataSize = 1f)
        {
            EnhancedScrollerCellRow<T> row = GetLastOrAdd();

            if (row.CurrentSize + dataSize > _rowCount && row.Count < _rowCount)
            {
                int count = row.Count;
                for (int j = 0; j < _rowCount - count; j++)
                {
                    row.Add(null);
                }

                row = GetLastOrAdd();
            }

            row.Add(data, dataSize);
            // if (dataSize > 1 && row.Count < 3)
            //     row.Add(null);
            // if (dataSize > 2 && row.Count < 3)
            //     row.Add(null);
        }
    }
}
