//
// = The script is part of BigHead and the framework is individually owned by Eric Lee.
// = Cannot be commercially used without the authorization.
//
//  Author  |  UpdateTime     |   Desc  
//  Eric    |  2020年12月17日  |   打印扩展方法
//

using System;
using UnityEngine;

namespace BigHead.Framework.Extension
{
    public static class Log
    {
        public static void Print(this string str)
        {
            Debug.Log($"[>>] {str}");
        }
        
        public static void Warning(this string str)
        {
            Debug.LogWarning($"<color=yellow>[>>] {str}</color>");
        }
        
        public static void Error(this string str)
        {
            Debug.LogError($"<color=red>[>>] {str}</color>");
        }
        
        public static void Highlight(this string str)
        {
            Debug.Log($"<color=green>[>>] {str}</color>");
        }

        public static void Exception(this Exception e)
        {
            Debug.LogError($"<color=red>[Exception] {e.Message}</color>");
        }

        public static void Exception(this string str)
        {
            Exception(new Exception(str));
        }
    }
}