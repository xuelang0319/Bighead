//
// = The script is part of BigHead and the framework is individually owned by Eric Lee.
// = Cannot be commercially used without the authorization.
//
//  Author  |  UpdateTime     |   Desc  
//  Eric    |  2021年7月26日   |   Addressable 资源加载。
//

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public partial class Res
{
    /// <summary> 加载中的资源 </summary>
    private Dictionary<string, HashSet<object>> _loadings = new Dictionary<string, HashSet<object>>();
    
    /// <summary>
    /// 异步加载资源
    /// </summary>
    /// <param name="key">资源键</param>
    /// <param name="callback">完成回调</param>
    /// <typeparam name="T">资源类型</typeparam>
    public void LoadAsset<T>(string key, Action<T> callback)
    {
        if (ContainKey(key))
        {
            var obj = (T) Get(key);
            callback?.Invoke(obj);
            return;
        }

        if (_loadings.ContainsKey(key))
        {
            _loadings[key].Add(callback);
            return;
        }

        _loadings[key] = new HashSet<object> {callback};

        Addressables.LoadAssetAsync<T>(key).Completed += handle =>
        {
            if (!Equals(handle.Status, AsyncOperationStatus.Succeeded))
            {
                $"Load asset failed, asset key: {key}, please check.".Exception();
                _loadings.Remove(key);
                return;
            }

            Register(key, handle.Result, EnumAssetSource.Addressable);
            
            var hashset = _loadings[key];
            _loadings.Remove(key);
            
            foreach (var item in hashset)
            {
                var function = item as Action<T>;
                function?.Invoke(handle.Result);
            }
        };
    }
    
    /// <summary>
    /// 预加载资源
    /// </summary>
    /// <param name="keys">预加载资源键集合</param>
    /// <param name="residueCallback">剩余数量回调，完成时为0</param>
    public void PreloadAssets(IEnumerable<string> keys, Action<int> residueCallback)
    {
        if (Equals(null, keys))
        {
            "Async preload assets invoke failed, the given key is null. Please check.".Exception();
            return;
        }

        var unloadKeys = keys.Except(_loadings.Keys).ToArray();
        var unloadCount = unloadKeys.Length;

        if (Equals(unloadCount, 0))
        {
            residueCallback?.Invoke(0);
            return;
        }

        for (int i = 0; i < unloadKeys.Length; i++)
        {
            var key = unloadKeys[i];
            LoadAsset<object>(key, asset =>
            {
                --unloadCount;
                residueCallback?.Invoke(unloadCount);
            });
        }
    }

    /// <summary>
    /// 根据标签异步加载资源
    /// </summary>
    /// <param name="label">资源标签</param>
    /// <param name="callback">完成回调</param>
    /// <typeparam name="T">资源类型</typeparam>
    public void LoadAssetsByLabel<T>(string label, Action<IList<T>> callback)
    {
        if (ContainKey(label))
        {
            var obj = (IList<T>) Get(label);
            callback?.Invoke(obj);
            return;
        }

        if (_loadings.ContainsKey(label))
        {
            _loadings[label].Add(callback);
            return;
        }

        _loadings[label] = new HashSet<object>() {callback};

        Addressables.LoadAssetsAsync<T>(label, null).Completed += handle =>
        {
            if (!Equals(handle.Status, AsyncOperationStatus.Succeeded))
            {
                $"Load asset by label failed, label name: {label}, please check.".Exception();
                _loadings.Remove(label);
                return;
            }

            Register(label, handle.Result, EnumAssetSource.Addressable);
            
            var hashset = _loadings[label];
            _loadings.Remove(label);
            
            foreach (var item in hashset)
            {
                var function = item as Action<IList<T>>;
                function?.Invoke(handle.Result);
            }
        };
    }

    /// <summary>
    /// 卸载全部资源并预加载指定资源，通常用于转场的资源释放和重新添加。
    /// </summary>
    /// <param name="keys">资源集合键</param>
    /// <param name="residueCallback">剩余量回调</param>
    public void ReleaseAllAndPreloadAssets(IEnumerable<string> keys, Action<int> residueCallback)
    {
        var enumerable = keys as string[] ?? keys.ToArray();
        if (Equals(enumerable.Length, 0))
        {
            "Release all and preload assets invoke failed, the given key is null. Please check.".Exception();
            return;
        }

        var uselessKeys = _preloads.Keys.Except(enumerable);
        foreach (var uselessKey in uselessKeys)
            UnRegister(uselessKey);

        var preloadKeys = enumerable.Except(_preloads.Keys);
        PreloadAssets(preloadKeys, residueCallback);
    }
}