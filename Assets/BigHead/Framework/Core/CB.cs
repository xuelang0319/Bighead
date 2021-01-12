//
// = The script is part of BigHead and the framework is individually owned by Eric Lee.
// = Cannot be commercially used without the authorization.
//
//  Author  |  UpdateTime     |   Desc  
//  Eric    |  2020年12月17日  |   delegate处理回调的基础模板
//

namespace BigHead.Framework.Core
{
    public delegate void Callback();

    public delegate void Callback<in T>(T param);

    public delegate void Callback<in T1, in T2>(T1 param1, T2 param2);

    public delegate void Callback<in T1, in T2, in T3>(T1 param1, T2 param2, T3 param3);

    public delegate TResult Recall<out TResult>();

    public delegate TResult Recall<out TResult, in T>(T param);

    public delegate TResult Recall<out TResult, in T1, in T2>(T1 param1, T2 param2);

    public delegate TResult Recall<out TResult, in T1, in T2, in T3>(T1 param1, T2 param2, T3 param3);

}