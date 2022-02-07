//
// = The script is part of Bighead and the framework is individually owned by Eric Lee.
// = Cannot be commercially used without the authorization.
//
//  Author  |  UpdateTime     |   Desc  
//  Eric    |  2021年10月22日  |   物理引擎封装助手
//

using System.Collections.Generic;
using UnityEngine;

public static class PhysicsHelper
{
    /// <summary>
    /// 将囊体移动碰撞后停止位置预检测
    /// </summary>
    /// <param name="startPoint">射线起点</param>
    /// <param name="targetPoint">射线终点</param>
    /// <param name="capsuleHeight">胶囊体高度</param>
    /// <param name="capsuleRadius">胶囊体半径</param>
    /// <param name="castLayer">检测层级，这里的层级序号需要使用位移后获得，Unity支持多层级位移后相加检测。</param>
    /// <param name="hits">路径上碰撞的所有点，该返回值无需排序是按碰撞顺序添加，可以通过该参数数量判断是否发生碰撞，及发生几次碰撞。</param>
    /// <param name="hitStop">碰撞后是否停止，如果不停止将计算与碰撞体的法线距离，模拟人物蹭墙走时的偏移。</param>
    /// <param name="faultTolerantDistance">在Unity内如果紧邻碰撞体则检测不到该碰撞体而导致检测失败，这里默认向后多添加一点距离从而可以检测紧邻的碰撞物，当然也可以当做容错距离使用。</param>
    /// <returns>物体最终移动的位置，此位置非碰撞位置。</returns>
    public static Vector3 CapsuleMoveCast(Vector3 startPoint, Vector3 targetPoint, float capsuleHeight,
        float capsuleRadius, int castLayer, List<RaycastHit> hits, bool hitStop = false,
        float faultTolerantDistance = 0.01f)
    {
        var y = startPoint.y;
        var expectTranslate = targetPoint - startPoint;
        var direction = expectTranslate.normalized;
        var distance = expectTranslate.magnitude;

        var halfHeightOffset = Vector3.up * capsuleHeight / 2;
        var pointOffset = direction * faultTolerantDistance;
        var point1 = startPoint + halfHeightOffset - pointOffset;
        var point2 = startPoint - halfHeightOffset - pointOffset;

        if (Physics.CapsuleCast(point1, point2, capsuleRadius, direction, out var hit, distance + faultTolerantDistance,
            castLayer))
        {
            if (hits == null) hits = new List<RaycastHit>();
            hits.Add(hit);

            var point = hit.point;
            point.y = y;

            // 碰撞后圆心的回退距离
            const float missDistance = 0.005f;

            // 碰撞点计算圆心所在位置
            var normal = hit.normal;
            var centerPosition = point + normal * capsuleRadius;

            // 回退后最终停止位置
            var stopPosition = centerPosition - direction * missDistance;

            // 如果仅计算停止位置，则直接返回该碰撞后的物体位置。
            if (hitStop) return stopPosition;

            // 计算碰撞切线朝向
            var shadow = Vector3.Project(-expectTranslate, normal);
            var tangentNormal = (shadow + expectTranslate).normalized;

            // 计算剩余向量在切线上的投影从而计算出切线移动的剩余距离
            var surplus = expectTranslate - stopPosition + startPoint;
            var offset = Vector3.Project(surplus, tangentNormal);

            // 递归计算切线移动时方向上是否存在碰撞体及位置
            targetPoint = CapsuleMoveCast(stopPosition, stopPosition + offset, capsuleHeight, capsuleRadius, castLayer,
                hits);
        }

        targetPoint.y = y;
        return targetPoint;
    }
}