//
// = The script is part of Bighead and the framework is individually owned by Eric Lee.
// = Cannot be commercially used without the authorization.
//
//  Author  |  UpdateTime     |   Desc  
//  Eric    |  2021年12月02日  |   Task延时调用
//

using System;

public class TaskDelay : TaskBase
{
    private float _delayTime;
    private readonly Action _callback;

    public TaskDelay(float delayTime, Action callback = null)
    {
        _delayTime = delayTime;
        _callback = callback;
    }

    public override TaskState Update(float deltaTime)
    {
        _delayTime -= deltaTime;
        if (_delayTime > 0)
            return TaskState.Running;

        _callback?.Invoke();
        return TaskState.Success;
    }
}