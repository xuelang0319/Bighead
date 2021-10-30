//
// = The script is part of BigHead and the framework is individually owned by Eric Lee.
// = Cannot be commercially used without the authorization.
//
//  Author  |  UpdateTime     |   Desc  
//  Eric    |  2021年10月30日  |   修改选中模型的子物体的名称
//

using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace BigHead.Editor.Scripts
{
    public class ChangeChildrenName
    {
        /// <summary>
        /// 批量修改选中物体指定子物体名字
        /// </summary>
        /// <param name="mappingKey">
        /// 映射字典 -
        /// key : 原名称包含的部分字段  
        /// value: 补充文字 </param>
        /// <param name="handle">
        /// 名称处理器 -
        /// 第一个参数为 ： 物体原名称
        /// 第二个参数为 ： 传入字典的补充文字
        /// 返回数据为 ： 物体最终的名称
        /// </param>
        private static void ChangeSelectObjectChildrenName(Dictionary<string, string> mappingKey,
            Func<string, string, string> handle)
        {
            var selections = Selection.gameObjects;
            var objectCount = selections.Length;
            if (objectCount == 0) return;
        
            for (int i = 0; i < objectCount; i++)
            {
                var selectObject = selections[i];
                foreach (Transform child in selectObject.transform)
                {
                    var name = child.name;
                    foreach (var kv in mappingKey)
                    {
                        if (!name.Contains(kv.Key)) continue;
                        child.name = handle?.Invoke(name, kv.Value) ?? name;
                        break;
                    }
                }
            }
        }
    }
}