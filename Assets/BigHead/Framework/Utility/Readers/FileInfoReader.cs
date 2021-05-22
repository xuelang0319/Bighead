//
// = The script is part of BigHead and the framework is individually owned by Eric Lee.
// = Cannot be commercially used without the authorization.
//
//  Author  |  UpdateTime     |   Desc  
//  Eric    |  2021年1月14日   |   文件读取器
//

using System;
using System.IO;
using System.Text;
using BigHead.Framework.Core;

namespace BigHead.Framework.Utility.Readers
{
    public static class FileInfoReader
    {
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
    }
}