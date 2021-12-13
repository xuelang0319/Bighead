//
// = The script is part of BigHead and the framework is individually owned by Eric Lee.
// = Cannot be commercially used without the authorization.
//
//  Author  |  UpdateTime     |   Desc  
//  Eric    |  2021年5月23日   |
//

using System;
using BigHead.Framework.Core;
using JetBrains.Annotations;

/*
 * 【注意】由于使用CSV文件规范，所以的Excel数据中都不能含有','符号！！！
 * Excel自动生成工具使用方法：
 * 1、格式: 固定前三行， 第一行为生成代码时的变量名称， 第二行为变量类型， 第三行为中文注释
 * 例:
 * Id        Type        Desc        Value        IsStatic
 * :int      :str        :str        :Array:Str   :Bool
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
 */


namespace BigHead.Customer
{
    public static class CustomerGenCsv
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
                case "INT":
                case "Int":
                case ":Int":
                case "Uni:Int":
                case "UNI:INT":
                case "Uni:INT":
                case "UNI:Int":
                case "int":
                    return "int";
                case "STR":
                case "STRING":
                case ":Str":
                case ":str":
                case "Uni:STR":
                case "Uni:STRING":
                case "Uni:Str":
                case "Uni:String":
                case "UNI:STR":
                case "UNI:STRING":
                case "UNI:Str":
                case "UNI:String":
                case "string":
                    return "string";
                case "BOOL":
                case "Bool":
                case ":Bool":
                case ":bool":
                case "bool":
                    return "bool";
                case "FLOAT":
                case "Float":
                case ":float":
                case "float":
                case ":FLO":
                    return "float";
                case "[INT]":
                case "[Int]":
                case ":Array:Int":
                case "INT[]":
                case "Int[]":
                case "int[]":
                    return "int[]";
                case "[[INT]]":
                case "[[Int]]":
                case ":Array:Int[]":
                case "INT[][]":
                case "Int[][]":
                case "int[,]":
                    return "int[,]";
                case "[STRING]":
                case "[STR]":
                case "[Str]":
                case ":Array:Str":
                case "string[]":
                    return "string[]";
                case "[BOOL]":
                case "[Bool]":
                case ":Array:Bool":
                case ":Array:bool":
                case "bool[]":
                    return "bool[]";
                case "[FLOAT]":
                case "[Float]":
                case ":Array:float":
                case ":Array:Float":
                case "float[]":
                    return "float[]";
                default:
                    throw new Exception($"转换CSV属性类型错误，值: {str}");
            }
        }
  
        /// <summary>
        /// 转义方法获取解析方法名称
        /// </summary>
        public static string GetTransformFunc(string type, string value)
        {
            switch (type)
            {
                case "INT":
                case "Int":
                case ":Int":
                case "Uni:Int":
                case "UNI:INT":
                case "Uni:INT":
                case "UNI:Int":
                    return $"ToInt({value})";
                case "STR":
                case "STRING":
                case ":Str":
                case ":str":
                case "Uni:STR":
                case "Uni:STRING":
                case "Uni:Str":
                case "Uni:String":
                case "UNI:STR":
                case "UNI:STRING":
                case "UNI:Str":
                case "UNI:String":
                    return value;
                case "BOOL":
                case "Bool":
                case ":Bool":
                case ":bool":
                    return $"bool.Parse({value})";
                case "FLOAT":
                case "Float":
                case ":float":
                case ":FLO":
                    return $"ToFloat({value})";
                case "[INT]":
                case "[Int]":
                case "INT[]":
                case "Int[]":
                case ":Array:Int":
                    return $"ToIntArray({value})";
                case "[[INT]]":
                case "[[Int]]":
                case ":Array:Int[]":
                case "INT[][]":
                case "Int[][]":
                    return $"ToInt2Array({value})";
                case "[STRING]":
                case "[STR]":
                case "[Str]":
                case ":Array:Str":
                    return $"ToStringArray({value})";
                case "[BOOL]":
                case "[Bool]":
                case ":Array:Bool":
                case ":Array:bool":
                    return $"ToBoolArray({value})";
                case "[FLOAT]":
                case "[Float]":
                case ":Array:float":
                case ":Array:FLO":
                    return $"ToFloatArray({value})";
                default:
                    throw new Exception($"转换CSV数据类型错误，指定类型： {type}, 值: {value}");
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