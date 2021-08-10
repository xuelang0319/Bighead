//
// = The script is part of BigHead and the framework is individually owned by Eric Lee.
// = Cannot be commercially used without the authorization.
//
//  Author  |  UpdateTime     |   Desc  
//  Eric    |  2021年8月10日   |   生成选择物体参数基类
//

using System.Collections.Generic;

namespace BigHead.Editor.Generate.GenTransformScript.Param
{
    public abstract class GenSelectParamBase
    {
        // 继承类
        public virtual string Parent { get; }
        
        // 类修饰词
        public virtual GenBasic.GenBasic.modifier ClassModifier { get; } = GenBasic.GenBasic.modifier.Public;
        
        // 继承类是否为泛型
        public virtual bool VirtualType { get; }
        
        // 生成脚本类名称头缀
        public virtual string HeadName { get; }
        
        // 脚本生成路径
        public virtual string GeneratePath { get; } = "Scripts/Gen/";
        
        // 初始化方法名称
        public virtual string FunctionName { get; } = "Awake";
        
        // 初始化方法返回类型
        public virtual string FunctionReturnType { get; } = "void";
        
        // 初始化方法修饰词
        public virtual GenBasic.GenBasic.modifier FunctionModifier { get; } = GenBasic.GenBasic.modifier.Public;

        // 初始化方法是否为继承复写类型
        public virtual bool FunctionOverride { get; } = false;

        // 引用集
        public virtual List<string> Usings { get; }
        
        // 检索键值对
        public virtual Dictionary<string, string> KeyPair { get; }
    }
}