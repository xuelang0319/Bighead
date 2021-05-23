//
// = The script is part of BigHead and the framework is individually owned by Eric Lee.
// = Cannot be commercially used without the authorization.
//
//  Author  |  UpdateTime     |   Desc  
//  Eric    |  2020年12月18日  |   对象池
//

using System;
using System.Collections.Generic;
using BigHead.Framework.Core;
using UnityEngine;
using Object = UnityEngine.Object;

namespace BigHead.Framework.Pool
{
    public static class PoolAssistant
    {
        public static Pool<T> GetPool<T>(
            Func<T> createFoo = null,
            Action<T> recycleFoo = null, 
            Action<T> clearFoo = null) where T : class
        {
            return new Pool<T>(createFoo, recycleFoo, clearFoo);
        }

        public static MonoPool<T> GetMonoPool<T>(
            string parentName,
            Func<T> createFoo = null,
            Action<T> recycleFoo = null,
            Action<T> clearFoo = null) where T : class
        {
            return new MonoPool<T>(parentName, createFoo, recycleFoo, clearFoo);
        }
    }

    public class Pool<T> where T : class
    {
        private readonly Stack<T> _pool;

        private Func<T> _createFoo;
        private Action<T> _recycleFoo;
        private Action<T> _clearFoo;

        public Pool(Func<T> createFoo = null, Action<T> recycleFoo = null, Action<T> clearFoo = null)
        {
            _pool = new Stack<T>();
            _createFoo = createFoo;
            _recycleFoo = recycleFoo;
            _clearFoo = clearFoo;
        }

        public void Recycle(T item)
        {
            _recycleFoo?.Invoke(item);
            _pool.Push(item);
        }

        public T Get()
        {
            return _pool.Count > 0 ? _pool.Pop() : _createFoo?.Invoke();
        }

        public void Clear()
        {
            while (_pool.Count > 0)
            {
                var item = _pool.Pop();
                _clearFoo?.Invoke(item);
            }
        }
    }


    public class MonoPool<T> where T : class
    {
        private readonly Pool<T> _pool;
        private Transform _parent { get; }

        public MonoPool(string parentName = "", 
            Func<T> createFoo = null, 
            Action<T> recycleFoo = null, 
            Action<T> clearFoo = null)
        {
            var component = new GameObject(string.IsNullOrEmpty(parentName)? "MonoPool" : parentName).AddComponent<PoolComponent>();
            component.Initialize(OnParentDestroy);
            _parent = component.transform;
            _pool = new Pool<T>(
                createFoo, 
                (item =>
            {
                if (item is GameObject gameObject)
                    gameObject.transform.SetParent(_parent);
                
                recycleFoo?.Invoke(item);
            }), 
                clearFoo);
        }

        public T Get() => _pool.Get();
        
        public void Recycle(T item) => _pool.Recycle(item);

        private void OnParentDestroy() => _pool.Clear();
        
        public void Clear() => Object.Destroy(_parent.gameObject);
    }
}