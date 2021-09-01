//
// = The script is part of BigHead and the framework is individually owned by Eric Lee.
// = Cannot be commercially used without the authorization.
//
//  Author  |  UpdateTime     |   Desc  
//  Eric    |  2021年5月29日   |   Csv函数
//

using System.Collections.Generic;

public static class CsvFunctions
{
    private static Dictionary<string, BasicCsv> _csvBasics = new Dictionary<string, BasicCsv>();

    public static T GetCsv<T>(string name) where T: BasicCsv, new()
    {
        _csvBasics.TryGetValue(name, out var value);
        if (Equals(null, value))
        {
            value = new T();
            _csvBasics.Add(name, value);
        }

        return value as T;
    }

    public static void RegisterCsv(string key, BasicCsv basicCsv)
    {
        bool isPlus = false;
        if (key.EndsWith("Plus"))
        {
            key = key.Substring(0, key.Length - 4);
            isPlus = true;
        }

        if (_csvBasics.ContainsKey(key) && !isPlus)
            return;
        
        _csvBasics[key] = basicCsv;
    }
}