//
// = The script is part of Bighead and the framework is individually owned by Eric Lee.
// = Cannot be commercially used without the authorization.
//
//  Author  |  UpdateTime     |   Desc  
//  Eric    |  2021年1月8日    |   框架管理器
//

using System;
using UnityEngine;

namespace BigHead
{
    public class BigHeadManager : MonoGlobalSingleton<BigHeadManager>
    {
        /// <summary>
        /// 帧调用间隔时间
        /// </summary>
        private float UpdateIntervalTime;
        
        /// <summary>
        /// 帧调用事件
        /// </summary>
        public Action<float> UpdateEvent;
        
        /// <summary>
        /// 销毁事件
        /// </summary>
        public Action DestroyEvent;

        private void Awake()
        {
            UpdateIntervalTime = Time.realtimeSinceStartup;
        }

        private void OnDestroy()
        {
            DestroyEvent?.Invoke();
        }
        
        private void Update()
        {
            var now = Time.realtimeSinceStartup;
            var intervalTime = now - UpdateIntervalTime;
            UpdateIntervalTime = now;
            UpdateEvent?.Invoke(intervalTime);
        }
    }
}