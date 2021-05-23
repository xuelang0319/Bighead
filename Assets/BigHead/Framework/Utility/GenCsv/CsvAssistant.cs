//
// = The script is part of BigHead and the framework is individually owned by Eric Lee.
// = Cannot be commercially used without the authorization.
//
//  Author  |  UpdateTime     |   Desc  
//  Eric    |  2021年2月27日   |   Csv调用入口
//

using System.Collections.Generic;

namespace BigHead.Framework.Utility.GenCsv
{
    public static class CsvAssistant
    {
        private static Dictionary<string, ICsvTable> _csvTables = new Dictionary<string, ICsvTable>();

        private static ICsvTable _GetTable(string tableName)
        {
            _csvTables.TryGetValue(tableName, out var table);
            return table;
        }
    }
}