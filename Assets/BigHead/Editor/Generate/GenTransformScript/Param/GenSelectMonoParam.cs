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