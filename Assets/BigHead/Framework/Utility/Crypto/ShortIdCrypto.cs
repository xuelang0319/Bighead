//
// = The script is part of BigHead and the framework is individually owned by Eric Lee.
// = Cannot be commercially used without the authorization.
//
//  Author  |  UpdateTime     |   Desc  
//  Eric    |  2020年12月17日  |   ShortId计算
//

using System;

namespace BigHead.Framework.Utility.Crypto
{
    public partial class BigHeadCrypto
    {
        /// <summary>
        /// 短编码生成器
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string ShortId(string input, bool fix = false)
        {
            Random rd = new Random();
            string[] chars =
            {
                "a", "b", "c", "d", "e", "f", "g", "h",
                "i", "j", "k", "l", "m", "n", "o", "p",
                "q", "r", "s", "t", "u", "v", "w", "x",
                "y", "z", "0", "1", "2", "3", "4", "5",
                "6", "7", "8", "9", "A", "B", "C", "D",
                "E", "F", "G", "H", "I", "J", "K", "L",
                "M", "N", "O", "P", "Q", "R", "S", "T",
                "U", "V", "W", "X", "Y", "Z"
            };
            string hex = MD5Encode(input);
            //把加密字符按照8位一组16进制与0x3FFFFFFF进行位与运算
            int hexint = 0x3FFFFFFF & Convert.ToInt32("0x" + hex.Substring(rd.Next(0, 4) * 8, 8), 16);
            if (fix) hexint = 0x3FFFFFFF & Convert.ToInt32("0x" + hex.Substring(8, 8), 16);
            string outChars = string.Empty;
            for (int j = 0; j < 6; j++)
            {
                //把得到的值与0x0000003D进行位与运算，取得字符数组chars索引
                int index = 0x0000003D & hexint;
                //把取得的字符相加
                outChars += chars[index];
                //每次循环按位右移5位
                hexint = hexint >> 5;
            }

            return outChars;
        }
    }
}