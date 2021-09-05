using System.Collections;
using System.Collections.Generic;
using EnhancedUI.EnhancedScroller;
using UnityEngine;

namespace Community.Default
{
    public class UI_EnhancedScroller : MonoBehaviour, IEnhancedScrollerDelegate
    {
        [SerializeField] private UIItem_EnhancedCellView cellViewPrefab;
        [SerializeField] private EnhancedScroller _scroller;
        public List<DataEnhanced> _datas = new List<DataEnhanced>();
        
        private void Awake()
        {
            _scroller.Delegate = this;
        }
        
        public void ReloadData()
        {
            _scroller.ReloadData();
        }

        public void RefreshActiveCellViews()
        {
            _scroller.RefreshActiveCellViews();
        }
        
        public int GetNumberOfCells(EnhancedScroller scroller)
        {
            return _datas.Count;
        }

        public float GetCellViewSize(EnhancedScroller scroller, int dataIndex)
        {
            return 150f;
        }

        public EnhancedScrollerCellView GetCellView(EnhancedScroller scroller, int dataIndex, int cellIndex)
        {
            UIItem_EnhancedCellView cellView = scroller.GetCellView(cellViewPrefab) as UIItem_EnhancedCellView;

            cellView.name = "UIItem_EnhancedCellView_" + dataIndex.ToString();
            cellView.SetView(this, _datas[dataIndex]);

            return cellView;
        }
    }
}
