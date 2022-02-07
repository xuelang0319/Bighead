//
// = The script is part of BigHead and the framework is individually owned by Eric Lee.
// = Cannot be commercially used without the authorization.
//
//  Author  |  UpdateTime     |   Desc  
//  Eric    |  2021年08月17日  |   添加Hierarchy右键功能 （也会显示在MenuItem中的GameObject选项中）
//

using BigHead.Editor.Generate.GenTransformScript;
using BigHead.Editor.Generate.GenTransformScript.Param;
using UnityEditor;

namespace BigHead.Editor
{
    public static class MenuContext
    {
        [MenuItem("GameObject/GenerateSelect/MonoScript", false, 0)]
        public static void GenerateSelectMonoScript()
        {
            GenTransformScript.AnalysisSelectPrefab(new GenSelectMonoParam());
        }
        
        [MenuItem("GameObject/GenerateSelect/PanelScript", false, 0)]
        public static void GenerateSelectPanelScript()
        {
            GenTransformScript.AnalysisSelectPrefab(new GenSelectUIParam());
        }
    }
}