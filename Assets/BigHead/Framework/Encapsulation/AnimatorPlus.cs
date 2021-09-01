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
using UnityEditor.Animations;
using UnityEngine;

namespace BigHead.Framework.Encapsulation
{
    public class AnimatorPlus : Animator
    {
        /// <summary> 动作当前时间 </summary>
        private Dictionary<string, float> _actionTimes = new Dictionary<string, float>();
        /// <summary> 动作原始时间 </summary>
        private Dictionary<string, float> _originTimes = new Dictionary<string, float>();
        /// <summary> 初始化状态 </summary>
        private bool _hasInit = false;

        /// <summary>
        /// 初始化方法
        /// </summary>
        private void Step()
        {
            if (_hasInit)
                return;

            _hasInit = true;
            ForeachClip(clip =>
            {
                // 融合动作可能会添加两次，这里做排除
                if (_originTimes.ContainsKey(clip.name))
                    return;

                _originTimes.Add(clip.name, clip.length);
                _actionTimes.Add(clip.name, clip.length);
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
        /// 遍历Animator中的AnimatorState
        /// </summary>
        /// <param name="callback">逐个回调</param>
        private void ForeachState(Action<AnimatorState> callback)
        {
            var layers = ((AnimatorController) runtimeAnimatorController).layers;
            var length = layers.Length;
            for (int i = 0; i < length; i++)
            {
                var childStates = layers[i].stateMachine.states;
                length = childStates.Length;
                for (int j = 0; j < length; j++)
                {
                    callback?.Invoke(childStates[j].state);
                }
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
            Step();
            if (!_actionTimes.ContainsKey(stateName))
            {
                $"Play animator failed. {name} can not find state name - {stateName}.".Exception();
                return;
            }

            if (Mathf.Abs(_actionTimes[stateName] - realTime) < 0.01f)
                return;
        
            ForeachState( clip =>
            {
                if (clip.name != stateName)
                    return;

                clip.speed = _originTimes[stateName] / realTime;
                _actionTimes[stateName] = realTime;
            });
        }

        /// <summary>
        /// AnimatorPlus的封装方法，播放指定名称的状态机
        /// </summary>
        /// <param name="layer">状态机所在层级</param>
        /// <param name="stateName">状态机名称</param>
        /// <param name="realTime">真实播放时间</param>
        public void Play(int layer, string stateName, float realTime)
        {
            if (!_actionTimes.ContainsKey(stateName))
            {
                $"Play animator failed. {name} can not find state name - {stateName}.".Exception();
                return;
            }

            ForceUpdateStateSpeed(stateName, realTime);
            Play(stateName, layer);
        }
    }
}