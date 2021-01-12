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

using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace BigHead.Editor.Toolbar
{
    public class ViewLayout
    {
        public static readonly List<Action> QuickArea = new List<Action>();

        static int m_toolCount;
        
        static ViewLayout()
        {
            Type toolbarType = typeof(UnityEditor.Editor).Assembly.GetType("UnityEditor.Toolbar");
            FieldInfo toolIcons = toolbarType.GetField("s_ShownToolIcons",
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
            var array = (Array) toolIcons.GetValue(null);
            m_toolCount = array != null ? array.Length : 6;

            MainLayout.MainAction -= OnGUI;
            MainLayout.MainAction += OnGUI;
        }

        private static GUIStyle style;
        
        static void OnGUI()
        {
            var screenWidth = EditorGUIUtility.currentViewWidth;

            // Following calculations match code reflected from Toolbar.OldOnGUI()
            float playButtonsPosition = (screenWidth - 100) / 2;

            Rect rightRect = new Rect(0, 0, screenWidth, Screen.height);
            rightRect.xMin = playButtonsPosition;
            rightRect.xMin += CommandStyle.Style.fixedWidth * 5; // Play buttons
            rightRect.xMax = screenWidth;
            rightRect.xMax -= 10; // Spacing right
            rightRect.xMax -= 80; // Layout
            rightRect.xMax -= 10; // Spacing between layout and layers
            rightRect.xMax -= 80; // Layers
            rightRect.xMax -= 20; // Spacing between layers and account
            rightRect.xMax -= 80; // Account
            rightRect.xMax -= 10; // Spacing between account and cloud
            rightRect.xMax -= 32; // Cloud
            rightRect.xMax -= 10; // Spacing between cloud and collab
            rightRect.xMax -= 78; // Colab
            
            rightRect.xMin += 10;
            rightRect.xMax -= 10;
            rightRect.y = 5;
            rightRect.height = 24;
            
            if (rightRect.width > 0)
            {
                GUILayout.BeginArea(rightRect);
                GUILayout.BeginHorizontal();
                foreach (var handler in QuickArea)
                {
                    handler();
                }

                GUILayout.EndHorizontal();
                GUILayout.EndArea();
            }
        }
    }
}