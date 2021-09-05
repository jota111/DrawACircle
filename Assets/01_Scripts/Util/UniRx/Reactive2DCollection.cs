/*
* Copyright (c) CookApps.
* 이진호(jhlee8@cookapps.com)
*/

using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UniRx;

namespace SH.Util.UniRx
{
    public readonly struct Collection2DMoveEvent<T> : IEquatable<Collection2DMoveEvent<T>>
    {
        public int OldX { get; }
        public int OldY { get; }
        public int NewX { get; }
        public int NewY { get; }
        public T Value { get; }

        public Collection2DMoveEvent(int OldX, int OldY, int NewX, int NewY, T value)
            : this()
        {
            this.OldX = OldX;
            this.OldY = OldY;
            this.NewX = NewX;
            this.NewY = NewY;
            Value = value;
        }

        public bool Equals(Collection2DMoveEvent<T> other)
        {
            return OldX == other.OldX && OldY == other.OldY && NewX == other.NewX && NewY == other.NewY &&
                   EqualityComparer<T>.Default.Equals(Value, other.Value);
        }

        public override bool Equals(object obj)
        {
            return obj is Collection2DMoveEvent<T> other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = OldX;
                hashCode = (hashCode * 397) ^ OldY;
                hashCode = (hashCode * 397) ^ NewX;
                hashCode = (hashCode * 397) ^ NewY;
                hashCode = (hashCode * 397) ^ EqualityComparer<T>.Default.GetHashCode(Value);
                return hashCode;
            }
        }
    }

    public readonly struct Collection2DReplaceEvent<T> : IEquatable<Collection2DReplaceEvent<T>>
    {
        public int X { get; }
        public int Y { get; }
        public T OldValue { get; }
        public T NewValue { get; }

        public Collection2DReplaceEvent(int x, int y, T oldValue, T newValue)
            : this()
        {
            X = x;
            Y = y;
            OldValue = oldValue;
            NewValue = newValue;
        }

        public bool Equals(Collection2DReplaceEvent<T> other)
        {
            return X == other.X && Y == other.Y && EqualityComparer<T>.Default.Equals(OldValue, other.OldValue) && EqualityComparer<T>.Default.Equals(NewValue, other.NewValue);
        }

        public override bool Equals(object obj)
        {
            return obj is Collection2DReplaceEvent<T> other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = X;
                hashCode = (hashCode * 397) ^ Y;
                hashCode = (hashCode * 397) ^ EqualityComparer<T>.Default.GetHashCode(OldValue);
                hashCode = (hashCode * 397) ^ EqualityComparer<T>.Default.GetHashCode(NewValue);
                return hashCode;
            }
        }
    }
    
    public readonly struct Collection2DRemoveEvent<T> : IEquatable<Collection2DRemoveEvent<T>>
    {
        public int X { get; }
        public int Y { get; }
        public T Value { get; }

        public Collection2DRemoveEvent(int x, int y, T value)
            : this()
        {
            X = x;
            Y = y;
            Value = value;
        }

        public bool Equals(Collection2DRemoveEvent<T> other)
        {
            return X == other.X && Y == other.Y && EqualityComparer<T>.Default.Equals(Value, other.Value);
        }

        public override bool Equals(object obj)
        {
            return obj is Collection2DRemoveEvent<T> other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = X;
                hashCode = (hashCode * 397) ^ Y;
                hashCode = (hashCode * 397) ^ EqualityComparer<T>.Default.GetHashCode(Value);
                return hashCode;
            }
        }
    }

    public class Reactive2DArrayCollection<T> : IDisposable where T : class
    {
        [NonSerialized]
        private readonly T[,] array2d;
        [NonSerialized]
        bool isDisposed = false;
        
        public Reactive2DArrayCollection([NotNull]T[,] array2d)
        {
            this.array2d = array2d;
            Width = array2d.GetLength(0);
            Height = array2d.GetLength(1);
        }

        public readonly int Width;
        public readonly int Height;

        public (int x, int y)? GetCoord(T data)
        {
            if (data == null)
                return null;

            for (int y = 0; y < Height; ++y)
            {
                for (int x = 0; x < Width; ++x)
                {
                    var item = array2d[x, y];
                    if (data == item)
                        return (x, y);
                }
            }

            return null;
        }

        public void SetItem(int x, int y, T item)
        {
            var oldItem = GetItem(x, y);
            array2d[x, y] = item;
            collectionReplace?.OnNext(new Collection2DReplaceEvent<T>(x, y, oldItem, item));
        }

        public T GetItem(int x, int y)
        {
            var item = array2d[x, y];
            return item;
        }

        public void RemoveItem(int x, int y)
        {
            var item = GetItem(x, y);
            if (item != null)
            {
                array2d[x, y] = default;
                collectionRemove?.OnNext(new Collection2DRemoveEvent<T>(x, y, item));   
            }
        }

        public void SwapItem(int oldX, int oldY, int newX, int newY)
        {
            var oldItem = GetItem(oldX, oldY);
            var newItem = GetItem(newX, newY);
            SetItem(oldX, oldY, newItem);
            SetItem(newX, newY, oldItem);
            collectionMove?.OnNext(new Collection2DMoveEvent<T>(oldX, oldY, newX, newY, oldItem));
            collectionMove?.OnNext(new Collection2DMoveEvent<T>(newX, newY, oldX, oldY, newItem));
        }
        
        public void MoveItem(int oldX, int oldY, int newX, int newY)
        {
            var oldItem = GetItem(oldX, oldY);
            RemoveItem(oldX, oldY);
            SetItem(newX, newY, oldItem);
            collectionMove?.OnNext(new Collection2DMoveEvent<T>(oldX, oldY, newX, newY, oldItem));
        }
        
        [NonSerialized]
        Subject<Collection2DReplaceEvent<T>> collectionReplace = null;
        public IObservable<Collection2DReplaceEvent<T>> ObserveReplace()
        {
            if (isDisposed) return Observable.Empty<Collection2DReplaceEvent<T>>();
            return collectionReplace ??= new Subject<Collection2DReplaceEvent<T>>();
        }
        
        [NonSerialized]
        Subject<Collection2DRemoveEvent<T>> collectionRemove = null;
        public IObservable<Collection2DRemoveEvent<T>> ObserveRemove()
        {
            if (isDisposed) return Observable.Empty<Collection2DRemoveEvent<T>>();
            return collectionRemove ??= new Subject<Collection2DRemoveEvent<T>>();
        }
        
        [NonSerialized]
        Subject<Collection2DMoveEvent<T>> collectionMove = null;
        public IObservable<Collection2DMoveEvent<T>> ObserveMove()
        {
            if (isDisposed) return Observable.Empty<Collection2DMoveEvent<T>>();
            return collectionMove ??= new Subject<Collection2DMoveEvent<T>>();
        }
        
        private void DisposeSubject<TSubject>(ref Subject<TSubject> subject)
        {
            if (subject != null)
            {
                try
                {
                    subject.OnCompleted();
                }
                finally
                {
                    subject.Dispose();
                    subject = null;
                }
            }
        }

        public void Dispose()
        {
            if (!isDisposed)
            {
                DisposeSubject(ref collectionReplace);
                DisposeSubject(ref collectionRemove);
                DisposeSubject(ref collectionMove);
                isDisposed = true;   
            }
        }
    }
    
    public static class Reactive2DArrayCollectionExtensions
    {
        public static Reactive2DArrayCollection<T> ToReactiveCollection<T>(this T[,] source) where T : class
        {
            return new Reactive2DArrayCollection<T>(source);
        }
    }
}