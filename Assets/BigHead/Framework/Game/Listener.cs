//
// = The script is part of BigHead and the framework is individually owned by Eric Lee.
// = Cannot be commercially used without the authorization.
//
//  Author  |  UpdateTime     |   Desc  
//  Eric    |  2021年5月29日   |   观察者模式
//  Eric    |  2021年7月18日   |   防止在广播过程中出现添加或移除某个监听发生问题。
//  Eric    |  2021年7月23日   |   添加广播信息存储，便用于初始化传参等方法，防止由于特殊原因无法指定传参也由于初始化顺序导致消息遗漏等情况。
//

using System;
using System.Collections.Generic;
using BigHead.Framework.Core;
using BigHead.Framework.Extension;

namespace BigHead.Framework.Game
{
    public sealed class Listener : Singleton<Listener>
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