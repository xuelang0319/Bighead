//
// = The script is part of Bighead and the framework is individually owned by Eric Lee.
// = Cannot be commercially used without the authorization.
//
//  Author  |  UpdateTime     |   Desc  
//  Eric    |  2021年12月02日  |   Task队列执行
//

using System.Collections.Generic;

public class TaskQueue : TaskBase
{
    private readonly Queue<TaskBase> _queue = new Queue<TaskBase>();

    public void Add(TaskBase task)
    {
        _queue.Enqueue(task);
    }

    public override TaskState Update(float deltaTime)
    {
        var state = _queue.Peek().Update(deltaTime);
        switch (state)
        {
            case TaskState.Running:
                return TaskState.Running;
            case TaskState.Success:
                _queue.Dequeue();
                return _queue.Count > 0 ? TaskState.Running : TaskState.Success;
            case TaskState.Break:
                return TaskState.Success;
            default:
                $"Task state {state} error.".Error();
                return TaskState.Success;
        }
    }
}