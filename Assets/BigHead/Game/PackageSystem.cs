//
// = The script is part of BigHead and the framework is individually owned by Eric Lee.
// = Cannot be commercially used without the authorization.
//
//  Author  |  UpdateTime     |   Desc  
//  Eric    |  2021年5月29日   |   背包系统泛型
//

using System;
using System.Collections.Generic;
using System.Linq;

public class PackageSystem<T>
{
    /// <summary> 背包列表 </summary>
    private HashSet<T> _package = new HashSet<T>();
    /// <summary> SingleId索引字典 </summary>
    private Dictionary<int, T> _dictionary = new Dictionary<int, T>();
    /// <summary> 批量查找处理器 </summary>
    private Dictionary<int, Func<T, object, bool>> _handlers = new Dictionary<int, Func<T, object, bool>>();
    /// <summary> 直接通过编号查找（不安全查找，可能报空） </summary>
    public T this[int index] => _dictionary[index];

    /// <summary>
    /// 注册查找方法
    /// </summary>
    public PackageSystem<T> RegisterHandlers(int handlerIndex, Func<T, object, bool> handler)
    {
        _handlers.AddValueAndLogError(handlerIndex, handler);
        return this;
    }
    
    /// <summary>
    /// 注册背包存放元素
    /// </summary>
    public void Register(T item, int singleId)
    {
        _package.Add(item);
        _dictionary.Add(singleId, item);
    }

    /// <summary>
    /// 注销背包存放元素
    /// </summary>
    public bool UnRegister(int singleId)
    {
        if (!_dictionary.ContainsKey(singleId))
            return false;

        var t = _dictionary[singleId];
        _dictionary.Remove(singleId);
        _package.Remove(t);
        return true;
    }

    /// <summary>
    /// 尝试查找是否存在该SingleId对应的元素
    /// </summary>
    public bool TryFind(int singleId, out T item)
    {
        item = default;
        if (!_dictionary.ContainsKey(singleId))
            return false;

        item = _dictionary[singleId];
        return true;
    }

    /// <summary>
    /// 通过已经注册的筛选处理器进行批量查找
    /// </summary>
    public T[] GetOrderByLabel(int handlerIndex, object param)
    {
        if (!_handlers.ContainsKey(handlerIndex))
        {
            $"背包系统未检测到该处理器，请检查是否注册: {handlerIndex}".Exception();
            return null;
        }

        return _package.Where(prop => _handlers[handlerIndex](prop, param)).ToArray();
    }
}