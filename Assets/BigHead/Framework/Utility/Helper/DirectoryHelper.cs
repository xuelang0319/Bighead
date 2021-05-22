//
// = The script is part of BigHead and the framework is individually owned by Eric Lee.
// = Cannot be commercially used without the authorization.
//
//  Author  |  UpdateTime     |   Desc  
//  Eric    |  2020年12月18日  |   文件夹帮助器
//

using System;
using System.IO;
using BigHead.Framework.Core;

namespace BigHead.Framework.Utility.Helper
{
    public static class DirectoryHelper
    {
        /// <summary>
        /// Directory 类升级方法。
        /// 检查是否存在该路径，不存在则创建。
        /// 若因路径异常报错则仅显示打印信息。
        /// </summary>
        /// <param name="path"> 项目文件夹下的相对路径 </param>
        /// <returns> 创建前是否存在 </returns>
        public static bool ForceCreateDirectory(string path)
        {
            if (!Directory.Exists(path))
            {
                try
                {
                    if (!Directory.CreateDirectory(path).Exists)
                        return false;
                }
                catch (Exception e)
                {
                    $"创建文件夹异常，路径 - {path}。".Error();
                    e.Exception();
                    return false;
                }
            }

            return true;
        }
    }
}