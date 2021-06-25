//
// = The script is part of BigHead and the framework is individually owned by Eric Lee.
// = Cannot be commercially used without the authorization.
//
//  Author  |  UpdateTime     |   Desc  
//  Eric    |  2021年2月27日   |   Csv调用入口
//


using System;
using System.Linq;
using BigHead.Framework.Core;
using BigHead.Framework.Utility.Helper;
using UnityEngine;

public static partial class CsvAssistant
{
    public static T AsPlus<T>(this BasicCsv t) where T: BasicCsv => (T) t;

    public static void Step(Action callback)
    {
        var array = typeof(BasicCsv).CreateAllDerivedClass<BasicCsv>().ToList();
        var waitCount = array.Count;
        if (waitCount == 0)
        {
            "没有检测到任何Csv配置表，请检查。".Exception();
            callback?.Invoke();
        }

        foreach (var csv in array)
        {
            CsvFunctions.RegisterCsv(csv.GetType().Name, csv);
            csv.InitCsv(() =>
            {
                --waitCount;
                if(Equals(waitCount, 0)) 
                    callback?.Invoke();
            });
        }
    }
}