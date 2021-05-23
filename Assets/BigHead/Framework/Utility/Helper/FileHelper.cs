//
// = The script is part of BigHead and the framework is individually owned by Eric Lee.
// = Cannot be commercially used without the authorization.
//
//  Author  |  UpdateTime     |   Desc  
//  Eric    |  2021年5月23日   |   文件助手
//

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using BigHead.Framework.Core;

namespace BigHead.Framework.Utility.Helper
{
    public static class FileHelper
    {
        /// <summary>
        /// 通过路径获取文件所有信息并按行存储为列表
        /// </summary>
        public static List<string> ReadFile(string path)
        {
            try
            {
                return File.ReadAllLines(path).ToList();
            }
            catch (Exception e)
            {
                $"读取文件失败： {e}".Exception();
                return null;
            }
        }
        
        /// <summary>
        /// 通过FileInfo获取文件整体信息
        /// </summary>
        public static string ReadFile(this FileInfo info)
        {
            try
            {
                using (var stream = info.OpenRead())
                {
                    var buffer = new byte[stream.Length];
                    var length = stream.Read(buffer, 0, buffer.Length);
                    return Encoding.UTF8.GetString(buffer, 0, length);
                }
            }
            catch (Exception e)
            {
                e.Exception();
                return string.Empty;
            }
        }

        /// <summary>
        /// 读取文件的所有信息并转换为Bytes
        /// </summary>
        public static byte[] ReadFileToBytes(string path)
        {
            return Encoding.UTF8.GetBytes(File.ReadAllText(path));
        }

        /// <summary>
        /// 创建文件
        /// </summary>
        public static void CreateFile(string path,string content, string fileName)
        {
            DirectoryHelper.ForceCreateDirectory(path);
            var bytes = Encoding.UTF8.GetBytes(content);
            using (var fileStream = new FileStream(path.TrimEnd('/').TrimEnd('\\') + "/" + fileName, FileMode.Create))
            {
                fileStream.Write(bytes, 0, bytes.Length);
            }
        }

        /// <summary>
        /// 删除文件
        /// </summary>
        public static void DeleteFile(string path)
        {
            if(File.Exists(path)) File.Delete(path);
            else $"删除废弃脚本时没有找到对应的生成.cs文件,路径：{path}".Error(); 
        }
    }
}