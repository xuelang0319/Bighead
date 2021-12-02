//
// = The script is part of Bighead and the framework is individually owned by Eric Lee.
// = Cannot be commercially used without the authorization.
//
//  Author  |  UpdateTime     |   Desc  
//  Eric    |  2021年12月02日  |   Task延时调用
//

using System.Collections.Generic;
using UnityEngine;

namespace BigHead.Framework.Game.Tasks
{
    public class TaskManager : MonoGlobalSingleton<TaskManager>
    {
        /// <summary> 组名 - 任务组 </summary>
        private readonly Dictionary<string, HashSet<TaskBase>> _groupName2Tasks = new Dictionary<string, HashSet<TaskBase>>();
        /// <summary> 任务 - 组名 </summary>
        private readonly Dictionary<TaskBase, string> _task2GroupName = new Dictionary<TaskBase, string>();
        /// <summary> 运行中的任务 </summary>
        private readonly List<TaskBase> _runningTask = new List<TaskBase>();
        /// <summary> 等待加入任务组 </summary>
        private readonly Queue<TaskBase> _waitAddQueue = new Queue<TaskBase>();
        /// <summary> 等待移除任务组 </summary>
        private readonly Queue<TaskBase> _waitRemoveQueue = new Queue<TaskBase>();

        /// <summary>
        /// 托管Task
        /// </summary>
        /// <param name="groupName">组名</param>
        /// <param name="task">任务</param>
        public void Add(string groupName, TaskBase task)
        {
            if (!_groupName2Tasks.ContainsKey(groupName))
                _groupName2Tasks[groupName] = new HashSet<TaskBase>();
        
            _groupName2Tasks[groupName].Add(task);
            _task2GroupName[task] = groupName;
            _waitAddQueue.Enqueue(task);
        }

        /// <summary>
        /// 移除Task
        /// </summary>
        /// <param name="task">任务</param>
        public void Remove(TaskBase task)
        {
            if (!_task2GroupName.TryGetValue(task, out var groupName)) 
                return;

            _waitRemoveQueue.Enqueue(task);
            
            var group = _groupName2Tasks[groupName];
            group.Remove(task);

            if (group.Count > 0) return;
            _groupName2Tasks.Remove(groupName);
        }

        public void Update()
        {
            while (_waitAddQueue.Count > 0)
            {
                _runningTask.Add(_waitAddQueue.Dequeue());
            }

            while (_waitRemoveQueue.Count > 0)
            {
                _runningTask.Remove(_waitRemoveQueue.Dequeue());
            }

            for (int i = 0; i < _runningTask.Count; i++)
            {
                var state = _runningTask[i].Update(Time.deltaTime);
                if (state == TaskState.Success) Remove(_runningTask[i]);
            }
        }
    }
}