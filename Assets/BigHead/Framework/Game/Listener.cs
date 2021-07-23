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
    public class Listener : Singleton<Listener>
    {
        public class ListenerItem
        {
            public string Key;
            public Action<object[]> Callback;
        }
        
        /// <summary> 监听器集合 </summary>
        private Dictionary<string, HashSet<Action<object[]>>> _listeners = new Dictionary<string, HashSet<Action<object[]>>>();
        /// <summary> 滞留消息集合 </summary>
        private Dictionary<string, HashSet<object[]>> _storeMessages = new Dictionary<string, HashSet<object[]>>();
        /// <summary> 等待加入集合 </summary>
        private Queue<ListenerItem> _waitAdd = new Queue<ListenerItem>();
        /// <summary> 等待移除集合 </summary>
        private Queue<ListenerItem> _waitRemove = new Queue<ListenerItem>();

        /// <summary>
        /// 广播信息，如果没有监听者将存储起来，当第一个监听者加入时统一进行接收
        /// </summary>
        public void BroadcastStorageMessage(string key, params object[] message)
        {
            CheckState();
            if (_listeners.ContainsKey(key))
            {
                foreach (var action in _listeners[key])
                    action?.Invoke(message);
            }
            else
            {
                if (!_storeMessages.ContainsKey(key))
                    _storeMessages[key] = new HashSet<object[]>();
                _storeMessages[key].Add(message);
            }
        }

        /// <summary>
        /// 广播信息
        /// </summary>
        public void Broadcast(string key, params object[] message)
        {
            CheckState();
            if (!_listeners.ContainsKey(key)) return;
            foreach (var action in _listeners[key])
                action?.Invoke(message);
        }

        /// <summary>
        /// 检查等待加入和移除状态
        /// </summary>
        private void CheckState()
        {
            if (_waitAdd.Count > 0)
            {
                while (_waitAdd.Count > 0)
                {
                    var item = _waitAdd.Dequeue();
                    if (!_listeners.ContainsKey(item.Key))
                        _listeners[item.Key] = new HashSet<Action<object[]>>();
            
                    _listeners[item.Key].AddUniqueValue(item.Callback);
                }
            }

            if (_waitRemove.Count > 0)
            {
                while (_waitRemove.Count > 0)
                {
                    var item = _waitRemove.Dequeue();
                    if (!_listeners.ContainsKey(item.Key) || !_listeners[item.Key].Contains(item.Callback))
                    {
                        $"Listener can not find callback with key: {item.Key}, please make sure you have register your callback before".Exception();
                        return;
                    }

                    _listeners[item.Key].Remove(item.Callback);
                    if (_listeners[item.Key].Count == 0)
                        _listeners.Remove(item.Key);
                }
            }
        }

        /// <summary>
        /// 添加监听器
        /// </summary>
        public void Add(string key, Action<object[]> callback)
        {
            _waitAdd.Enqueue(new ListenerItem {Key = key, Callback = callback});
            if (!_storeMessages.ContainsKey(key)) return;
            var hashset = _storeMessages[key];
            _storeMessages.Remove(key);
            foreach (var message in hashset)
                callback?.Invoke(message);
        }

        /// <summary>
        /// 移除监听器
        /// </summary>
        public void Remove(string key, Action<object[]> callback)
        {
            _waitRemove.Enqueue(new ListenerItem {Key = key, Callback = callback});
        }
    }
}