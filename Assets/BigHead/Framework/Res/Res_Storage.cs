//
// = The script is part of BigHead and the framework is individually owned by Eric Lee.
// = Cannot be commercially used without the authorization.
//
//  Author  |  UpdateTime     |   Desc  
//  Eric    |  2021年7月23日   |   资源存储管理器
//

using System.Collections.Generic;
using BigHead.Framework.Core;
using UnityEngine;
using UnityEngine.AddressableAssets;

public partial class Res
{
    /// <summary> 预加载资源 </summary>
    private Dictionary<string, PreloadItem> _preloads = new Dictionary<string, PreloadItem>();

    public class PreloadItem
    {
        public object Value;
        public EnumAssetSource AssetSource;
    }
    
    public enum EnumAssetSource
    {
        Addressable,
        Resources
    }

    /// <summary>
    /// 检测资源是否存在
    /// </summary>
    /// <param name="key">资源键</param>
    /// <returns>是否存在</returns>
    public bool ContainKey(string key)
    {
        return _preloads.ContainsKey(key);
    }

    /// <summary>
    /// 注册预加载资源
    /// </summary>
    /// <param name="key">资源Key</param>
    /// <param name="value">资源</param>
    /// <param name="source">资源来源</param>
    public void Register(string key, object value, EnumAssetSource source)
    {
        if (_preloads.ContainsKey(key))
        {
            $"This resource is already register, do not register twice. Resource key: {key}".Exception();
            return;
        }

        _preloads[key] = new PreloadItem() {Value = value, AssetSource = source};
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

        var asset = _preloads[key];
        _preloads.Remove(key);
        
        if (Equals(asset.AssetSource, EnumAssetSource.Addressable))
        {
            Addressables.Release(key);
        }
        
        if (Equals(asset.AssetSource, EnumAssetSource.Resources))
        {
            Resources.UnloadAsset((Object) asset.Value);
        }
    }
}