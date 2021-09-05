using System.Collections;
using System.Collections.Generic;
using EnhancedUI.EnhancedScroller;
using UnityEngine;

namespace Community.Default
{
    public class UIItem_EnhancedCellView : EnhancedScrollerCellView
    {
        [SerializeField] private UI_EnhancedScroller scroller;
        private DataEnhanced data;

        public void SetView(UI_EnhancedScroller _scroller, DataEnhanced _data)
        {
            scroller = _scroller;
            data = _data;
        }
        public override void RefreshCellView()
        {
        }
    }
}
