//
// = The script is part of Bighead and the framework is individually owned by Eric Lee.
// = Cannot be commercially used without the authorization.
//
//  Author  |  UpdateTime     |   Desc  
//  Eric    |  2021年12月02日  |   Task条件执行
//

using System;

public class TaskConditional : TaskBase
{
    private float? _maxTime;
    private readonly Func<bool> _condition;

    public TaskConditional(Func<bool> condition, float? maxTime = null)
    {
        if (condition == null)
            "Task conditional action is null.".Error();

        _condition = condition;
        _maxTime = maxTime;
    }

    public override TaskState Update(float deltaTime)
    {
        if (_condition?.Invoke() ?? true)
            return TaskState.Success;

        if (_maxTime == null)
            return TaskState.Running;

        _maxTime -= deltaTime;
        return _maxTime <= 0
            ? TaskState.Success
            : TaskState.Running;
    }
}