//
// = The script is part of BigHead and the framework is individually owned by Eric Lee.
// = Cannot be commercially used without the authorization.
//
//  Author  |  UpdateTime     |   Desc  
//  Eric    |  2021年7月26日   |   Resources资源加载，在有可能切换加载方式的前提下建议使用Res_Common提供的方法进行加载。
//

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public partial class Res
{
    /// <summary>
    /// 异步在Resources中加载资源
    /// </summary>
    /// <param name="key">资源键</param>
    /// <param name="callback">完成回调</param>
    /// <typeparam name="T">资源类型</typeparam>
    public void ResourcesLoadAsset<T>(string key, Action<T> callback) where T : UnityEngine.Object
    {
        if (ContainKey(key))
        {
            var result = (T) Get(key);
            callback?.Invoke(result);
            return;
        }
        
        _loadings.Add(key, new HashSet<object>() {callback});
        StartCoroutine(_AsyncLoadAssetsInResources<T>(key));
    }

    private IEnumerator _AsyncLoadAssetsInResources<T>(string key) where T : UnityEngine.Object
    {
        var t = Resources.LoadAsync<T>(key);
        yield return t;

        Register(key, t.asset, EnumAssetSource.Resources);
        
        var hashset = _loadings[key];
        _loadings.Remove(key);

        foreach (var callback in hashset.Cast<Action<T>>())
            callback?.Invoke((T)t.asset);
    }

    /// <summary>
    /// 同步加载
    /// </summary>
    /// <param name="key">资源键</param>
    /// <typeparam name="T">资源类型</typeparam>
    /// <returns></returns>
    public T ResourcesLoadAsset<T>(string key) where T : UnityEngine.Object
    {
        var asset = Resources.Load<T>(key);
        Register(key, asset, EnumAssetSource.Resources);
        return asset;
    }
}