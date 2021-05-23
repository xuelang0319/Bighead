//
// = The script is part of BigHead and the framework is individually owned by Eric Lee.
// = Cannot be commercially used without the authorization.
//
//  Author  |  UpdateTime     |   Desc  
//  Eric    |  2021年5月22日   |   自动生成基类 - 当前版本生成模板没有NameSpace
//

using System;
using System.Text;

namespace BigHead.Editor.Generate.GenBasic
{
    public abstract class GenBasic
    {
        public enum modifier
        {
            Private,
            Public,
            Protected,
            Private_Static,
            Public_Static,
            Protected_Static
        }

        protected modifier Modifier = modifier.Public;
        protected string GetModifier => Modifier.ToString().Replace('_', ' ').ToLower();
        
        protected string Name;
        
        protected string Charactor;
        protected int CharLength;
        
        protected const string Space = " ";
        protected readonly string CharNewLine;
        protected readonly string DoubleCharNewLine;

        /// <summary> 注释 </summary>
        protected string Annotation;
        
        public GenBasic(int charLength, string name)
        {
            Name = name;
            CharLength = charLength;
            Charactor = new string(' ', CharLength);
            CharNewLine = Environment.NewLine + Charactor;
            DoubleCharNewLine = Environment.NewLine + Environment.NewLine + Charactor;
        }

        /// <summary>
        /// 开始生成
        /// </summary>
        public abstract StringBuilder StartGenerate();
        
        /// <summary>
        /// 获取在当前空格符基础长度之上再偏移的换行符
        /// </summary>
        protected string GetOffsetCharactor(int offset)
        {
            if (offset + CharLength < 0) return "";
            return new string(' ', CharLength + offset);
        }

        /// <summary>
        /// 重新设置空格符字符串长度，
        /// </summary>
        public string ResetCharactor(int charLength)
        {
            Charactor = new string(' ', charLength);
            return Charactor;
        }
        
        /// <summary>
        /// 添加小括号
        /// </summary>
        protected string AddBrackets(StringBuilder builder)
        {
            return $"({builder})";
        }

        /// <summary>
        /// 添加大括号
        /// </summary>
        protected string AddBraces(StringBuilder builder)
        {
            return new StringBuilder()
                .Append(Charactor)
                .Append("{")
                .AppendLine()
                .Append(builder)
                .AppendLine()
                .Append(Charactor)
                .Append("}")
                .ToString();
        }

        /// <summary>
        /// 获取注释
        /// </summary>
        protected StringBuilder GetAnnotation()
        {
            var builder = new StringBuilder();
            
            if (string.IsNullOrEmpty(Annotation))
                return builder;

            const string start = @"/// <summary>";
            const string middle = @"/// ";
            const string end = @"/// </summary>";
            builder.Append(CharNewLine);
            builder.Append(start);
            
            builder.Append(CharNewLine);
            builder.Append(middle);
            builder.Append(Annotation);
            
            builder.Append(CharNewLine);
            builder.Append(end);

            builder.Append(CharNewLine);
            return builder;
        }
    }
}