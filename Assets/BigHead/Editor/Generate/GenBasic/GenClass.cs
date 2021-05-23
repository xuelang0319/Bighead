//
// = The script is part of BigHead and the framework is individually owned by Eric Lee.
// = Cannot be commercially used without the authorization.
//
//  Author  |  UpdateTime     |   Desc  
//  Eric    |  2021年5月22日   |   类模板,当前版本没有提供接口
//

using System.Collections.Generic;
using System.Text;

namespace BigHead.Editor.Generate.GenBasic
{
    public class GenClass : GenBasic
    {
        public List<string> Attributes = new List<string>();
        public List<string> Usings = new List<string>();
        public List<GenFoo> Foos = new List<GenFoo>();
        public List<GenProperty> Properties = new List<GenProperty>();
        public string Parent;
        public string virtualType;
        public string Namespace;
        public bool IsPartial = false;

        public GenClass(int charLength, string name) : base(charLength, name)
        {
        }

        /// <summary>
        /// 添加方法
        /// </summary>
        public GenFoo AddFoo(string name, string returnType)
        {
            var foo = new GenFoo(CharLength + 4, name, returnType);
            Foos.Add(foo);
            return foo;
        }

        /// <summary>
        /// 添加属性
        /// </summary>
        public GenProperty AddProperty(string name, string type)
        {
            var property = new GenProperty(CharLength + 4, name, type);
            Properties.Add(property);
            return property;
        }

        /// <summary>
        /// 获取引用
        /// </summary>
        private string GetUsing(string str)
        {
            return "using " + str + ";" + CharNewLine;
        }

        /// <summary>
        /// 开始生成
        /// </summary>
        public override StringBuilder StartGenerate()
        {
            var builder = new StringBuilder();
            for (var i = 0; i < Usings.Count; i++)
            {
                builder.Append(GetUsing(Usings[i]));
                if (i == Usings.Count - 1) builder.Append(DoubleCharNewLine);
            }

            builder.Append(GetAnnotation());
            
            if (Attributes != null)
            {
                foreach (var attribute in Attributes)
                {
                    builder.Append($"[{attribute}]");
                }

                builder.Append(CharNewLine);
            }

            builder.Append(GetModifier + Space + (IsPartial ? "partial " : "") + "class" + Space + Name);
            
            if (Parent != null)
            {
                builder.Append(" : " + Parent);
            }

            if (!string.IsNullOrEmpty(virtualType))
            {
                builder.Append($"<{virtualType}>");
            }

            var detailBuilder = new StringBuilder();
            for (int i = 0; i < Properties.Count; i++)
            {
                detailBuilder.Append(Properties[i].StartGenerate());
                if (i < Properties.Count - 1) detailBuilder.Append(CharNewLine);
            }

            if (Foos.Count > 0) detailBuilder.Append(CharNewLine);

            for (int i = 0; i < Foos.Count; i++)
            {
                detailBuilder.Append(Foos[i].StartGenerate());
                if (i < Foos.Count - 1) detailBuilder.Append(CharNewLine);
            }

            builder.Append(AddBraces(detailBuilder));

            return builder;
        }
    }
}