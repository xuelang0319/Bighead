//
// = The script is part of BigHead and the framework is individually owned by Eric Lee.
// = Cannot be commercially used without the authorization.
//
//  Author  |  UpdateTime     |   Desc  
//  Eric    |  2021年11月16日  |   数字类型扩展方法
//

using UnityEngine;

namespace BigHead.Framework.Extension
{
    public static class NumberExtension
    {
        /// <summary>
        /// 保留小数位
        /// </summary>
        /// <param name="value">当前值</param>
        /// <param name="keepCount">保留小数位，默认为2</param>
        /// <returns>返回保留后的浮点类型</returns>
        public static float KeepDecimals(this float value, int keepCount = 2)
        {
            var temporary = Mathf.Pow(10, keepCount);
            return (int) (value * temporary) / temporary;
        }
    }
}