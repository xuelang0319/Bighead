//
// = The script is part of BigHead and the framework is individually owned by Eric Lee.
// = Cannot be commercially used without the authorization.
//
//  Author  |  UpdateTime     |   Desc  
//  Eric    |  2021年5月29日   |   观察者模式
//  Eric    |  2021年7月18日   |   防止在广播过程中出现添加或移除某个监听发生问题。
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
        
        private Dictionary<string, List<Action<object[]>>> _listeners = new Dictionary<string, List<Action<object[]>>>();
        private Queue<ListenerItem> _waitAdd = new Queue<ListenerItem>();
        private Queue<ListenerItem> _waitRemove = new Queue<ListenerItem>();

        public void Broadcast(string key, params object[] message)
        {
            if (_waitAdd.Count > 0)
            {
                while (_waitAdd.Count > 0)
                {
                    var item = _waitAdd.Dequeue();
                    if (!_listeners.ContainsKey(item.Key))
                        _listeners[item.Key] = new List<Action<object[]>>();
            
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
                        $"Listener can not find callback with key: {key}, please make sure you have register your callback before".Exception();
                        return;
                    }

                    _listeners[item.Key].Remove(item.Callback);
                    if (_listeners[item.Key].Count == 0)
                        _listeners.Remove(item.Key);
                }
            }

            if (!_listeners.ContainsKey(key)) return;
            _listeners[key].ForEach(action => action?.Invoke(message));
        }

        public void Add(string key, Action<object[]> callback)
        {
            _waitAdd.Enqueue(new ListenerItem {Key = key, Callback = callback});
        }

        public void Remove(string key, Action<object[]> callback)
        {
            _waitRemove.Enqueue(new ListenerItem {Key = key, Callback = callback});
        }
    }
}