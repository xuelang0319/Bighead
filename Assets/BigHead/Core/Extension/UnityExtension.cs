//
// = The script is part of BigHead and the framework is individually owned by Eric Lee.
// = Cannot be commercially used without the authorization.
//
//  Author  |  UpdateTime     |   Desc  
//  Eric    |  2021年10月19日  |   Unity原生组件拓展方法
//

using UnityEngine;

public static class UnityExtension
{
    /// <summary>
    /// 重置位移组件属性
    /// </summary>
    /// <param name="transform">被重置位移组件</param>
    public static void Reset(this Transform transform)
    {
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        transform.localScale = Vector3.one;
    }

    /// <summary>
    /// 设置位移组件父物体，并重置位移组件属性
    /// </summary>
    /// <param name="transform">被重置位移组件</param>
    /// <param name="parent">位移组件父物体</param>
    public static void SetParentReset(this Transform transform, Transform parent)
    {
        transform.SetParent(parent);
        Reset(transform);
    }
}