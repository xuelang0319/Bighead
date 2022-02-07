//
// = The script is part of BigHead and the framework is individually owned by Eric Lee.
// = Cannot be commercially used without the authorization.
//
//  Author  |  UpdateTime     |   Desc  
//  Eric    |  2021年5月29日   |   观察者模式
//  Eric    |  2021年7月18日   |   防止在广播过程中出现添加或移除某个监听发生问题。
//  Eric    |  2021年7月23日   |   添加广播信息存储，便用于初始化传参等方法，防止由于特殊原因无法指定传参也由于初始化顺序导致消息遗漏等情况。
//  Eric    |  2022年2月07日   |   尝试使用双向链表结构处理广播系统，将原广播系统更改名称为GameListener, 目前Listener还没有进行测试
//

using System;
using System.Collections.Generic;
using BigHead.Framework.Core;
using UnityEngine;

public sealed class Listener : Singleton<Listener>
{
    private readonly SingleId _singleId = SingleId.New();
    private readonly Dictionary<int, DoubleLinkManager> _listeners = new Dictionary<int, DoubleLinkManager>();
    private readonly Dictionary<int, Queue<object[]>> _storeMessages = new Dictionary<int, Queue<object[]>>();
    private readonly Dictionary<int, ListenerItem> _allListeners = new Dictionary<int, ListenerItem>();

    /// <summary>
    /// 广播消息
    /// </summary>
    /// <param name="key">广播主键</param>
    /// <param name="param">参数</param>
    public void Broadcast(string key, params object[] param)
    {
        var hash = Animator.StringToHash(key);
        if (!_listeners.TryGetValue(hash, out var manager)) return;
        manager.Do(node =>
        {
            if (node is ListenerItem item)
                item.Callback?.Invoke(param);
        });
    } 
    
    /// <summary>
    /// 广播消息，如果没有监听则会存储，广播给第一个存储进来的监听者
    /// </summary>
    /// <param name="key"></param>
    /// <param name="param"></param>
    public void BroadcastStorageMessage(string key, params object[] param)
    {
        var hash = Animator.StringToHash(key);
        if (!_listeners.TryGetValue(hash, out var manager))
        {
            if (!_storeMessages.ContainsKey(hash))
                _storeMessages[hash] = new Queue<object[]>();
            _storeMessages[hash].Enqueue(param);
            return;
        }
        
        manager.Do(node =>
        {
            if (node is ListenerItem item)
                item.Callback?.Invoke(param);
        });
    }
    
    /// <summary>
    /// 添加监听
    /// </summary>
    /// <param name="key">监听主键</param>
    /// <param name="callback">监听回调</param>
    public int Add(string key, Action<object[]> callback)
    {
        var hash = Animator.StringToHash(key);
        if (_storeMessages.TryGetValue(hash, out var queue))
        {
            while (queue.Count > 0)
                callback?.Invoke(queue.Dequeue());

            _storeMessages.Remove(hash);
        }

        var item = new ListenerItem() {Callback = callback};
        if (!_listeners.ContainsKey(hash))
            _listeners[hash] = new DoubleLinkManager();
        
        _listeners[hash].Add(item);
        var id = _singleId.Get();
        _allListeners.Add(id, item);
        return id;
    }

    /// <summary>
    /// 移除监听
    /// </summary>
    /// <param name="key">监听主键</param>
    /// <param name="singleId">添加时返回的唯一ID</param>
    public void Remove(string key, int singleId)
    {
        if (!_allListeners.TryGetValue(singleId, out var item))
            return;
        _allListeners.Remove(singleId);
        
        var hash = Animator.StringToHash(key);
        if (!_listeners.TryGetValue(hash, out var manager))
            return;
        
        manager.Remove(item);
        if (manager.Count == 0)
            _listeners.Remove(hash);
    }

    private class ListenerItem : IDoubleLinkNode
    {
        public IDoubleLinkNode Front { get; set; }
        public IDoubleLinkNode Next { get; set; }
        public Action<object[]> Callback;
    }
}



namespace BigHead.Framework.Game
{
    public sealed class GameListener : Singleton<GameListener>
    {
        /// <summary> 执行状态 </summary>
        private bool _working = false;
        
        /// <summary> 监听器集合 </summary>
        private readonly Dictionary<string, HashSet<Action<object[]>>> _listeners =
            new Dictionary<string, HashSet<Action<object[]>>>();
        
        /// <summary> 滞留消息集合 </summary>
        private readonly Dictionary<string, HashSet<object[]>> _storeMessages = 
            new Dictionary<string, HashSet<object[]>>();

        /// <summary> 操作队列 </summary>
        /// Type : 0 - BroadcastMessage, 1 - BroadcastStorageMessage, 2 - Add, 3 - Remove
        private readonly Queue<(int type, string key, object[] param)> _waitOperations =
            new Queue<(int type, string key, object[] param)>();
            
        /// <summary>
        /// 广播消息
        /// </summary>
        /// <param name="key">广播主键</param>
        /// <param name="param">参数</param>
        public void Broadcast(string key, params object[] param)
        {
            AddOperation(0, key, param);
        }
        
        /// <summary>
        /// 广播消息，如果没有监听则会存储，广播给第一个存储进来的监听者
        /// </summary>
        /// <param name="key"></param>
        /// <param name="param"></param>
        public void BroadcastStorageMessage(string key, params object[] param)
        {
            AddOperation(1, key, param);
        }

        /// <summary>
        /// 添加监听
        /// </summary>
        /// <param name="key">监听主键</param>
        /// <param name="callback">监听回调</param>
        public void Add(string key, Action<object[]> callback)
        {
            AddOperation(2, key, callback);
        }  
        
        /// <summary>
        /// 移除监听
        /// </summary>
        /// <param name="key">监听主键</param>
        /// <param name="callback">监听回调</param>
        public void Remove(string key, Action<object[]> callback)
        {
            AddOperation(3, key, callback);
        }

        /// <summary>
        /// 添加操作
        /// </summary>
        /// <param name="type">操作类型</param>
        /// <param name="key">主键</param>
        /// <param name="param">参数</param>
        private void AddOperation(int type, string key, params object[] param)
        {
            _waitOperations.Enqueue((type, key, param));
            StartWorking();
        }

        /// <summary>
        /// 开始工作
        /// </summary>
        private void StartWorking()
        {
            if (_working) 
                return;

            _working = true;
            
            while (_waitOperations.Count > 0)
            {
                var (type, key, param) = _waitOperations.Dequeue();
                switch (type)
                {
                    // BroadcastMessage
                    case 0:
                        if (_listeners.ContainsKey(key))
                        {
                            foreach (var action in _listeners[key])
                            {
                                action?.Invoke(param);
                            }
                        }

                        break;
                    
                    // BroadcastStorageMessage
                    case 1:
                        if (_listeners.ContainsKey(key))
                        {
                            foreach (var action in _listeners[key])
                            {
                                action?.Invoke(param);
                            }
                        }
                        else
                        {
                            if (!_storeMessages.ContainsKey(key))
                            {
                                _storeMessages[key] = new HashSet<object[]>();
                            }

                            _storeMessages[key].Add(param);
                        }

                        break;
                    
                    // AddListener
                    case 2:
                        if (!_listeners.ContainsKey(key))
                        {
                            _listeners[key] = new HashSet<Action<object[]>>();
                        }

                        var callback = (Action<object[]>) param[0];
                        _listeners[key].Add(callback);
                        
                        if (_storeMessages.ContainsKey(key))
                        {
                            var array = _storeMessages[key];
                            _storeMessages.Remove(key);
                            foreach (var objects in array)
                            {
                                callback?.Invoke(objects);
                            }
                        }

                        break;
                    
                    // RemoveListener
                    case 3:
                        if (!_listeners.ContainsKey(key))
                        {
                            $"Remove listener failed. Can't find listener key : {key}".Exception();
                            break;
                        }

                        callback = (Action<object[]>) param[0];
                        if (_listeners[key].Remove(callback))
                        {
                            if (_listeners[key].Count == 0)
                            {
                                _listeners.Remove(key);
                            }
                        }
                        else
                        {
                            $"Remove listener failed. Can't find listener callback. key : {key}".Exception();
                        }

                        break;
                    
                    default:
                        $"Listener type error. Type : {type}".Exception();
                        break;
                }
            }

            _working = false;
        }
    }
}