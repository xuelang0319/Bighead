//
// = The script is part of BigHead and the framework is individually owned by Eric Lee.
// = Cannot be commercially used without the authorization.
//
//  Author  |  UpdateTime     |   Desc  
//  Eric    |  2020年12月18日  |   编辑器自动启动项
//  Eric    |  2020年12月24日  |   修复由于泛型问题导致的自动生成失败。
//

using System.Reflection;
using BigHead.Asset;
using BigHead.Framework.Utility.Helper;
using UnityEditor;

namespace BigHead.Editor
{
    public static class Startup
    {
        /// <summary>
        /// 初始化编辑器Asset配置
        /// </summary>
        [InitializeOnLoadMethod]
        public static void InitAssets()
        {
            var baseType = typeof(AssetBase<>);
            var types = baseType.GetAllDerivedTypes();
            /* TODO: 将所有脚本写成配置表，当脚本被删除时自动删除生成的配置表*/
            foreach (var type in types)
            {
                var method = baseType
                    .MakeGenericType(type)
                    .GetMethod("Initialization", BindingFlags.Static | BindingFlags.Public);
                method?.Invoke(null, new object[0]);
            }
        }
    }
}