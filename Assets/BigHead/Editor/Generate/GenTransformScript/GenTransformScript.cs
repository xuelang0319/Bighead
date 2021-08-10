//
// = The script is part of BigHead and the framework is individually owned by Eric Lee.
// = Cannot be commercially used without the authorization.
//
//  Author  |  UpdateTime     |   Desc  
//  Eric    |  2021年8月9日    |   选择的物体自动生成脚本
//

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using BigHead.Editor.Generate.GenBasic;
using BigHead.Editor.Generate.GenTransformScript.Param;
using BigHead.Framework.Core;
using BigHead.Framework.Utility.Helper;
using UnityEditor;
using UnityEngine;

namespace BigHead.Editor.Generate.GenTransformScript
{
    public static class GenTransformScript
    {
        public static void AnalysisSelectPrefab(GenSelectParamBase param)
        {
            var selections = Selection.gameObjects;
            if (Equals(0, selections.Length))
            {
                "You didn't select any object, please select panel game object.".Error();
                return;
            }

            var genPath = Application.dataPath + "/" + param.GeneratePath.TrimEnd('/').TrimEnd('\\') + "/";
            DirectoryHelper.ForceCreateDirectory(genPath);

            for (var i = 0; i < selections.Length; i++)
            {
                var obj = selections[i];
                var fullName = param.HeadName + obj.name;

                var genClass = new GenClass(0, fullName) {Parent = param.Parent, Modifier = param.ClassModifier};
                if (param.VirtualType) genClass.virtualType = fullName;
                for (var j = 0; j < param.Usings.Count; j++)
                    genClass.AddUsing(param.Usings[j]);

                var keyPair = param.KeyPair;
                var list = new List<(Transform t, string typeName)>();
                
                EditorUtility.DisplayProgressBar("Processing", $"正在处理: {obj.name}", i / (float)selections.Length - 1);
                GetAll(keyPair, obj.transform, (transform, s) =>
                {
                    var fullType = keyPair[s];
                    genClass.AddProperty(transform.name, fullType);
                    list.Add((transform, fullType));
                });

                var initFoo = genClass.AddFoo(param.FunctionName, param.FunctionReturnType);
                initFoo.Modifier = param.FunctionModifier;
                initFoo.SetOverrider(param.FunctionOverride);
                for (int j = 0; j < list.Count; j++)
                {
                    var t = list[j].t;
                    var typeName = list[j].typeName;

                    var fooDetail = "";
                    if (Equals(t, obj.transform))
                    {
                        fooDetail = $"{t.name} = GetComponent<{typeName}>();";
                    }
                    else
                    {
                        var path = t.name;
                        var temp = t;
                        while (temp.parent != null && temp.parent != obj.transform)
                        {
                            var parent = temp.parent;
                            path = parent.name + "/" + path;
                            temp = parent;
                        }

                        fooDetail = $"{t.name} = transform.Find(\"{path}\").GetComponent<{typeName}>();";
                    }

                    initFoo.AddDetail(fooDetail);
                }

                var data = Encoding.UTF8.GetBytes(genClass.StartGenerate().ToString());

                var scriptName = genPath + fullName + ".cs";
                using (var fileStream = new FileStream(scriptName, FileMode.Create))
                    fileStream.Write(data, 0, data.Length);
            }
            
            EditorUtility.ClearProgressBar();
            AssetDatabase.Refresh();
        }
        
        private static void GetAll(Dictionary<string, string> keyPair, Transform t, Action<Transform, string> callback)
        {
            var name = t.name;
            foreach (var key in keyPair.Keys)
            {
                if (!name.Contains(key)) 
                    continue;
                
                callback?.Invoke(t, key);
            }
            
            if (Equals(0, t.childCount)) 
                return;
            
            foreach (Transform child in t)
                GetAll(keyPair, child, callback);
        }
    }
}