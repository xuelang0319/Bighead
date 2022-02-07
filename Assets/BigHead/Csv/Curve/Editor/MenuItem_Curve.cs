﻿//
// = The script is part of BigHead and the framework is individually owned by Eric Lee.
// = Cannot be commercially used without the authorization.
//
//  Author  |  UpdateTime     |   Desc  
//  Eric    |  2022年2月5日    |   曲线编辑器菜单扩展项
//

using BigHead.Csv.Curve.Editor;
using UnityEditor;

public class MenuItem_Curve
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
    
    [MenuItem("BigHead/Window/CurveEditor")]
    public static void OpenCurveWindow()
    {
        CurveWindow.OpenWindow();
    }
}