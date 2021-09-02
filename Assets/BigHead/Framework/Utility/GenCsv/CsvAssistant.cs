//
// = The script is part of BigHead and the framework is individually owned by Eric Lee.
// = Cannot be commercially used without the authorization.
//
//  Author  |  UpdateTime     |   Desc  
//  Eric    |  2021年2月27日   |   Csv调用入口
//


using System;
using System.Collections.Generic;
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
        if (array.Count == 0)
        {
            "没有检测到任何Csv配置表，请检查。".Exception();
            callback?.Invoke();
        }

        var plusArray = array.Where(csv => csv.GetType().Name.EndsWith("Plus")).ToArray();
        var dictionary = new Dictionary<string, BasicCsv>();
        for (int i = 0; i < plusArray.Length; i++)
        {
            var item = plusArray[i];
            var name = item.GetType().Name;
            name = name.Substring(0, name.Length - 4);
            dictionary.Add(name, item);
            array.Remove(item);
        }

        for (var i = 0; i < array.Count; i++)
        {
            var item = array[i];
            var name = item.GetType().Name;
            if (dictionary.ContainsKey(name)) continue;
            dictionary.Add(name, item);
        }
        
        var waitCount = dictionary.Count;
        foreach (var kv in dictionary)
        {
            CsvFunctions.RegisterCsv(kv.Key, kv.Value);
            kv.Value.InitCsv(() =>
            {
                --waitCount;
                if(Equals(waitCount, 0)) 
                    callback?.Invoke();
            });
        }
    }
}