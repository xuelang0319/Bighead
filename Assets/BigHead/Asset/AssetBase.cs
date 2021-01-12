//
// = The script is part of BigHead and the framework is individually owned by Eric Lee.
// = Cannot be commercially used without the authorization.
//
//  Author  |  UpdateTime     |   Desc  
//  Eric    |  2020年12月17日  |   自动化配置表基类
//

using System.Threading;
using BigHead.Framework.Extension;
using UnityEditor;
using UnityEngine;

/*使用方法：

1、继承AssetBase父类
2、显示型属性添加标签： [SerializeField]
3、需要提示型属性标签： [Tooltip("String")]

即可自动在配置路径下生成该类名称的Asset文件。*/
namespace BigHead.Asset
{
    public abstract class AssetBase<T> : ScriptableObject where T : AssetBase<T>, new()
    {
        /// <summary>
        /// 类名即文件名
        /// </summary>
        protected static string ASSET_NAME => typeof(T).Name;

        /// <summary>
        /// 完整路径
        /// </summary>
        protected static string ASSET_PATH => $"Assets/BigHead/Customer/Configs/{ASSET_NAME}.asset";

        /// <summary>
        /// 目录创建
        /// </summary>
        protected static int IsConfigDirCreate;

        /// <summary>
        /// 文件创建
        /// </summary>
        protected static int IsConfigFileCreate;

        /// <summary>
        /// 获取实例信息
        /// </summary>
        public static T Config;

        /// <summary>
        /// 初始化配置文件
        /// </summary>
        public static void Initialization()
        {
            if (IsConfigFileCreate != default && IsConfigFileCreate != default)
            {
                $"this {ASSET_NAME} is already created.".Error();
                return;
            }

            lock (ASSET_PATH)
            {
                Config = AssetDatabase.LoadAssetAtPath<T>(ASSET_PATH);
                if (Equals(Config, null))
                {
                    Config = CreateInstance<T>();
                    if (!AssetDatabase.IsValidFolder("Assets/BigHead/Customer") &&
                        Interlocked.Exchange(ref IsConfigDirCreate, 1) == 0)
                        AssetDatabase.CreateFolder("Assets/BigHead", "Customer");
                    if (Interlocked.Exchange(ref IsConfigFileCreate, 1) == 0)
                        AssetDatabase.CreateAsset(Config, ASSET_PATH);
                }
            }

            Config = AssetDatabase.LoadAssetAtPath<T>(ASSET_PATH);
        }
    }
}