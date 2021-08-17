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