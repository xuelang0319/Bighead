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

using BigHead.Customer;
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
                EditorGUIUtility.IconContent("AudioMixerSnapshot Icon"),
                CommandStyle.Style))
            {
            }

            EditorGUIUtility.SetIconSize(new Vector2(18, 18));
            if (GUILayout.Button(
                EditorGUIUtility.IconContent("Collab.BuildSucceeded"),
                CommandStyle.Style))
            {
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