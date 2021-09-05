/*
* Copyright (c) CookApps.
* 이진호(jhlee8@cookapps.com)
*/

using System;
using System.Threading;
using CodeStage.AntiCheat.ObscuredTypes;
using UniRx;

namespace SH.Util
{
    internal interface IObserverLinkedList<T>
    {
        void UnsubscribeNode(ObserverNode<T> node);
    }
    
    internal sealed class ObserverNode<T> : IObserver<T>, IDisposable
    {
        readonly IObserver<T> observer;
        IObserverLinkedList<T> list;

        public ObserverNode<T> Previous { get; internal set; }
        public ObserverNode<T> Next { get; internal set; }

        public ObserverNode(IObserverLinkedList<T> list, IObserver<T> observer)
        {
            this.list = list;
            this.observer = observer;
        }

        public void OnNext(T value)
        {
            observer.OnNext(value);
        }

        public void OnError(Exception error)
        {
            observer.OnError(error);
        }

        public void OnCompleted()
        {
            observer.OnCompleted();
        }

        public void Dispose()
        {
            var sourceList = Interlocked.Exchange(ref list, null);
            if (sourceList != null)
            {
                sourceList.UnsubscribeNode(this);
                sourceList = null;
            }
        }
    }
    
    /// <summary>
    /// 암호화 int ReactiveProperty
    /// </summary>
    [Serializable]
    public sealed class ObscuredIntReactiveProperty : IReactiveProperty<int>, IObserverLinkedList<int>, IDisposable
    {
        private ObscuredInt _value;
        
        [NonSerialized]
        ObserverNode<int> root;

        [NonSerialized]
        ObserverNode<int> last;

        [NonSerialized]
        bool isDisposed = false;
        
        public ObscuredIntReactiveProperty()
            : this(default(int))
        {
        }

        public ObscuredIntReactiveProperty(int initialValue)
        {
            SetValue(initialValue);
        }
        
        public IDisposable Subscribe(IObserver<int> observer)
        {
            if (isDisposed)
            {
                observer.OnCompleted();
                return Disposable.Empty;
            }
            
            observer.OnNext(Value);
            var next = new ObserverNode<int>(this, observer);
            if (root == null)
            {
                root = last = next;
            }
            else
            {
                last.Next = next;
                next.Previous = last;
                last = next;
            }
            return next;
        }

        public int Value
        {
            get => _value;

            set
            {
                if (_value != value)
                {
                    SetValue(value);
                    if (isDisposed)
                        return;

                    RaiseOnNext(ref value);
                }
            }
        }

        public bool HasValue => true;

        private void SetValue(int value)
        {
            this._value = value;
        }

        public void SetValueAndForceNotify(int value)
        {
            SetValue(value);
            if (isDisposed)
                return;

            RaiseOnNext(ref value);
        }
        
        void RaiseOnNext(ref int value)
        {
            var node = root;
            while (node != null)
            {
                node.OnNext(value);
                node = node.Next;
            }
        }

        void IObserverLinkedList<int>.UnsubscribeNode(ObserverNode<int> node)
        {
            if (node == root)
            {
                root = node.Next;
            }
            if (node == last)
            {
                last = node.Previous;
            }

            if (node.Previous != null)
            {
                node.Previous.Next = node.Next;
            }
            if (node.Next != null)
            {
                node.Next.Previous = node.Previous;
            }
        }
        
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (isDisposed) return;

            var node = root;
            root = last = null;
            isDisposed = true;

            while (node != null)
            {
                node.OnCompleted();
                node = node.Next;
            }
        }
        
        public override string ToString()
        {
            return _value.ToString();
        }
    }
    
    //----------------------------------------------------------------------------------------------------
    
    /// <summary>
    /// 암호화 float ReactiveProperty
    /// </summary>
    [Serializable]
    public sealed class ObscuredFloatReactiveProperty : IReactiveProperty<float>, IObserverLinkedList<float>, IDisposable
    {
        private ObscuredFloat _value;
        
        [NonSerialized]
        ObserverNode<float> root;

        [NonSerialized]
        ObserverNode<float> last;

        [NonSerialized]
        bool isDisposed = false;
        
        public ObscuredFloatReactiveProperty()
            : this(default(int))
        {
        }

        public ObscuredFloatReactiveProperty(float initialValue)
        {
            SetValue(initialValue);
        }
        
        public IDisposable Subscribe(IObserver<float> observer)
        {
            if (isDisposed)
            {
                observer.OnCompleted();
                return Disposable.Empty;
            }
            
            observer.OnNext(Value);
            var next = new ObserverNode<float>(this, observer);
            if (root == null)
            {
                root = last = next;
            }
            else
            {
                last.Next = next;
                next.Previous = last;
                last = next;
            }
            return next;
        }

        public float Value
        {
            get => _value;

            set
            {
                if (_value != value)
                {
                    SetValue(value);
                    if (isDisposed)
                        return;

                    RaiseOnNext(ref value);
                }
            }
        }

        public bool HasValue => true;

        private void SetValue(float value)
        {
            this._value = value;
        }

        public void SetValueAndForceNotify(float value)
        {
            SetValue(value);
            if (isDisposed)
                return;

            RaiseOnNext(ref value);
        }
        
        void RaiseOnNext(ref float value)
        {
            var node = root;
            while (node != null)
            {
                node.OnNext(value);
                node = node.Next;
            }
        }

        void IObserverLinkedList<float>.UnsubscribeNode(ObserverNode<float> node)
        {
            if (node == root)
            {
                root = node.Next;
            }
            if (node == last)
            {
                last = node.Previous;
            }

            if (node.Previous != null)
            {
                node.Previous.Next = node.Next;
            }
            if (node.Next != null)
            {
                node.Next.Previous = node.Previous;
            }
        }
        
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (isDisposed) return;

            var node = root;
            root = last = null;
            isDisposed = true;

            while (node != null)
            {
                node.OnCompleted();
                node = node.Next;
            }
        }
        
        public override string ToString()
        {
            return _value.ToString();
        }
    }
}