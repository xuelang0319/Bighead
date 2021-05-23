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
using UnityEngine;

namespace BigHead.Editor.Customer
{
    public class CustomerGenCsv
    {
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
                    return "string";
                case "BOOL":
                case "Bool":
                case ":Bool":
                case ":bool":
                    return "bool";
                case "FLOAT":
                case "Float":
                case ":float":
                case ":FLO":
                    return "float";
                case "[INT]":
                case "[Int]":
                case ":Array:Int":
                case "INT[]":
                case "Int[]":
                    return "int[]";
                case "[[INT]]":
                case "[[Int]]":
                case ":Array:Int[]":
                case "INT[][]":
                case "Int[][]":
                    return "int[,]";
                case "[STRING]":
                case "[STR]":
                case "[Str]":
                case ":Array:Str":
                    return "string[]";
                case "[BOOL]":
                case "[Bool]":
                case ":Array:Bool":
                case ":Array:bool":
                    return "bool[]";
                case "[FLOAT]":
                case "[Float]":
                case ":Array:float":
                case ":Array:FLO":
                    return "float[]";
                case "Vector2[]":
                case "Vec2[]":
                case ":array:Vec2":
                    return "Vector2[]";
                case "Vector2":
                case "Vec2":
                case "V2":
                    return "Vector2";
                default:
                    throw new Exception($"转换CSV属性类型错误，值: {str}");
            }
        }

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
                case "Vector2[]":
                case "Vec2[]":
                case ":array:Vec2":
                    return $"ToVector2Array({value})";
                case "Vector2":
                case "Vec2":
                case "V2":
                    return $"ToVector2({value})";
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
            catch(Exception e)
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
            if (string.IsNullOrEmpty(str)) return new int[0,0];
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

        public static Vector2[] ToVector2Array(string str)
        {
            if (string.IsNullOrEmpty(str)) return new Vector2[0];
            var item =  str.Split('|');
            var array = new Vector2[item.Length];
            for (int i = 0; i < item.Length; i++)
            {
                
                var value = item[i].Split('_');
                array[i] = new Vector2(float.Parse(value[0]), float.Parse(value[1]));
            }
            return array;
        }
        public static Vector2 ToVector2(string str)
        {
            if (string.IsNullOrEmpty(str)) return new Vector2(0,0);
            var item = str.Split('_');
            return new Vector2(float.Parse(item[0]), float.Parse(item[1]));
        }
            

        [CanBeNull]
        public static string ToNull()
        {
            return null;
        }
    }
}