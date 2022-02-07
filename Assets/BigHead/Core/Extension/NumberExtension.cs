//
// = The script is part of BigHead and the framework is individually owned by Eric Lee.
// = Cannot be commercially used without the authorization.
//
//  Author  |  UpdateTime     |   Desc  
//  Eric    |  2021年11月16日  |   数字类型扩展方法
//

using UnityEngine;

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

    /// <summary>
    /// 获得同一水平随机位置
    /// </summary>
    /// <param name="v3">当前位置</param>
    /// <param name="radius">随机半径</param>
    /// <returns>当前位置为原点半径内随机坐标</returns>
    public static Vector3 GetHorizontalRandomPos(this Vector3 v3, float radius)
    {
        var randomPos = Random.insideUnitCircle * radius;
        return new Vector3(v3.x + randomPos.x, v3.y, v3.z + randomPos.y);
    }
}
