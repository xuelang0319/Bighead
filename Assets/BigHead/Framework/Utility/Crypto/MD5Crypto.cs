//
// = The script is part of BigHead and the framework is individually owned by Eric Lee.
// = Cannot be commercially used without the authorization.
//
//  Author  |  UpdateTime     |   Desc  
//  Eric    |  2020年12月17日  |   MD5加密方法
//

using System;
using System.Security.Cryptography;
using System.Text;

namespace BigHead.Framework.Utility.Crypto
{
    public class MD5Crypto
    {
        /// <summary>
        /// MD5的全局句柄
        /// </summary>
        private static readonly MD5 MD5Provider = MD5.Create();

        /// <summary>
        /// 获取Md5值,每次Md5的加权已经加了Salt,外部无法验证通过
        /// </summary>
        /// <param name="input">输入</param>
        /// <param name="hasSalt">是否进行加密化处理</param>
        /// <param name="preSalt"></param>
        /// <param name="postSalt"></param>
        /// <returns>输出</returns>
        public static string MD5Encode(string input, bool hasSalt = false, string preSalt = "", string postSalt = "")
        {
            if (hasSalt)
            {
                input = $"{preSalt}{input}{postSalt}";
            }

            var data = MD5Provider.ComputeHash(Encoding.UTF8.GetBytes(input));

            var sBuilder = new StringBuilder();
            
            for (int i = 0; i < data.Length; i++)
                sBuilder.Append(data[i].ToString("x2"));
            
            return sBuilder.ToString();
        }

        /// <summary>
        /// 验证Md5的加盐Salt哈希
        /// </summary>
        /// <param name="input">字符串</param>
        /// <param name="md5">需要验证的Md5串</param>
        /// <param name="hasSalt">是否进行加密化处理</param>
        /// <returns>验证结果</returns>
        public static bool MD5Verify(string input, string md5, bool hasSalt = false) =>
            StringComparer.OrdinalIgnoreCase.Compare(MD5Encode(input, hasSalt), md5) == 0;
    }
}