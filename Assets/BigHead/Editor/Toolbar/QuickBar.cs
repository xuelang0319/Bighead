// -------------------------------------------------------------------------------------
//
// = The script is part of BigHead and the framework is individually owned by Eric Lee.
// = Cannot be commercially used without the authorization.
//
// = Author： Eric
// = Time： 2020年3月3日
// = Email：110925095@qq.com
// = Desc：
//
// -------------------------------------------------------------------------------------

using BigHead.Editor.Customer;
using BigHead.Editor.Generate.GenCsv;
using BigHead.Editor.Generate.GenTmx;
using UnityEditor;
using UnityEngine;

namespace BigHead.Editor.Toolbar
{
    [InitializeOnLoad]
    public class QuickBar
    {
        static QuickBar()
        {
            ViewLayout.QuickArea.Add(OnGUI);
        }

        static void OnGUI()
        {
            EditorGUIUtility.SetIconSize(new Vector2(18, 18));
            if (GUILayout.Button(
                EditorGUIUtility.IconContent("lightMeter/redLight"),
                CommandStyle.Style))
            {
                TestOnEditor.CustomFoo();
            }

            EditorGUIUtility.SetIconSize(new Vector2(18, 18));
            if (GUILayout.Button(
                EditorGUIUtility.IconContent("MonoLogo"),
                CommandStyle.Style))
            {
                Excel2Csv.Generate();
            }

            EditorGUIUtility.SetIconSize(new Vector2(18, 18));
            if (GUILayout.Button(
                EditorGUIUtility.IconContent("Collab.BuildSucceeded"),
                CommandStyle.Style))
            {
                Tmx2Txt.Generate();
            }


            EditorGUIUtility.SetIconSize(new Vector2(18, 18));
            if (GUILayout.Button(
                EditorGUIUtility.IconContent("GameObject Icon"),
                CommandStyle.Style))
            {

            }
        }
    }
}