//
// = The script is part of Bighead and the framework is individually owned by Eric Lee.
// = Cannot be commercially used without the authorization.
//
//  Author  |  UpdateTime     |   Desc  
//  Eric    |  2021年9月1日    |   新增计算助手。
//  Eric    |  2021年9月1日    |   添加获取两个物体的夹角
//

using UnityEngine;

public static class GameMathf
{
    /// <summary>
    /// 使用角度和半径获取相对于原点的坐标位置.
    /// 仅限于3D同水平计算.
    /// </summary>
    /// <param name="angle">角度</param>
    /// <param name="radius">半径</param>
    /// <returns>返回相对于原点的坐标</returns>
    public static Vector3 GetLocalPositionByAngleAndRadius_3D(float angle, float radius)
    {
        const float param = 3.14f / 180;
        var x = radius * Mathf.Cos(angle * param);
        var z = radius * Mathf.Sin(angle * param);
        return new Vector3(x, 0, z);
    }

    /// <summary>
    /// 使用角度和半径获取相对于原点的坐标位置.
    /// 仅限于2D平面计算.
    /// </summary>
    /// <param name="angle">角度</param>
    /// <param name="radius">半径</param>
    /// <returns>返回相对于原点的坐标</returns>
    public static Vector2 GetLocalPositionByAngleAndRadius_2D(float angle, float radius)
    {
        const float param = 3.14f / 180;
        var x = radius * Mathf.Cos(angle * param);
        var y = radius * Mathf.Sin(angle * param);
        return new Vector2(x, y);
    }

    /// <summary>
    /// 获取角度，使用X,Z计算
    /// </summary>
    /// <param name="origin">原点坐标</param>
    /// <param name="target">目标点坐标</param>
    /// <param name="range360">是否为360度，True为0-360，False为-180到180</param>
    /// <returns>返回角度</returns>
    public static float GetAngle3D(Vector3 origin, Vector3 target, bool range360)
    {
        return GetAngle(origin.x, origin.z, target.x, target.z, range360);
    }

    /// <summary>
    /// 获取角度，使用X,Y计算
    /// </summary>
    /// <param name="origin">原点坐标</param>
    /// <param name="target">目标点坐标</param>
    /// <param name="range360">是否为360度，True为0-360，False为-180到180</param>
    /// <returns>返回角度</returns>
    public static float GetAngle2D(Vector3 origin, Vector3 target, bool range360)
    {
        return GetAngle(origin.x, origin.y, target.x, target.y, range360);
    }

    /// <summary>
    /// 获取角度
    /// </summary>
    /// <param name="x1">原点X值</param>
    /// <param name="y1">原点Y值，如果为3D平面则为Z值</param>
    /// <param name="x2">目标点X值</param>
    /// <param name="y2">目标点Y值，如果为3D平面则为Z值</param>
    /// <param name="range360">是否为360度，True为0-360，False为-180到180</param>
    /// <returns>返回角度</returns>
    public static float GetAngle(float x1, float y1, float x2, float y2, bool range360)
    {
        var x = x2 - x1;
        var z = y2 - y1;

        float deltaAngle = 0;
        if (x == 0 && z == 0)
            return 0;

        if (x > 0 && z > 0)
        {
            deltaAngle = 0;
        }
        else if (x > 0 && z == 0)
        {
            return 90;
        }
        else if (x > 0 && z < 0)
        {
            deltaAngle = 180;
        }
        else if (x == 0 && z < 0)
        {
            return 180;
        }
        else if (x < 0 && z < 0)
        {
            deltaAngle = -180;
        }
        else if (x < 0 && z == 0)
        {
            return -90;
        }
        else if (x < 0 && z > 0)
        {
            deltaAngle = 0;
        }

        float angle = Mathf.Atan(x / z) * Mathf.Rad2Deg + deltaAngle;

        if (range360 && angle < 0) angle += 360;
        return angle;
    }
}