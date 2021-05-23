//
// = The script is part of BigHead and the framework is individually owned by Eric Lee.
// = Cannot be commercially used without the authorization.
//
//  Author  |  UpdateTime     |   Desc  
//  Eric    |  2021年2月27日   |   CsvTable基类
//

using System.Collections.Generic;
using System.Linq;

namespace BigHead.Framework.Utility.GenCsv
{
    public interface ICsvTable
    {
        void AnalysisTable(List<string> rows);
    }

    public abstract class CsvTable<T> : ICsvTable
    {
        protected Dictionary<string, T> TableRows = new Dictionary<string, T>();

        public abstract void AnalysisTable(List<string> rows);

        public T GetRowByUniKey(string key)
        {
            TableRows.TryGetValue(key, out var row);
            return row;
        }

        public T GetRowByUniKey(int key) => 
            GetRowByUniKey(key.ToString());

        public List<T> GetTable() =>
            TableRows.Values.ToList();
    }
}