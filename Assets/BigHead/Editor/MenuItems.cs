//
// = The script is part of BigHead and the framework is individually owned by Eric Lee.
// = Cannot be commercially used without the authorization.
//
//  Author  |  UpdateTime     |   Desc  
//  Eric    |  2020年12月18日  |   编辑器菜单扩展项
//

using BigHead.Editor.ArtTools;
using BigHead.Editor.Generate.GenCsv;
using UnityEditor;
using UnityEngine;

namespace BigHead.Editor
{
    /*
     * 添加快捷键：
     * 单一建: _
     * Shift: #
     * Ctrl:  %
     * Alt:   &
     * 如: _t， #t,  %t, &t
     * 可以添加如下快捷键： [MenuItem("BigHead/Test %t")], 即按Ctrl + t就可以直接触发该方法
     */
    
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
        
        [MenuItem("BigHead/Csv/Generate")]
        public static void GenCsv()
        {
            Excel2Csv.Generate();
        }

        [MenuItem("BigHead/ArtTools/GetSelectionVertexes")]
        public static void GetNumberOfVertexes()
        {
            ModelTools.GetSelectionModelVertexesAndTriangularFacet();
        }
    }
}