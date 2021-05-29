//
// = The script is part of BigHead and the framework is individually owned by Eric Lee.
// = Cannot be commercially used without the authorization.
//
//  Author  |  UpdateTime     |   Desc  
//  Eric    |  2021年5月23日   |   Csv生成Cs脚本
//

using System;
using System.Collections.Generic;
using System.Linq;
using BigHead.Editor.Generate.GenBasic;
using BigHead.Framework.Core;
using BigHead.Framework.Utility.Helper;
using static BigHead.Customer.CustomerGenCsv;

namespace BigHead.Editor.Generate.GenCsv
{
    public static class Csv2Cs
    {
        public static void ClearGenCs()
        {
            DirectoryHelper.ClearDirectory(BigheadConfig.GenerateCsPath);
        }

        public static void GenerateCs(string path)
        {
            var datas = FileHelper.ReadFile(path);
            if (Equals(datas, null) || datas.Count < 3)
            {
                $"File data is null or row count less then 3 - {path}".Error();
                return;
            }

            var name = datas[0].Split(',');
            var type = datas[1].Split(',');
            var desc = datas[2].Split(',');

            var baseName = path.Split('/').Last().Split('.').First();
            var fileName = baseName + "Csv";
            var rowName = baseName + "Row";
            var totalName = fileName + ".cs";

            

            var csvRow = GetCsvRow(name, desc, type, path, rowName);
            var assistant = GetAssistant(fileName);
            var analysis = GetAnalysis(path, baseName, fileName, rowName, name, type);
            
            var csvClass = assistant.StartGenerate()
                .Append(Environment.NewLine)
                .Append(analysis.StartGenerate()).ToString();
            
            DirectoryHelper.ForceCreateDirectory(BigheadConfig.GenerateCsPath);
            FileHelper.CreateFile(BigheadConfig.GenerateCsPath, csvRow.StartGenerate().ToString(), rowName+ ".cs");
            FileHelper.CreateFile(BigheadConfig.GenerateCsPath, csvClass, totalName);
        }

        private static GenClass GetCsvRow(string[] name, string[] desc, string[] type, string path, string rowName)
        {
            var genClass = new GenClass(0, rowName);
            genClass.AddUsing("UnityEngine");
            
            for (int i = 0; i < name.Length; i++)
            {
                var transitionType = "";
                
                try
                {
                    transitionType = GetPropertyType(type[i]);
                }
                catch
                {
                    $"在生成CSV代码时发生类型转换错误，路径： {path}，第{i + 3}列，Type: {type}".Exception();
                    transitionType = ToNull();
                }

                var prop = genClass.AddProperty(name[i], transitionType);
                prop.Annotation = desc[i];
            }
                

            return genClass;
        }

        private static GenClass GetAssistant(string fileName)
        {
            var genClass = new GenClass(0, "CsvAssistant");
            genClass.IsPartial = true;
            genClass.Modifier = GenBasic.GenBasic.modifier.Public_Static;
            genClass
                .AddUsing("System.Collections.Generic")
                .AddUsing("System")
                .AddUsing("static BigHead.Customer.CustomerGenCsv")
                .AddUsing("UnityEngine");
            

            var foo = genClass.AddFoo($"Get{fileName}", fileName);
            foo.Modifier = GenBasic.GenBasic.modifier.Public_Static;
            foo
                .AddDetail($"if(_instance{fileName}.Equals(null)) _instance{fileName} = CsvFunctions.GetCsv<{fileName}>(\"{fileName}\");")
                .AddDetail($"return _instance{fileName};");
            var prop = genClass.AddProperty($"_instance{fileName}", fileName);
            prop.Modifier = GenBasic.GenBasic.modifier.Private_Static;
            
            return genClass;
        }

        private static GenClass GetAnalysis(string path, string baseName, string fileName, string rowName,
            string[] name, string[] type)
        {
            var genClass = new GenClass(0, fileName)
            {
                Modifier = GenBasic.GenBasic.modifier.Public,
                Parent = "BasicCsv",
            };

            // 解析方法
            var readFoo = genClass.AddFoo("AnalysisCsv", "void");
            readFoo.Modifier = GenBasic.GenBasic.modifier.Protected;
            readFoo.SetOverrider(true);
            readFoo.AddParam("List<string>", "list");
            readFoo.AddParam("Action", "callback");

            var list = new List<int>();
            for (int i = 0; i < type.Length; i++)
            {
                if (type[i].StartsWith("Uni:") || type[i].StartsWith("UNI:"))
                    list.Add(i);
            }


            var key = "";
            if (list.Count == 0)
            {
                key = "item[0]";
            }
            else
            {
                for (int i = 0; i < list.Count; i++)
                {
                    key += $"item[{list[i]}]";
                    if (i < list.Count - 1)
                    {
                        key += " + ";
                    }
                }
            }

            readFoo
                .AddDetail("for (int i = 0; i < list.Count; i++)")
                .AddDetail("{")
                .AddDetail("    var item = list[i].Split(',');")
                .AddDetail($"    var csvClass = new {rowName}();");


            for (int i = 0; i < name.Length; i++)
            {
                var value = $"item[{i}]";
                var line = "";
                try
                {
                    line = $"    csvClass.{name[i]} = {GetTransformFunc(type[i], value)};";
                }
                catch (Exception e)
                {
                    e.Exception();
                    line = $"    csvClass.{name[i]} = default;";
                }

                readFoo.AddDetail(line);
            }

            readFoo
                .AddDetail($"    CustomerRowHandler({key}, csvClass);")
                .AddDetail("}")
                .AddDetail("callback?.Invoke();");

            var dictProp =
                genClass
                    .AddProperty("_dict", $"Dictionary<string, {rowName}>")
                    .SetSet(false)
                    .SetGet(false)
                    .SetValue($"new Dictionary<string, {rowName}>()");
            dictProp.Modifier = GenBasic.GenBasic.modifier.Protected;

            var pathProp =
                genClass
                    .AddProperty("Path", "string")
                    .SetSet(true)
                    .SetGet(true)
                    .SetValue($"\"DynamicCsv/{baseName}.csv\"")
                    .SetOverrider(true);
            pathProp.Modifier = GenBasic.GenBasic.modifier.Protected;

            genClass
                .AddFoo("GetRowByKey", rowName)
                .AddDetail("_dict.TryGetValue(key, out var value);")
                .AddDetail("return value;")
                .AddParam("string", "key");

            genClass
                .AddFoo("GetRowByKey", rowName)
                .AddDetail("var key = id.ToString();")
                .AddDetail("_dict.TryGetValue(key, out var value);")
                .AddDetail("return value;")
                .AddParam("int", "id");

            genClass
                .AddFoo("CustomerRowHandler", "void")
                .SetVirtual(true)
                .AddDetail("_dict.Add(key, row);")
                .AddParam("string", "key")
                .AddParam(rowName, "row");

            return genClass;
        }
    }
}