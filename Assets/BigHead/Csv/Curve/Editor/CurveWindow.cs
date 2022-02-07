//
// = The script is part of BigHead and the framework is individually owned by Eric Lee.
// = Cannot be commercially used without the authorization.
//
//  Author  |  UpdateTime     |   Desc  
//  Eric    |  2021年12月13日  |   曲线编辑器
//

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using BigHead.Core.Editor.GenBasic;
using BigHead.Csv.Core;
using UnityEditor;
using UnityEngine;

namespace BigHead.Csv.Curve.Editor
{
    public class CurveWindow : EditorWindow
    {
        /// <summary> 曲线参数集合 </summary>
        private static readonly Dictionary<string, CurveParam> CurveParams = new Dictionary<string, CurveParam>();
        
        /// <summary>
        /// 入口
        /// </summary>
        public static void OpenWindow()
        {
            LoadCsvData();
        }

        /// <summary>
        /// 读取本地数据
        /// </summary>
        private static void LoadCsvData()
        {
            CurveParams.Clear();
            var window = GetWindow(typeof(CurveWindow));
            window.titleContent = new GUIContent("Curve Editor");
            if (!File.Exists(CurveConfig.CurveCsvPath))
            {
                FileHelper.CreateFile(CsvConfig.ConstCsvPath, GetCsvHead(), CurveConfig.CurveCsvName);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }

            var str = CsvReader.ReadCsvWithPath(CurveConfig.CurveCsvPath);
            var curves = CsvReader.ToListWithDeleteFirstLines(str, 3);
            for (int i = 0; i < curves.Count; i++)
            {
                var curveParam = new CurveParam(curves[i]);
                CurveParams.Add(curveParam.Id, curveParam);
            }
        }

        /// <summary>
        /// 创建新的曲线
        /// </summary>
        private static void CreateNew()
        {
            var id = ((CurveParams.Count > 0 ? CurveParams.Keys.Select(int.Parse).Max() : 100001) + 1).ToString();
            var curveParam = new CurveParam(id, $"Curve_{id}");
            CurveParams.Add(id, curveParam);
        }

        /// <summary>
        /// 保存文件
        /// </summary>
        private static void Save()
        {
            SaveCsvData();
            GenerateCurveScript();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        /// <summary>
        /// 保存CSV格式曲线数据
        /// </summary>
        private static void SaveCsvData()
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.Append(GetCsvHead());
            foreach (var param in CurveParams)
            {
                stringBuilder.AppendLine();
                stringBuilder.Append(param.Value.ToString());
            }
            
            FileHelper.CreateFile(CsvConfig.ConstCsvPath, stringBuilder.ToString(), CurveConfig.CurveCsvName);
        }

        /// <summary>
        /// 生成曲线对应脚本
        /// </summary>
        private static void GenerateCurveScript()
        {
            var genClass = new GenClass(0, "CurveAssistant");
            genClass.IsPartial = true;
            genClass.Modifier = BigHead.Core.Editor.GenBasic.GenBasic.modifier.Public_Static;

            foreach (var curveParam in CurveParams.Values)
            {
                if(curveParam.Removed) continue;
                var foo = genClass.AddFoo($"Get{curveParam.Name}Value", "float");
                foo.Modifier = BigHead.Core.Editor.GenBasic.GenBasic.modifier.Public_Static;
                foo
                    .AddParam("float", "percent")
                    .AddDetail($"return GetCurveValue({curveParam.Id}, percent);");
            }
            
            FileHelper.CreateFile(
                CurveConfig.GenerateCurveScriptPath, 
                genClass.StartGenerate().ToString(),
                CurveConfig.GenerateCurveScriptName);
            
            EditorUtility.ClearProgressBar();
            AssetDatabase.Refresh();
        }

        /// <summary>
        /// GUI
        /// </summary>
        private void OnGUI()
        {
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.BeginVertical();
            GUILayout.Space(10);
            GUILayout.Label("Bighead Curve Editor", CurveStyle.Style, GUILayout.Height(50));
            GUILayout.Space(20);
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("New", GUILayout.Width(120)))
            {
                CreateNew();
            }
            GUILayout.Space(10f);
            if (GUILayout.Button("Save", GUILayout.Width(120)))
            {
                Save();
            }

            GUILayout.Space(200f);
            if (GUILayout.Button("Reload", GUILayout.Width(120)))
            {
                LoadCsvData();
            }
            EditorGUILayout.EndHorizontal();

            GUILayout.Space(20);

            foreach (var param in CurveParams.Values)
            {
                if(param.Removed) continue;
                EditorGUILayout.BeginHorizontal();
                GUILayout.Label("Id: " + param.Id, GUILayout.Width(100));
                GUILayout.Space(10);
                GUILayout.Label("Name: ", GUILayout.Width(50));
                GUILayout.Space(10);
                param.Name = EditorGUILayout.TextField(param.Name, GUILayout.Width(200));
                GUILayout.Space(70);
                if (GUILayout.Button("Delete", GUILayout.Width(120)))
                {
                    param.Removed = true;
                }
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.CurveField(param.AnimationCurve, GUILayout.Width(580));
                GUILayout.Space(10);
            }

            EditorGUILayout.EndVertical();
            EditorGUI.EndChangeCheck();
        }
        
        /// <summary>
        /// 获取CSV文件头部
        /// </summary>
        /// <returns></returns>
        private static string GetCsvHead()
        {
            var builder = new StringBuilder();
            builder
                .Append("ID,Name,Frames").AppendLine()
                .Append(":Int,:Str,:Array:Float").AppendLine()
                .Append("编号,描述,帧数据");
            return builder.ToString();
        }
        
        /// <summary>
        /// 曲线数据参数
        /// </summary>
        private class CurveParam
        {
            /// <summary>
            /// 曲线ID
            /// </summary>
            public readonly string Id;
            
            /// <summary>
            /// 曲线描述
            /// </summary>
            public string Name;

            /// <summary>
            /// 已经移除
            /// </summary>
            public bool Removed;
            
            /// <summary>
            /// 当前曲线句柄
            /// </summary>
            public readonly AnimationCurve AnimationCurve;
            
            /// <summary>
            /// 构造方法 - 用于新建
            /// </summary>
            /// <param name="id">曲线ID</param>
            /// <param name="name">曲线描述</param>
            public CurveParam(string id, string name)
            {
                Id = id;
                Name = name;
                AnimationCurve = new AnimationCurve();
            }
            
            /// <summary>
            /// 构造方法 - 用于读取
            /// </summary>
            /// <param name="line">配置表参数</param>
            public CurveParam(string line)
            {
                var strs = line.Split(',');
                Id = strs[0];
                Name = strs[1];
                AnimationCurve = new AnimationCurve();
                
                if (string.IsNullOrWhiteSpace(strs[2])) return;
                var frameValues = strs[2].Split('|');
                for (int i = 0; i < frameValues.Length; i++)
                {
                    var frame = GetKeyframe(frameValues[i]);
                    AnimationCurve.AddKey(frame);
                }
            }

            /// <summary>
            /// 获取曲线帧
            /// </summary>
            /// <param name="str"></param>
            /// <returns></returns>
            private Keyframe GetKeyframe(string str)
            {
                var info = Array.ConvertAll(str.Split('、'), float.Parse);
                return new Keyframe(info[0], info[1], info[2], info[3]);
            }

            /// <summary>
            /// 重写字符串生成方法
            /// </summary>
            public new string ToString()
            {
                var stringBuilder = new StringBuilder();
                var commaSymbol = ',';
                var middleCharacter = '|';
                var name = string.IsNullOrEmpty(Name) || string.IsNullOrWhiteSpace(Name) ? $"Curve_{Id}" : Name;
                stringBuilder.Append(Id).Append(commaSymbol).Append(name).Append(commaSymbol);
                var keys = AnimationCurve.keys;
                for (int i = 0; i < keys.Length; i++)
                {
                    if (i > 0) stringBuilder.Append(middleCharacter);
                    var key = keys[i];
                    stringBuilder.Append(
                        $"{key.time.KeepDecimals()}、{key.value.KeepDecimals()}、{key.inTangent.KeepDecimals()}、{key.outTangent.KeepDecimals()}");
                }

                return stringBuilder.ToString();
            }
        }
    }
}