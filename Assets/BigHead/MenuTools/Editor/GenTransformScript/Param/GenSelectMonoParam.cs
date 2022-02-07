//
// = The script is part of BigHead and the framework is individually owned by Eric Lee.
// = Cannot be commercially used without the authorization.
//
//  Author  |  UpdateTime     |   Desc  
//  Eric    |  2021年8月17日   |   Mono参数
//

using System.Collections.Generic;

namespace BigHead.Editor.Generate.GenTransformScript.Param
{
    public class GenSelectMonoParam : GenSelectParamBase
    {   
        public override List<string> Usings { get; } = new List<string>()
        {
            "UnityEngine",
            "UnityEngine.UI",
        };

        public override Dictionary<string, string> KeyPair { get; } = new Dictionary<string, string>()
        {
            {"Tran", "Transform"},
            {"Rect", "RectTransform"},
            {"Txt", "Text"},
            {"Img", "Image"},
            {"Btn", "Button"},
            {"Tog", "Toggle"},
            {"Sld", "Slider"},
            {"Scb", "Scrollbar"},
            {"Dpd", "Dropdown"},
            {"Itf", "InputField"},
            {"Pnl", "Panel"},
            {"Scv", "ScrollView"},
        };
    }
}