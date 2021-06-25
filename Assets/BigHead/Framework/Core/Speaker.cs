//
// = The script is part of BigHead and the framework is individually owned by Eric Lee.
// = Cannot be commercially used without the authorization.
//
//  Author  |  UpdateTime     |   Desc  
//  Eric    |  2021年2月28日   |   Log封装类
//  Eric    |  2021年6月25日   |   使用编译指令优化Log封装类
//

#define BH_DEBUG
using System;
using UnityEngine;

namespace BigHead.Framework.Core
{
    public static class Speaker
    {
        public static void Log(this object message)
        {
#if BH_DEBUG
            Debug.Log($"[>>] {message}");  
#endif
        }

        public static void Warning(this object message)
        {
#if BH_DEBUG
            Debug.LogWarning($"<color=yellow>[>>] {message}</color>");
#endif
        }

        public static void Error(this object message)
        {
#if BH_DEBUG
            Debug.LogError($"<color=red>[>>] {message}</color>");
#endif
        }

        public static void Highlight(this object message)
        {
#if BH_DEBUG
            Debug.LogError($"<color=green>[>>] {message}</color>");
#endif
        }

        public static void Exception(this string exception)
        {
#if BH_DEBUG
            Debug.LogError($"<color=red>[Exception] {exception}</color>");
#endif
        }

        public static void Exception(this Exception exception)
        {
#if BH_DEBUG 
            Debug.LogError($"<color=red>[Exception] {exception.Message}</color>");
#endif
        }
    }
}