//
// = The script is part of BigHead and the framework is individually owned by Eric Lee.
// = Cannot be commercially used without the authorization.
//
//  Author  |  UpdateTime     |   Desc  
//  Eric    |  2020年12月18日  |   编辑器菜单扩展项
//

using BigHead.Csv.Core.Editor;
using BigHead.Csv.Curve.Editor;
using BigHead.Editor.Scripts;
using BigHead.Tmx.Editor;
using UnityEditor;

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
        

        [MenuItem("GameObject/BigHead/GetSelectionVertexes", false, 0)]
        public static void GetNumberOfVertexes()
        {
            ModelTools.GetSelectionModelVertexesAndTriangularFacet();
        }
    }
}