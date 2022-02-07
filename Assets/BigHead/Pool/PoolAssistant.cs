//
// = The script is part of BigHead and the framework is individually owned by Eric Lee.
// = Cannot be commercially used without the authorization.
//
//  Author  |  UpdateTime     |   Desc  
//  Eric    |  2020年12月18日  |   对象池
//

using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace BigHead.Framework.Pool
{
    public static class PoolAssistant
    {
        /// <summary>
        /// 普通全局对象池，只能挂载非MonoBehaviour的对象池。
        /// </summary>
        public static Pool<T> GetPool<T>(
            Func<T> createFoo = null,
            Action<T> recycleFoo = null, 
            Action<T> clearFoo = null) where T : class
        {
            return new Pool<T>(createFoo, recycleFoo, clearFoo);
        }

        /// <summary>
        /// Mono场景对象池，可以挂载所有对象，切换场景时清除所有存储对象。
        /// </summary>
        public static MonoPool<T> GetMonoScenePool<T>(
            string parentName,
            Func<T> createFoo = null,
            Action<T> recycleFoo = null,
            Action<T> clearFoo = null) where T : class
        {
            return new MonoPool<T>(parentName, createFoo, recycleFoo, clearFoo);
        }
        
        /// <summary>
        /// Mono场景对象池，可以挂载所有对象，存储对象不会随着切换场景而清除。
        /// </summary>
        public static MonoPool<T> GetMonoGlobalPool<T>(
            string parentName,
            Func<T> createFoo = null,
            Action<T> recycleFoo = null,
            Action<T> clearFoo = null) where T : class
        {
            var pool = new MonoPool<T>(parentName, createFoo, recycleFoo, clearFoo);
            Object.DontDestroyOnLoad(pool.Parent);
            return pool;
        }
    }

    public class Pool<T> where T : class
    {
        private readonly Stack<T> _pool;

        private readonly Func<T> _createFoo;
        private readonly Action<T> _recycleFoo;
        private readonly Action<T> _clearFoo;

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
        public Transform Parent { get; set; }
        
        public MonoPool(string parentName = "", 
            Func<T> createFoo = null, 
            Action<T> recycleFoo = null, 
            Action<T> clearFoo = null)
        {
            var component = new GameObject(string.IsNullOrEmpty(parentName)? "MonoPool" : parentName).AddComponent<PoolComponent>();
            component.Initialize(OnParentDestroy);
            Parent = component.transform;
            _pool = new Pool<T>(
                createFoo, 
                (item =>
            {
                if (item is GameObject gameObject)
                {
                    gameObject.transform.SetParent(Parent);
                    gameObject.SetActive(false);
                }
                
                else if (item is Component com)
                {
                    com.transform.SetParent(Parent);
                    com.gameObject.SetActive(false);
                }
                
                recycleFoo?.Invoke(item);
            }), 
                clearFoo);
        }

        public T Get()
        {
            var element = _pool.Get();
            if (element is GameObject gameObject)
            {
                gameObject.SetActive(true);
            }
            else if (element is Component com)
            {
                com.gameObject.SetActive(true);
            }

            return element;
        }
        
        public void Recycle(T item) => _pool.Recycle(item);

        private void OnParentDestroy() => _pool.Clear();
        
        public void Clear() => Object.Destroy(Parent.gameObject);
    }
}