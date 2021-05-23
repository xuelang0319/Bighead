//
// = The script is part of BigHead and the framework is individually owned by Eric Lee.
// = Cannot be commercially used without the authorization.
//
//  Author  |  UpdateTime     |   Desc  
//  Eric    |  2020年12月17日  |   Base64加解密算法
//

using System;
using System.Text;

namespace BigHead.Framework.Utility.Crypto
{
    public partial class BigHeadCrypto
    {
        /// <summary>
        /// Base64加密
        /// </summary>
        /// <param name="plainText">字符串</param>
        /// <returns>字符串</returns>
        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            return Convert.ToBase64String(plainTextBytes);
        }

        /// <summary>
        /// Base64解密
        /// </summary>
        /// <param name="base64EncodedData">字符串</param>
        /// <returns>字符串</returns>
        public static string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = Convert.FromBase64String(base64EncodedData);
            return Encoding.UTF8.GetString(base64EncodedBytes);
        }
    }
}