//
// = The script is part of BigHead and the framework is individually owned by Eric Lee.
// = Cannot be commercially used without the authorization.
//
//  Author  |  UpdateTime     |   Desc  
//  Eric    |  2021年5月23日   |   属性模板
//

using System.Text;

namespace BigHead.Editor.Generate.GenBasic
{
    public class GenProperty : GenBasic
    {
        public string Type;
        public string Value;
        public bool Get = true;
        public bool Set = true;
        public bool Override = false;

        public GenProperty(int charLength, string name, string type) : base(charLength, name)
        {
            Type = type;
        }

        public string AsParam()
        {
            return $"{Type} {Name}";
        }

        public override StringBuilder StartGenerate()
        {
            var getset = "";
            switch (Get)
            {
                case true when Set:
                    getset = " { get; set;}";
                    break;
                case true:
                    getset = " { get;}";
                    break;
                default:
                {
                    if (Set) getset = " { set;}";
                    break;
                }
            }
            
            var value = string.IsNullOrEmpty(Value) ? "" : $" = {Value};";
            var Override = this.Override ? "override " : "";

            return new StringBuilder($"{Charactor}{GetAnnotation()} {GetModifier} {Override}{Type} {Name}{getset}{value}");
        }
    }
}