//
// = The script is part of BigHead and the framework is individually owned by Eric Lee.
// = Cannot be commercially used without the authorization.
//
//  Author  |  UpdateTime     |   Desc  
//  Eric    |  2021年7月26日   |   通用资源加载，这里主要考虑到在不修改加载方法和逻辑代码的前提下，通过修改BigheadConfig的加载方式直接转换加载方式。从本地化进入服务器化。
//

using System;
using UnityEngine;
using UnityEngine.U2D;

public partial class Res
{
    // TODO:这里考虑后续根据加载的方式对键修改的封装
    
    public void AsyncLoadPrefab(string key, Action<GameObject> callback)
    {
        if (BigheadConfig.LoadInBundle_Prefab) 
            AddressableLoadAsset(key, callback);
        else
            ResourcesLoadAsset(key, callback);
    }

    public void AsyncLoadSprite(string key, Action<Sprite> callback)
    {
        if (BigheadConfig.LoadInBundle_Sprite) 
            AddressableLoadAsset(key, callback);
        else 
            ResourcesLoadAsset(key, callback);
    }

    public void AsyncLoadSprite(string atlaskey, string name, Action<Sprite> callback)
    {
        Action<SpriteAtlas> onComplete = atlas =>
            callback?.Invoke(atlas.GetSprite(name));

        if (BigheadConfig.LoadInBundle_Sprite) 
            AddressableLoadAsset(atlaskey, onComplete);
        else 
            ResourcesLoadAsset(atlaskey, onComplete);
    }

    public void AsyncLoadConfig(string key, Action<TextAsset> callback)
    {
        if (BigheadConfig.LoadInBundle_Config) 
            AddressableLoadAsset(key, callback);
        else 
            ResourcesLoadAsset(key, callback);
    }
    
    public void AsyncLoadAudioClip(string key, Action<AudioClip> callback)
    {
        if (BigheadConfig.LoadInBundle_Sound) 
            AddressableLoadAsset(key, callback);
        else 
            ResourcesLoadAsset(key, callback);
    }
}