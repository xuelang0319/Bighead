using UnityEngine;

namespace BigHead.Editor.Toolbar
{
    public class CommandStyle
    {
        public static readonly GUIStyle Style;

        static CommandStyle()
        {
            Style = new GUIStyle("Command")
            {
                fontSize = 9,
                alignment = TextAnchor.MiddleCenter,
                imagePosition = ImagePosition.ImageAbove,
                fontStyle = FontStyle.Bold
            };
        }
    }
}