//
// = The script is part of BigHead and the framework is individually owned by Eric Lee.
// = Cannot be commercially used without the authorization.
//
//  Author  |  UpdateTime     |   Desc  
//  Eric    |  2021年2月28日   |   类型转换器
//

using System.Collections.Generic;
using BigHead.Framework.Core;

namespace BigHead.Framework.Utility.GenCsv
{
    public static partial class CsvType
    {
        private static Dictionary<string, string> _csvTypes = new Dictionary<string, string>()
        {
            
        };

        private static Dictionary<string, string> _csvTransFoo = new Dictionary<string, string>()
        {

        };
        
        public static string ToNil(string value)
        {
            $"Csv读取类型错误，请检查。  --value: {value}".Error();
            return value;
        }

        public static int ToInt(string value) =>
            int.Parse(value);

        public static float ToFloat(string value) =>
            float.Parse(value);

        public static bool ToBool(string value) =>
            bool.Parse(value);
        
    }
}