//
// = The script is part of Bighead and the framework is individually owned by Eric Lee.
// = Cannot be commercially used without the authorization.
//
//  Author  |  UpdateTime     |   Desc  
//  Eric    |  2021年9月1日    |   Animator状态机封装，提供真实时间播放动作的方法。
//

using System;
using System.Collections.Generic;
using BigHead.Framework.Core;
using UnityEngine;

namespace BigHead.Framework.Encapsulation
{
    public class AnimatorPlus : Animator
    {
        /// <summary>
        /// 动画片段信息
        /// </summary>
        class AnimationClipSpeedInfo
        {
            /// <summary> 速度参数名称 </summary>
            public string SpeedParamName;
            /// <summary> 动画原生时长 </summary>
            public float AnimationLength;
            /// <summary> 当前设置时长 </summary>
            public float CurrentLength;
        }
        
        /// <summary> 速度信息字典 </summary>
        private Dictionary<string, AnimationClipSpeedInfo> _speedInfos = 
            new Dictionary<string, AnimationClipSpeedInfo>();

        /// <summary>
        /// 初始化方法, 字典中对应每个状态机的名称和对应控制的名称
        /// </summary>
        /// <param name="data">参数， Key - StateName, Value - ParameterName</param>
        public void Init(Dictionary<string, string> data)
        {
            foreach (var kv in data)
            {
                _speedInfos.Add(kv.Key, new AnimationClipSpeedInfo{SpeedParamName = kv.Value});
            }
            
            ForeachClip(clip =>
            {
                if (!_speedInfos.ContainsKey(clip.name))
                    return;

                var info = _speedInfos[clip.name];
                info.AnimationLength = info.CurrentLength = clip.length;
            });
        }
    
        /// <summary>
        /// 遍历Animator中的AnimationClip
        /// </summary>
        /// <param name="callback">逐个回调</param>
        private void ForeachClip(Action<AnimationClip> callback)
        {
            var clips = runtimeAnimatorController.animationClips;
            var length = clips.Length;
            for (int i = 0; i < length; i++)
            {
                callback?.Invoke(clips[i]);
            }
        }

        /// <summary>
        /// 强制比对更新时间。
        /// 如果与当前时间相同则不改变。
        /// 如果不同则用原始时间计算播放速度比例。
        /// </summary>
        /// <param name="stateName">状态机名称</param>
        /// <param name="realTime">真实播放时间</param>
        private void ForceUpdateStateSpeed(string stateName, float realTime)
        {
            if (!_speedInfos.ContainsKey(stateName))
            {
                $"AnimatorPlus don't have key : {stateName}, please register first".Warning();
                return;
            }

            var info = _speedInfos[stateName];
            if (Mathf.Abs(info.CurrentLength - realTime) < 0.001f)
                return;

            var currentSpeed = info.AnimationLength / realTime;
            info.CurrentLength = realTime;
            SetFloat(info.SpeedParamName.Trim(), info.AnimationLength * currentSpeed);
        }

        /// <summary>
        /// AnimatorPlus的封装方法，播放指定名称的状态机
        /// </summary>
        /// <param name="layer">状态机所在层级</param>
        /// <param name="stateName">状态机名称</param>
        /// <param name="realTime">真实播放时间</param>
        public void Play(int layer, string stateName, float realTime)
        {
            ForceUpdateStateSpeed(stateName, realTime);
            Play(stateName, layer);
        }
    }
}