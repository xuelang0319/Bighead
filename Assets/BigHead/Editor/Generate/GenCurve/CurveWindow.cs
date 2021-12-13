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
using BigHead.Editor.Toolbar;
using BigHead.Framework.Extension;
using BigHead.Framework.Utility.Helper;
using BigHead.Framework.Utility.Readers;
using UnityEditor;
using UnityEngine;

namespace BigHead.Editor.Generate.GenCurve
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
            if (!File.Exists(BigheadConfig.CurveCsvPath))
            {
                FileHelper.CreateFile(BigheadConfig.ConstCsvPath, GetCsvHead(), BigheadConfig.CurveCsvName);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }

            var str = CsvReader.ReadCsvWithPath(BigheadConfig.CurveCsvPath);
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
            var curveParam = new CurveParam(id, "New Curve");
            CurveParams.Add(id, curveParam);
        }

        /// <summary>
        /// 保存所有曲线
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
            
            FileHelper.CreateFile(BigheadConfig.ConstCsvPath, stringBuilder.ToString(), BigheadConfig.CurveCsvName);
            AssetDatabase.SaveAssets();
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
            GUILayout.Label("Bighead Curve Editor", TitleStyle.Style, GUILayout.Height(50));
            GUILayout.Space(20);
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("New", GUILayout.Width(120)))
            {
                CreateNew();
            }
            GUILayout.Space(10f);
            if (GUILayout.Button("Save", GUILayout.Width(120)))
            {
                SaveCsvData();
            }

            GUILayout.Space(200f);
            if (GUILayout.Button("Clear", GUILayout.Width(120)))
            {
                LoadCsvData();
            }
            EditorGUILayout.EndHorizontal();

            GUILayout.Space(20);

            foreach (var param in CurveParams.Values)
            {
                EditorGUILayout.BeginHorizontal();
                GUILayout.Label("Id: " + param.Id, GUILayout.Width(100));
                GUILayout.Space(10);
                GUILayout.Label("Desc: ", GUILayout.Width(50));
                param.Desc = EditorGUILayout.TextField(param.Desc, GUILayout.Width(200));
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
                .Append("ID,Desc,Frames").AppendLine()
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
            public string Desc;
            
            /// <summary>
            /// 当前曲线句柄
            /// </summary>
            public readonly AnimationCurve AnimationCurve;
            
            /// <summary>
            /// 构造方法 - 用于新建
            /// </summary>
            /// <param name="id">曲线ID</param>
            /// <param name="desc">曲线描述</param>
            public CurveParam(string id, string desc)
            {
                Id = id;
                Desc = desc;
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
                Desc = strs[1];
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
                stringBuilder.Append(Id).Append(commaSymbol).Append(Desc).Append(commaSymbol);
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