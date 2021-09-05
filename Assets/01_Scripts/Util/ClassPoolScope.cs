/*
* Copyright (c) CookApps.
* 이진호(jhlee8@cookapps.com)
*/

using System;
using System.Collections.Generic;
using Lean.Pool;
using UnityEngine;

namespace SH.Util
{
    public struct ClassPoolScope<T> : IDisposable where T : class, new()
    {
        public T Value { get; private set; }

        private static readonly Stack<T> Stack = new Stack<T>(); 

        public static ClassPoolScope<T> Spawn()
        {
            var element = Stack.Count == 0 ? new T() : Stack.Pop();
            return new ClassPoolScope<T> {Value = element};
        }
        
        public void Dispose()
        {
            if (Value != null)
            {
                Stack.Push(Value);
            }
        }
    }
}