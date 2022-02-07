//
// = The script is part of BigHead and the framework is individually owned by Eric Lee.
// = Cannot be commercially used without the authorization.
//
//  Author  |  UpdateTime     |   Desc  
//  Eric    |  2022年2月5日    |   曲线编辑器文本款式
//

using UnityEngine;

namespace BigHead.Csv.Curve.Editor
{
    public static class CurveStyle
    {
        public static readonly GUIStyle Style;

        static CurveStyle()
        {
            Style = new GUIStyle
            {
                normal = new GUIStyleState() {textColor = Color.yellow},
                fontSize = 30,
                alignment = TextAnchor.MiddleCenter,
                imagePosition = ImagePosition.ImageAbove,
                fontStyle = FontStyle.Bold
            };
        }
    }
}