//
// = The script is part of BigHead and the framework is individually owned by Eric Lee.
// = Cannot be commercially used without the authorization.
//
//  Author  |  UpdateTime     |   Desc  
//  Eric    |  2021年2月28日   |   Log封装类
//

using System;
using BigHead.Customer;
using UnityEngine;

namespace BigHead.Framework.Core
{
    public static class Speaker
    {
        public static void Log(this object message)
        {
            if(BigheadConfig.Debug)
                Debug.Log($"[>>] {message}");
        }

        public static void Warning(this object message)
        {
            if(BigheadConfig.Debug) 
                Debug.LogWarning($"<color=yellow>[>>] {message}</color>");
        }

        public static void Error(this object message)
        {
            if(BigheadConfig.Debug)
                Debug.LogError($"<color=red>[>>] {message}</color>");
        }

        public static void Highlight(this object message)
        {
            if(BigheadConfig.Debug)
                Debug.LogError($"<color=green>[>>] {message}</color>");
        }

        public static void Exception(this string exception)
        {
            if(BigheadConfig.Debug) 
                Debug.LogError($"<color=red>[Exception] {exception}</color>");
        }

        public static void Exception(this Exception exception)
        {
            if(BigheadConfig.Debug) 
                Debug.LogError($"<color=red>[Exception] {exception.Message}</color>");
        }
    }
}