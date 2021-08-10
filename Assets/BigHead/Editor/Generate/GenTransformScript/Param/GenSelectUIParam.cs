//
// = The script is part of BigHead and the framework is individually owned by Eric Lee.
// = Cannot be commercially used without the authorization.
//
//  Author  |  UpdateTime     |   Desc  
//  Eric    |  2021年8月10日   |   生成选择物体UI参数
//


using System.Collections.Generic;

namespace BigHead.Editor.Generate.GenTransformScript.Param
{
    public class GenSelectUIParam : GenSelectParamBase
    {
        public override string Parent { get; } = "BasePanel";
        public override bool VirtualType { get; } = true;
        public override string HeadName { get; } = "Panel_";
        public override string FunctionName { get; } = "OnAwake";
        public override GenBasic.GenBasic.modifier FunctionModifier { get; } = GenBasic.GenBasic.modifier.Protected;
        public override bool FunctionOverride { get; } = true;

        public override List<string> Usings { get; } = new List<string>()
        {
            "UnityEngine.UI"
        };

        public override Dictionary<string, string> KeyPair { get; } = new Dictionary<string, string>()
        {
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