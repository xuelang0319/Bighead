//
// = The script is part of Bighead and the framework is individually owned by Eric Lee.
// = Cannot be commercially used without the authorization.
//
//  Author  |  UpdateTime     |   Desc  
//  Eric    |  2021年12月02日  |   Task基类
//

public enum TaskState
{
    /// 运行中
    Running,

    /// 已完成
    Success,

    /// 中断 - 用于队列进行的处理，当返回中断时，队列中断。
    Break
}

public abstract class TaskBase
{
    public abstract TaskState Update(float deltaTime);
}