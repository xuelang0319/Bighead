//
// = The script is part of BigHead and the framework is individually owned by Eric Lee.
// = Cannot be commercially used without the authorization.
//
//  Author  |  UpdateTime     |   Desc  
//  Eric    |  2020年12月18日  |   类帮助器
//  Eric    |  2020年12月24日  |   1、 Assembly.GetCallingAssembly() 修改为 Type.Assembly 以解决当调用程序和检索类不在同一程序集时获取为空的问题。
//                            |   2、 添加HasImplementedRawGeneric方法，以获取继承泛型和接口的子类。
//

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;

namespace BigHead.Framework.Utility.Helper
{
    public static class ClassHelper
    {
        /// <summary>
        /// 通过基类类型获取所有继承类类型
        /// </summary>
        public static Type[] GetAllDerivedTypes(this Type baseclass)
        {
            return baseclass.Assembly.GetTypes()
                .Where(type => type.HasImplementedRawGeneric(baseclass))
                .Where(t => !t.IsAbstract && t.IsClass)
                .ToArray();
        }
        
        /// <summary>
        /// 通过基类获取所有继承类并创建。
        /// </summary>
        public static IEnumerable<T> CreateAllDerivedClass<T>()
        {
            return GetAllDerivedTypes(typeof(T))
                .Select(t => (T)Activator.CreateInstance(t));
        }

        /// <summary>
        /// 通过基类获取所有继承类并创建。
        /// </summary>
        public static IEnumerable<T> CreateAllDerivedClass<T>(this Type baseclass)
        {
            return GetAllDerivedTypes(baseclass)
                .Select(t => (T)Activator.CreateInstance(t));
        }
        
        /// <summary>
        /// 判断指定的类型 <paramref name="type"/> 是否是指定泛型类型的子类型，或实现了指定泛型接口。
        /// </summary>
        /// <param name="type">需要测试的类型。</param>
        /// <param name="generic">泛型接口类型，传入 typeof(IXxx&lt;&gt;)</param>
        /// <returns>如果是泛型接口的子类型，则返回 true，否则返回 false。</returns>
        public static bool HasImplementedRawGeneric(this Type type, Type generic)
        {
            if (Equals(null, type) || Equals(null, generic)) return false;
            
            var isTheRawGenericType = type.GetInterfaces().Any(IsTheRawGenericType);
            if (isTheRawGenericType) return true;

            while (type != null && type != typeof(object))
            {
                isTheRawGenericType = IsTheRawGenericType(type);
                if (isTheRawGenericType) return true;
                type = type.BaseType;
            }

            return false;
            
            bool IsTheRawGenericType(Type test)
                => generic == (test.IsGenericType ? test.GetGenericTypeDefinition() : test);
        }
    }
}