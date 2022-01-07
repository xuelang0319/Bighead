//
// = The script is part of BigHead and the framework is individually owned by Eric Lee.
// = Cannot be commercially used without the authorization.
//
//  Author  |  UpdateTime     |   Desc  
//  Eric    |  2021年5月29日   |   Csv基类
//

using System;
using System.Collections.Generic;
using BigHead.Framework.Utility.Readers;

public abstract class BasicCsv
{
    protected abstract void AnalysisCsv(List<string> list, Action callback);
    protected virtual string Path { get; set; }

    public void InitCsv(Action action)
    {
        CsvReader.ReadCsv(Path, s =>
        {
            var list = CsvReader.ToListWithDeleteFirstLines(s, 3);
            AnalysisCsv(list, action);
        });
    }
}