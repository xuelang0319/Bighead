//
// = The script is part of BigHead and the framework is individually owned by Eric Lee.
// = Cannot be commercially used without the authorization.
//
//  Author  |  UpdateTime     |   Desc  
//  Eric    |  2021年7月23日   |   资源存储管理器
//

using System.Collections.Generic;
using BigHead.Framework.Core;
using UnityEngine.AddressableAssets;

public partial class Res
{
    /// <summary> 预加载资源 </summary>
    private Dictionary<string, object> _preloads = new Dictionary<string, object>();

    /// <summary>
    /// 注册预加载资源
    /// </summary>
    /// <param name="key">资源Key</param>
    /// <param name="value">资源</param>
    public void Register(string key, object value)
    {
        if (_preloads.ContainsKey(key))
        {
            $"This resource is already register, do not register twice. Resource key: {key}".Exception();
            return;
        }

        _preloads[key] = value;
    }

    /// <summary>
    /// 获取预加载资源
    /// </summary>
    /// <param name="key">资源Key</param>
    /// <returns>资源，可能返回default</returns>
    public object Get(string key)
    {
        _preloads.TryGetValue(key, out var asset);
        return asset;
    }

    /// <summary>
    /// 清空资源
    /// </summary>
    public void Clear()
    {
        if (Equals(_preloads.Count, 0)) return;
        UnRegister(_preloads.Keys);
    }

    /// <summary>
    /// 卸载部分资源
    /// </summary>
    /// <param name="keys">资源Key合集</param>
    public void UnRegister(IEnumerable<string> keys)
    {
        foreach (var key in keys)
            UnRegister(key);
    }

    /// <summary>
    /// 卸载单一资源
    /// </summary>
    /// <param name="key">资源Key</param>
    public void UnRegister(string key)
    {
        if (!_preloads.ContainsKey(key))
        {
            $"This resource is not registered, please check. Resource Key: {key}".Exception();
            return;
        }
        Addressables.Release(key);
    }
}