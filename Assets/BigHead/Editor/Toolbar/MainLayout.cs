using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace BigHead.Editor.Toolbar
{
    public class MainLayout
    {
        static Type _toolbarType = typeof(UnityEditor.Editor).Assembly.GetType("UnityEditor.Toolbar");
        static Type _guiViewType = typeof(UnityEditor.Editor).Assembly.GetType("UnityEditor.GUIView");

        static PropertyInfo _viewVisualTree = _guiViewType.GetProperty("visualTree",
            BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

        static FieldInfo _imguiContainerOnGui = typeof(IMGUIContainer).GetField("m_OnGUIHandler",
            BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

        static ScriptableObject _currentToolbar;
        
        public static Action MainAction;
        
        /// <summary>
        /// Awake function.
        /// </summary>
        static MainLayout()
        {
            EditorApplication.update -= OnUpdate;
            EditorApplication.update += OnUpdate;
        }
        
        static void OnUpdate()
        {
            // Relying on the fact that toolbar is ScriptableObject and gets deleted when layout changes
            if (_currentToolbar == null)
            {
                // Find toolbar
                var toolbars = Resources.FindObjectsOfTypeAll(_toolbarType);
                _currentToolbar = toolbars.Length > 0 ? (ScriptableObject) toolbars[0] : null;
                if (_currentToolbar != null)
                {
                    // Get it's visual tree
                    var visualTree = (VisualElement) _viewVisualTree.GetValue(_currentToolbar, null);

                    // Get first child which 'happens' to be toolbar IMGUIContainer
                    var container = (IMGUIContainer) visualTree[0];

                    // (Re)attach handler
                    var handler = (Action) _imguiContainerOnGui.GetValue(container);
                    handler -= OnGUI;
                    handler += OnGUI;
                    _imguiContainerOnGui.SetValue(container, handler);
                }
            }
        }

        /// <summary>
        /// Update function.
        /// </summary>
        static void OnGUI()
        {
            var action = MainAction;
            if (action != null) action();
        }
    }
}