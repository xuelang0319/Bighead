//
// = The script is part of BigHead and the framework is individually owned by Eric Lee.
// = Cannot be commercially used without the authorization.
//
//  Author  |  UpdateTime     |   Desc  
//  Eric    |  2021年5月23日   |   Excel生成Csv文件
//

using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using BigHead.Framework.Core;
using BigHead.Framework.Extension;
using BigHead.Framework.Utility.Crypto;
using BigHead.Framework.Utility.Helper;
using BigHead.Framework.Utility.Readers;
using Excel;
using UnityEditor;
using static BigheadConfig;

namespace BigHead.Editor.Generate.GenCsv
{
  public static class Excel2Csv
    {
        public static void Generate()
        {
            try
            {
                AnalysisExcel();
                AssetDatabase.Refresh();
                GC.Collect();
            }
            catch (Exception e)
            {  
                EditorUtility.ClearProgressBar();
                e.Exception();
                ClearAll();
            }
        }

        public static void ClearAll()
        {
            DeleteDynamicCsv();
            DeleteConfig();
            Csv2Cs.ClearGenCs();
            AssetDatabase.Refresh();
        }

        private static void DeleteDynamicCsv()
        {
            DirectoryHelper.ClearDirectory(DynamicCsvPath);
        }

        private static void DeleteConfig()
        {
            var configPath = CsvConfigFullName;
            FileHelper.DeleteUnityFile(configPath);
        }

        private static void AnalysisExcel()
        {
            DirectoryHelper.ForceCreateDirectory(DynamicCsvPath);
            DirectoryHelper.ForceCreateDirectory(ConstCsvPath);

            // 上一次生成存储的MD5数据
            var oldDatas = new Dictionary<string, string>();
            // 此次生成存储的MD5数据
            var newDatas = new Dictionary<string, string>();

            // 读取生成存储信息
            var fullName = CsvConfigFullName;
            if (File.Exists(fullName))
            {
                using (var stream = new StreamReader(fullName))
                {
                    var line = "";
                    while ((line = stream.ReadLine()) != null)
                    {
                        var array = line.Split('@');
                        // 0 - 文件名或全路径， 1 - 校验码
                        oldDatas.Add(array[0], array[1]);
                    }
                }
            }

            // 获取ConstCsv文件夹下所有文件路径
            var constFiles = Directory.GetFiles(ConstCsvPath, "*", SearchOption.AllDirectories);
            var constPaths = constFiles.Where(name => name.EndsWith(".csv"))
                .Select(name => name.Replace('/', '\\')).ToList();

            // 存在ConstCsv文件
            if (constPaths.Count > 0)
            {
                foreach (var constPath in constPaths)
                {
                    var content = FileHelper.ShareReadFile(constPath);
                    var md5 = BigHeadCrypto.MD5Encode(content);
                    var fileNameWithExtension = Path.GetFileName(constPath);
                    newDatas.Add(fileNameWithExtension, md5);

                    if (oldDatas.ContainsKey(fileNameWithExtension) && 
                        Equals(oldDatas[fileNameWithExtension], md5))
                    {
                        // 数据没有变化
                        oldDatas.Remove(fileNameWithExtension);
                    }
                    else
                    {
                        // 数据发生变化
                        Csv2Cs.GenerateCs(constPath);
                        if (oldDatas.ContainsKey(fileNameWithExtension))
                            oldDatas.Remove(fileNameWithExtension);
                    }
                }
            }




            // 获取Excel文件夹下所有文件路径
            var files = Directory.GetFiles(ExcelPath, "*", SearchOption.AllDirectories);
            var paths = files
                .Where(name => name.EndsWith(".xlsx") || name.EndsWith(".xls"))
                .Where(name=> !name.Contains("~$"))
                .Select(name => name.Replace('/', '\\')).ToArray();
            
            // 发生变化的列表
            var changedFilter = new List<string>();
            
            // 对每个Excel做MD5变更校验
            foreach (var path in paths)
            {
                var content = FileHelper.ShareReadFile(path);
                var md5 = BigHeadCrypto.MD5Encode(content);

                var fileNameWithExtension = Path.GetFileName(path);
                // 添加正确数据，直接修正，但对发生了变化或新增的文件进行记录
                newDatas.Add(fileNameWithExtension, md5);

                if (oldDatas.ContainsKey(fileNameWithExtension) &&
                    Equals(oldDatas[fileNameWithExtension], md5))
                {
                    // 说明数据没有变化
                    var excelName = Path.GetFileNameWithoutExtension(fileNameWithExtension);
                    var all = oldDatas.Keys.Where(key =>
                    {
                        if (!key.StartsWith(excelName)) return false;
                        return !string.IsNullOrEmpty(Path.GetExtension(key)) || 
                               Equals(excelName.Split('$')[0], excelName);
                    }).ToList();

                    foreach (var key in all)
                    {
                        var value = oldDatas[key];
                        newDatas.AddValue(key, value);
                        oldDatas.Remove(key);
                    }

                    continue;
                }
                
                // 发生了变化
                changedFilter.Add(path);
                
                // 由于即将进行处理，所以在这里移除。
                if (oldDatas.ContainsKey(fileNameWithExtension))
                    oldDatas.Remove(fileNameWithExtension);
            }

            // 正则比对名称，如果有括号则选用括号内的名称
            var regex1 = new Regex(@"\((\w+)\)");
            var regex2 = new Regex(@"\（(\w+)\）");
            
            // 遍历发生变更或新建的Excel
            foreach (var path in changedFilter)
            {
                var excelName = Path.GetFileNameWithoutExtension(path);
                using(var stream = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    using(var excelReader = path.EndsWith(".xls")
                        ? ExcelReaderFactory.CreateBinaryReader(stream)
                        : ExcelReaderFactory.CreateOpenXmlReader(stream))
                    {
                        var dataSet = excelReader.AsDataSet();

                        float createProgress = 0;
                        var tableCount = dataSet.Tables.Count;
                        foreach (DataTable table in dataSet.Tables)
                        {
                            ++createProgress;
                            // 过滤不生成的表单
                            var tableName = table.TableName.Trim();
                            if (tableName.StartsWith("#"))
                                continue;

                            // 正则后的表单名称
                            if (regex1.IsMatch(tableName))
                                tableName = regex1.Match(tableName).Value.TrimStart('(').TrimEnd(')');

                            if (regex2.IsMatch(tableName))
                                tableName = regex2.Match(tableName).Value.TrimStart('（').TrimEnd('）');
                            
                            EditorUtility.DisplayProgressBar($"正在解析：{excelName}", tableName, createProgress / tableCount);

                            var rows = table.Rows;
                            var cols = table.Columns;

                            // 过滤不生成的列
                            var ungenCols = new List<int>();
                            // lastColIndex是为了解决当数据后几列都不生成时导致生成的数据格式错误问题。
                            var lastColIndex = 0;
                            for (int index = 0; index < cols.Count; index++)
                            {
                                var value = rows[2][index].ToString().Trim();
                                if (value == "不生成" || value.StartsWith("#") || string.IsNullOrEmpty(value))
                                    ungenCols.Add(index);
                                else lastColIndex = index;
                            }

                            var tableBuilder = new StringBuilder();
                            for (var row = 0; row < rows.Count; row++)
                            {
                                var rowBuilder = new StringBuilder();
                                for (var col = 0; col < cols.Count; col++)
                                {
                                    var isUngen = ungenCols.Contains(col);
                                    
                                    if (!isUngen)
                                    {
                                        var grid = rows[row][col].ToString()
                                            .Replace('\n', '|')
                                            .Replace(",", "、");
                                            rowBuilder.Append(grid);
                                            
                                        if (col != lastColIndex)
                                                rowBuilder.Append(",");
                                    }

                                    if (col == lastColIndex && row != rows.Count - 1)
                                        rowBuilder.AppendLine();
                                }
                                
                                var rowArray = rowBuilder.ToString().Split(',');
                                if (!Array.TrueForAll(rowArray, string.IsNullOrEmpty) && 
                                    !Array.TrueForAll(rowArray, string.IsNullOrWhiteSpace))
                                {
                                    tableBuilder.Append(rowBuilder);
                                }
                            }

                            var str = tableBuilder.ToString().TrimEnd('\r', '\n');
                            
                            // 这里进行MD5校验
                            var md5 = BigHeadCrypto.MD5Encode(str);
                            var dataName = $"{excelName}${tableName}";
                            newDatas.Add(dataName, md5);

                            var bundleDynamicPath = $"{DynamicCsvPath}/{tableName}.csv";
                            
                            // 如果进入判断说明存在，否则为新增
                            if (oldDatas.ContainsKey(dataName))
                            {
                                var value = oldDatas[dataName];
                                // 移除旧数据存储，因为这里已经进行了整体处理。
                                oldDatas.Remove(dataName);
                                
                                // 存在未改变
                                if (Equals(value, md5)) continue;

                                // 已改变，删除原来生成的数据。  -csv, -.cs
                                EditorUtility.DisplayProgressBar($"删除变更前文件： {tableName}", path, createProgress / tableCount);
                                File.Delete(bundleDynamicPath);
                                var rowPath = GenerateCsPath + tableName + "Row.cs";
                                var csvPath = GenerateCsPath + tableName + "Csv.cs";
                                File.Delete(rowPath);
                                File.Delete(csvPath);
                            }

                            var data = Encoding.UTF8.GetBytes(str);
                            using (var fileStream = new FileStream(bundleDynamicPath, FileMode.Create))
                            {
                                fileStream.Write(data, 0, data.Length);
                            }
                            
                            Csv2Cs.GenerateCs(bundleDynamicPath);
                        }
                    }
                }
            }

            var deleteProgress = 0;
            var deleteCount = oldDatas.Count;
            // 没有被移除的均是被删除的文件或Sheet，在这里要进行删除对应的Csv和.cs文件
            foreach (var key in oldDatas.Keys)
            {
                ++deleteProgress;
                EditorUtility.DisplayProgressBar(  $"正在删除旧数据:", key, deleteProgress / deleteCount);
                var array = key.Split('$');

                // 这是Excel文件，无需处理。
                if (Equals(array.Length, 1) && !array[0].EndsWith(".csv"))
                    continue;

                var csvName = Equals(array.Length, 1)
                    ? Path.GetFileNameWithoutExtension(array[0]) 
                    : array[1];
                
                var bundlePath = Equals(array.Length, 1)
                    ? $"{ConstCsvPath}/{csvName}.csv"
                    : $"{DynamicCsvPath}/{csvName}.csv";
                
                File.Delete(bundlePath);

                // 删除生成的代码
                var rowPath = GenerateCsPath + csvName + "Row.cs";
                var csvPath = GenerateCsPath + csvName + "Csv.cs";
                File.Delete(rowPath);
                File.Delete(csvPath);
            }


            var strs = newDatas.Select(kv => $"{kv.Key}@{kv.Value}").ToArray();
            var info = string.Join(Environment.NewLine, strs);
            // 生成最新的配置文件
            var bytes = Encoding.UTF8.GetBytes(info);
            var configFullName = CsvConfigFullName;
            if (File.Exists(configFullName))
                File.Delete(configFullName);

            var configPath = ConfigPath;
            if (!Directory.Exists(configPath))
                Directory.CreateDirectory(configPath);
            using (var fileStream = new FileStream(configFullName, FileMode.Create))
            {
                fileStream.Write(bytes, 0, bytes.Length);
            }

            EditorUtility.ClearProgressBar();
        }
    }
}