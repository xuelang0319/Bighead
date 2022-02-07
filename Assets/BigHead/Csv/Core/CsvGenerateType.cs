//
// = The script is part of BigHead and the framework is individually owned by Eric Lee.
// = Cannot be commercially used without the authorization.
//
//  Author  |  UpdateTime     |   Desc  
//  Eric    |  2021年5月23日   |
//

using System;
using JetBrains.Annotations;

/*
 * 【注意】由于使用CSV文件规范，所以的Excel数据中都不能含有','符号！！！
 * Excel自动生成工具使用方法：
 * 1、格式: 固定前三行， 第一行为生成代码时的变量名称， 第二行为变量类型， 第三行为中文注释
 * 例:
 * Id        Type        Desc        Value        IsStatic
 * int       string      string      string[]     bool
 * 编号       类型         注释        值            是否静态
 * 100001    机甲         示例        护胸|肩甲      TRUE
 *
 * 2、特殊类型：如果需要添加新的类型和解析方式，可以在Assets/BigHead/Customer/CustomerGenCsv中添加以下数据：
 * ① GenPropertyType: 转义方法，返回真实变量类型。
 * ② GetTransformFunc: 转义方法，返回调用的解析方法名称。
 * ③ 在CustomerGenCsv脚本中添加②中的对应名称的解析方法，返回类型需与①中的变量真实类型相同。
 *
 * 3、特殊引用：如果生成的类型需要引用一些特殊的NameSpace,可以在Assets/BigHead/Customer/CustomerGenCsv.GetUsings()中添加相应的引用名称。
 * 例： "System" (不需要添加using前缀，也不要添加';'结束符)
 *
 * 4、如果表格名称前填写有 “#” 号则该表格不生成。
 * 5、如果表格内容第二行内容不填写或添加了 “#” 或 “不生成” 则不生成该列内容。
 * 例：
 *
 * Id        Type        Desc        Value        IsStatic
 * int        #                      #string[]    不生成
 * 编号       类型         注释        值           是否静态
 * 100001    机甲         示例        护胸|肩甲      TRUE
 * 该数据只会生成Id列
 * 
 * 6、第三行的描述会自动填写到该类的生成属性的描述中。
 */


namespace BigHead.Csv.Core
{
    public static class CsvGenerateType
    {
        /// <summary>
        /// 生成脚本的引用类型
        /// </summary>
        public static string[] GetUsings()
        {
            return new string[]
            {
                "UnityEngine",
                "System.Collections.Generic",
                "System",
            };
        }
        
        /// <summary>
        /// 转义方法获取真实类型
        /// </summary>
        public static string GetPropertyType(string str)
        {
            switch (str)
            {
                case "int":
                    return "int";
                case "string":
                    return "string";
                case "bool":
                    return "bool";
                case "float":
                    return "float";
                case "int[]":
                    return "int[]";
                case "int[,]":
                    return "int[,]";
                case "string[]":
                    return "string[]";
                case "bool[]":
                    return "bool[]";
                case "float[]":
                    return "float[]";
                default:
                    throw new Exception($"Convert csv data type error. Table type: {str}");
            }
        }
  
        /// <summary>
        /// 转义方法获取解析方法名称
        /// </summary>
        public static string GetTransformFunc(string type, string value)
        {
            switch (type)
            {
                case "int":
                    return $"ToInt({value})";
                case "string":
                    return value;
                case "bool":
                    return $"bool.Parse({value})";
                case "float":
                    return $"ToFloat({value})";
                case "int[]":
                    return $"ToIntArray({value})";  
                case "int[,]":
                    return $"ToInt2Array({value})";
                case "string[]":
                    return $"ToStringArray({value})";
                case "bool[]":
                    return $"ToBoolArray({value})";
                case "float[]":
                    return $"ToFloatArray({value})";
                default:
                    throw new Exception($"Convert csv data error. Table type： {type}, Table value: {value}");
            }
        }

        public static int ToInt(string str)
        {
            try
            {
                return string.IsNullOrEmpty(str) || string.IsNullOrWhiteSpace(str) ? 0 : int.Parse(str);
            }
            catch (Exception e)
            {
                $"Parse string error: {str} - Exception : {e}".Error();
                return 0;
            }
        }

        public static float ToFloat(string str) =>
            string.IsNullOrEmpty(str) ? 0 : float.Parse(str);

        public static int[] ToIntArray(string str)
        {
            if (string.IsNullOrEmpty(str)) return new int[0];
            return Array.ConvertAll(str.Split('|'), int.Parse);
        }

        public static int[,] ToInt2Array(string str)
        {
            if (string.IsNullOrEmpty(str)) return new int[0, 0];
            var x = str.Split('|');
            var y = x[0].Split('_');
            int[,] temp = new int[x.Length, y.Length];
            for (int i = 0; i < x.Length; i++)
            {

                y = x[i].Split('_');
                for (int j = 0; j < y.Length; j++)
                {
                    int value = 0;
                    int.TryParse(y[j], out value);
                    temp[i, j] = value;
                }
            }

            return temp;
        }

        public static string[] ToStringArray(string str)
        {
            if (string.IsNullOrEmpty(str)) return new string[0];
            return str.Split('|');
        }

        public static bool[] ToBoolArray(string str)
        {
            if (string.IsNullOrEmpty(str)) return new bool[0];
            return Array.ConvertAll(str.Split('|'), bool.Parse);
        }

        public static float[] ToFloatArray(string str)
        {
            if (string.IsNullOrEmpty(str)) return new float[0];
            return Array.ConvertAll(str.Split('|'), float.Parse);
        }


        [CanBeNull]
        public static string ToNull()
        {
            return null;
        }
    }
}