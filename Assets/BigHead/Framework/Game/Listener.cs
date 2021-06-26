//
// = The script is part of BigHead and the framework is individually owned by Eric Lee.
// = Cannot be commercially used without the authorization.
//
//  Author  |  UpdateTime     |   Desc  
//  Eric    |  2021年5月29日   |   观察者模式
//

using System;
using System.Collections.Generic;
using BigHead.Framework.Core;

namespace BigHead.Framework.Game
{
    public class Listener : Singleton<Listener>
    {
        private Dictionary<string, List<Action<object[]>>> _listeners = new Dictionary<string, List<Action<object[]>>>();
        
        public void Broadcast(string key, params object[] message)
        {
            if (!_listeners.ContainsKey(key)) return;
            _listeners[key].ForEach(action => action?.Invoke(message));
        }

        public void Add(string key, Action<object[]> callback)
        {

            if (!_listeners.ContainsKey(key))
                _listeners[key] = new List<Action<object[]>>();
            
            _listeners[key].Add(callback);
        }

        public void Remove(string key, Action<object[]> callback)
        {
            if (!_listeners.ContainsKey(key) || !_listeners[key].Contains(callback))
            {
                $"Listener can not find callback with key: {key}, please make sure you have register your callback before".Exception();
                return;
            }

            _listeners[key].Remove(callback);
            if (_listeners[key].Count == 0)
                _listeners.Remove(key);
        }

        private void OnDestroy()
        {
            _listeners.Clear();
            _listeners = null;
        }
    }
}