/*
* Copyright (c) CookApps.
* 이진호(jhlee8@cookapps.com)
*/

using System;
using DG.Tweening;

namespace SH.UI
{
    public class TextTransition : UnityEngine.UI.Text
    {
        public override string text
        {
            get => base.text;
            set => SetText(value);
        }

        private void SetText(string value)
        {
            if (string.IsNullOrEmpty(value))
                value = string.Empty;
            
            if (text.Equals(value))
                return;

            this.DOKill();
            SetTextValue(value);
            var c = color;
            c.a = 0;
            color = c;
            this.DOFade(1, 0.2f).SetRecyclable(true);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            this.DOKill();
        }

        private void SetTextValue(string value)
        {
            if (String.IsNullOrEmpty(value))
            {
                if (String.IsNullOrEmpty(m_Text))
                    return;
                m_Text = "";
                SetVerticesDirty();
            }
            else if (m_Text != value)
            {
                m_Text = value;
                SetVerticesDirty();
                SetLayoutDirty();
            }
        }
    }
}