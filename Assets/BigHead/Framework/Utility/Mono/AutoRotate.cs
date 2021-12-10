//
// = The script is part of Bighead and the framework is individually owned by Eric Lee.
// = Cannot be commercially used without the authorization.
//
//  Author  |  UpdateTime     |   Desc  
//  Eric    |  2021年12月10日  |   自动旋转组件
//

using UnityEngine;

public class AutoRotate : MonoBehaviour
{
    [Tooltip("旋转速率")]
    public Vector3 Rotate;
    [Tooltip("无视时间暂停")]
    public bool IgnoreTimeScale;

    public void Update()
    {
        transform.Rotate(Rotate * (IgnoreTimeScale ? Time.deltaTime : 0.02f));
    }
}