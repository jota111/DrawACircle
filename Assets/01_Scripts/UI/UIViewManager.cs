/*
* Copyright (c) CookApps.
* 이진호(jhlee8@cookapps.com)
*/

using System.Linq;
using Doozy.Engine.UI;
using JetBrains.Annotations;
using SH.Game.InGame.Msg;
using SH.Game.Manager;
using SH.UI.View;
using SH.Util.UniRx;
using UniRx;

namespace SH.UI
{
    public sealed class UIViewManager
    {
        public static UIViewManager Instance { get; private set; }
        private readonly ReactiveCollection<UIView> _stack = new ReactiveCollection<UIView>();

        public IReadOnlyReactiveCollection<UIView> AllStack => _stack;
        public bool IsEmpty => _stack.Count == 0;
        public int GetStackCount => _stack.Count;
        public bool Contains(UIView view) => _stack.Contains(view);
        public void Remove(UIView view) => _stack.Remove(view);

        public UIViewManager(InteractableManager interactableManager, SceneDisposable disposable)
        {
            Instance = this;
            _stack.ObserveCountChanged()
                .Select(value => value == 0)
                .DistinctUntilChanged()
                .Subscribe(interactableManager.SetInteractablePanAndZoom)
                .AddTo(disposable);
        }

        public void Push([NotNull] UIView view)
        {
            var last = _stack.LastOrDefault();
            if (last == view)
                return;

            if (!_stack.Contains(view))
            {
                _stack.Add(view);
                MessageBroker.Default.Publish(new UIViewPush(view));
            }

            if (last != null && CheckCantHide(last.ViewName)) return;
            last?.Hide(true);
        }

        public UIView GetLastOrDefault()
        {
            var last = _stack.LastOrDefault();
            return last;
        }

        public void Pop(UIView view)
        {
            var last = _stack.LastOrDefault();
            if (view == last)
            {
                _stack.Remove(view);
                last = _stack.LastOrDefault();
                if (last != null && !CheckCantHide(last.ViewName))
                    last.Show(true);
                MessageBroker.Default.Publish(new UIViewPop(view));
            }
        }

        public bool CheckCantHide(string viewName)
        {
            return viewName.Equals(UIViewName.Shop) || viewName.Equals(UIViewName.Quest) || viewName.Equals(UIViewName.InduceRating) ||
                   viewName.Equals(UIViewName.MailBox) || viewName.Equals(UIViewName.FriendsManaging) ||
                   viewName.Equals(UIViewName.Language);
        }

        public bool CheckCantInstant(string viewName)
        {
            return viewName.Equals(UIViewName.DetailedMail);
        }

        public void ClearStack()
        {
            _stack.Clear();
        }

        public void HideAll()
        {
            ClearStack();
            foreach (var visibleView in UIView.VisibleViews.ToArray())
            {
                visibleView.Hide();
            }
        }

        public static bool CheckView(string viewName)
        {
            return UIView.VisibleViews.Find(x => x.ViewName.Equals(viewName)) != null;
        }

        public bool PopLastByBackButton()
        {
            var last = _stack.LastOrDefault();
            if (last != null)
            {
                // BackButton 허용 안하면 리턴
                var view = last.gameObject.GetComponent<UIViewBase>();
                if (view != null && view.UseBackButton == false)
                    return true;

                last.Hide(_stack.Count > 1 && !CheckCantInstant(last.ViewName));
                return true;
            }

            return false;
        }
    }
}