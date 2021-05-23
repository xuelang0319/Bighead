//
// = The script is part of BigHead and the framework is individually owned by Eric Lee.
// = Cannot be commercially used without the authorization.
//
//  Author  |  UpdateTime     |   Desc  
//  Eric    |  2021年5月23日   |   方法模板
//

using System.Collections.Generic;
using System.Text;

namespace BigHead.Editor.Generate.GenBasic
{
    public class GenFoo : GenBasic
    {
        private readonly string _returnType;
        private List<GenProperty> _params;
        private List<string> _details;
        public bool IsOverride = false;
        public bool IsConstruct = false;
        public bool IsVirtual = false;


        public GenFoo(int charLength, string name, string returnType) : base(charLength, name)
        {
            _returnType = returnType;
        }
        
        public override StringBuilder StartGenerate()
        {
            var builder = new StringBuilder();

            builder.Append(Charactor);
            builder.Append(GetAnnotation());

            builder
                .Append(GetModifier)
                .Append(Space)
                .Append(IsOverride ? "override " : "")
                .Append(IsVirtual ? "virtual " : "")
                .Append(IsConstruct ? "" : _returnType + Space)
                .Append(Name);
            
            var paramBuilder = new StringBuilder();
            if (_params != null)
                for (int i = 0; i < _params.Count; i++)
                {
                    paramBuilder.Append(_params[i].AsParam());
                    if (i < _params.Count - 1)
                        paramBuilder.Append(", ");
                }

            builder.Append(AddBrackets(paramBuilder));

            var detailBuilder = new StringBuilder();
            if (_details != null)
                for (int i = 0; i < _details.Count; i++)
                {
                    // 由于要加入大括号，所以每个提前向后移动
                    detailBuilder
                        .Append(GetOffsetCharactor(4))
                        .Append(_details[i]);
                    if (i < _details.Count - 1)
                        detailBuilder.Append(CharNewLine);
                }

            builder.Append(AddBraces(detailBuilder));
            return builder;
        }

        /// <summary>
        /// 添加参数
        /// </summary>
        public GenProperty AddParam(string type, string name)
        {
            if (_params == null) _params = new List<GenProperty>();
            var property = new GenProperty(CharLength + 4, name, type);
            _params.Add(property);
            return property;
        }

        /// <summary>
        /// 添加方法代码行
        /// </summary>
        public GenFoo AddDetail(string line)
        {
            if (_details == null) _details = new List<string>();
            _details.Add(Charactor + line);
            return this;
        }
    }
}