//
// = The script is part of Bighead and the framework is individually owned by Eric Lee.
// = Cannot be commercially used without the authorization.
//
//  Author  |  UpdateTime     |   Desc  
//  Eric    |  2021年12月02日  |   Task并行
//

using System.Collections.Generic;

public class TaskParallel : TaskBase
{
    private readonly Queue<TaskBase> _waitAdd = new Queue<TaskBase>();
    private readonly Queue<TaskBase> _waitRemove = new Queue<TaskBase>();
    private readonly List<TaskBase> _list = new List<TaskBase>();

    public int RunningCount { get; private set; } = 0;

    public void AddTask(TaskBase taskBase)
    {
        ++RunningCount;
        _waitAdd.Enqueue(taskBase);
    }

    public void AddTask(IEnumerable<TaskBase> taskBases)
    {
        foreach (var taskBase in taskBases)
        {
            ++RunningCount;
            _waitAdd.Enqueue(taskBase);
        }
    }

    public override TaskState Update(float deltaTime)
    {
        while (_waitAdd.Count > 0)
        {
            _list.Add(_waitAdd.Dequeue());
        }

        for (int i = 0; i < _list.Count; i++)
        {
            var state = _list[i].Update(deltaTime);

            if (state == TaskState.Break)
                return TaskState.Break;

            if (state == TaskState.Success)
                _waitRemove.Enqueue(_list[i]);
        }

        while (_waitRemove.Count > 0)
        {
            --RunningCount;
            _list.Remove(_waitRemove.Dequeue());
        }

        return _list.Count == 0 ? TaskState.Success : TaskState.Running;
    }
}