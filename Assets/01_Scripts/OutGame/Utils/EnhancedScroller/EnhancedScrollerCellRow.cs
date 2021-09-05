using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SH.UI.EnhancedHelper
{
    public class EnhancedScrollerCellRow<T>
    {
        private readonly List<T> _data = new List<T>();

        public T this[int index]
        {
            get => _data[index];
            set => _data[index] = value;
        }
        public int Count=> _data.Count;

        public void Add(T data, float size = 1f)
        {
            _data.Add(data);
            CurrentSize += size;
        }

        public float CurrentSize = 0f;
    }
}
