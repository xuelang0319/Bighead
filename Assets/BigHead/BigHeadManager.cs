//
// = The script is part of Bighead and the framework is individually owned by Eric Lee.
// = Cannot be commercially used without the authorization.
//
//  Author  |  UpdateTime     |   Desc  
//  Eric    |  2021年1月8日    |   框架管理器
//

using System;
using UnityEngine;

public class BigHeadManager : MonoGlobalSingleton<BigHeadManager>
{
    /// <summary>
    /// 预处理帧事件
    /// </summary>
    public Action<float> PreUpdateEvent;

    /// <summary>
    /// 帧调用事件
    /// </summary>
    public Action<float> UpdateEvent;

    /// <summary>
    /// 后处理帧调用事件
    /// </summary>
    public Action<float> LateUpdateEvent;

    /// <summary>
    /// 无视时间缩放帧调用事件，不可与帧调用事件同时注册
    /// </summary>
    public Action<float> IgnoreTimescaleUpdateEvent;

    /// <summary>
    /// 固定帧率调用事件，该事件流不与其他帧事件相关联
    /// </summary>
    public Action<float> FixedUpdateEvent;
    
    /// <summary>
    /// 销毁事件
    /// </summary>
    public Action DestroyEvent;

    private void OnDestroy()
    {
        DestroyEvent?.Invoke();
    }

    private void Update()
    {
        var deltaTime = Time.deltaTime;
        PreUpdateEvent?.Invoke(deltaTime);
        UpdateEvent?.Invoke(deltaTime);
        IgnoreTimescaleUpdateEvent?.Invoke(Time.fixedUnscaledDeltaTime);
    }

    private void LateUpdate()
    {
        LateUpdateEvent?.Invoke(Time.deltaTime);
    }

    private void FixedUpdate()
    {
        FixedUpdateEvent?.Invoke(Time.fixedDeltaTime);
    }
}