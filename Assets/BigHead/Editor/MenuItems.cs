//
// = The script is part of BigHead and the framework is individually owned by Eric Lee.
// = Cannot be commercially used without the authorization.
//
//  Author  |  UpdateTime     |   Desc  
//  Eric    |  2020年12月18日  |   编辑器菜单扩展项
//

using BigHead.Editor.Generate.GenCsv;
using UnityEditor;

namespace BigHead.Editor
{
    public static class MenuItems
    {
        [MenuItem("BigHead/Test")]
        public static void Test()
        {
            
        }
        
        [MenuItem("BigHead/Csv/Clear")]
        public static void ClearGenCsv()
        {
            Excel2Csv.ClearAll();
        }
    }
}