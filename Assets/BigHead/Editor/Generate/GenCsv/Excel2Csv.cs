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
using BigHead.Editor.Customer;
using BigHead.Framework.Utility.Crypto;
using BigHead.Framework.Utility.Helper;
using Excel;
using UnityEditor;

namespace BigHead.Editor.Generate.GenCsv
{
  public static class Excel2Csv
    {
        public static void Generate()
        {
            try
            {
                DeleteDynamicCsv();
                AnalysisExcel();
                AssetDatabase.Refresh();
                GC.Collect();
            }
            catch (Exception e)
            {  
                EditorUtility.ClearProgressBar();
                Console.WriteLine(e);
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
            DirectoryHelper.ClearDirectory(CustomerConfig.DynamicCsvPath);
            DirectoryHelper.ClearDirectory(CustomerConfig.ResourcesCsvPath);
        }

        private static void DeleteConfig()
        {
            var configPath = CustomerConfig.CsvConfigFullName;
            FileHelper.DeleteFile(configPath);
        }

        private static void AnalysisExcel()
        {
            var excelPath = CustomerConfig.ExcelPath;
            var dynamicCsvPath = CustomerConfig.DynamicCsvPath;
            var resourcesCsvPath = CustomerConfig.ResourcesCsvPath;
            DirectoryHelper.ForceCreateDirectory(resourcesCsvPath);

            // 获取Excel文件夹下所有文件路径
            var files = Directory.GetFiles(excelPath, "*", SearchOption.AllDirectories);
            var paths = files.Where(name => name.EndsWith(".xlsx") || name.EndsWith(".xls"))
                .Select(name => name.Replace('/', '\\')).ToArray();

            // 校验数据
            var md5List = new List<string>();
            
            
            // 正则比对名称，如果有括号则选用括号内的名称
            var regex1 = new Regex(@"\((\w+)\)");
            var regex2 = new Regex(@"\（(\w+)\）");
            
            float progress = -1;
            foreach (var path in paths)
            {
                ++progress;
                EditorUtility.DisplayProgressBar("解析Excel文件", path, progress / paths.Length);
                using(var stream = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    using(var excelReader = path.EndsWith(".xls")
                        ? ExcelReaderFactory.CreateBinaryReader(stream)
                        : ExcelReaderFactory.CreateOpenXmlReader(stream))
                    {
                        var dataSet = excelReader.AsDataSet();

                        foreach (DataTable table in dataSet.Tables)
                        {
                            // 过滤不生成的表单
                            var tableName = table.TableName.Trim();
                            if (tableName.StartsWith("#"))
                                continue;

                            var rows = table.Rows;

                            // 过滤不生成的列
                            var ungenCols = new List<int>();
                            for (int index = 0; index < table.Columns.Count; index++)
                            {
                                var value = rows[2][index].ToString().Trim();
                                if (value == "不生成" || value.StartsWith("#") || string.IsNullOrEmpty(value))
                                    ungenCols.Add(index);
                            }

                            var tableBuilder = new StringBuilder();
                            for (int row = 0; row < table.Rows.Count; row++)
                            {
                                var rowBuilder = new StringBuilder();
                                for (int col = 0; col < table.Columns.Count; col++)
                                {
                                    if (ungenCols.Contains(col))
                                        continue;

                                    var grid = rows[row][col].ToString()
                                        .Replace('\n', '|')
                                        .Replace(",", "、");
                                    
                                    rowBuilder.Append(grid);
                                    
                                    var builder = col != table.Columns.Count - 1
                                        ? rowBuilder.Append(",") : 
                                        row == table.Rows.Count - 1
                                            ? rowBuilder.Append("")  
                                            : rowBuilder.Append(Environment.NewLine);

                                }
                                
                                var rowArray = rowBuilder.ToString().Split(',');
                                if (!Array.TrueForAll(rowArray, string.IsNullOrEmpty) && 
                                    !Array.TrueForAll(rowArray, string.IsNullOrWhiteSpace))
                                {
                                    tableBuilder.Append(rowBuilder);
                                }

                                if (regex1.IsMatch(tableName))
                                    tableName = regex1.Match(tableName).Value.TrimStart('(').TrimEnd(')');

                                if (regex2.IsMatch(tableName))
                                    tableName = regex2.Match(tableName).Value.TrimStart('（').TrimEnd('）');
                                
                                if (row == 2)
                                {
                                    md5List.Add(tableName + "@" + BigHeadCrypto.MD5Encode(tableBuilder.ToString()));
                                }
                            }

                            var str = tableBuilder.ToString().TrimEnd('\r', '\n');
                            var data = Encoding.UTF8.GetBytes(str);
                            using (var fileStream =
                                new FileStream($"{dynamicCsvPath}/{tableName}.csv", FileMode.Create))
                            {
                                fileStream.Write(data, 0, data.Length);
                            }
                            
                            if(CustomerConfig.GenerateCsvInResources)
                                using (var fileStream =
                                    new FileStream($"{resourcesCsvPath}/{tableName}.csv", FileMode.Create))
                                {
                                    fileStream.Write(data, 0, data.Length);
                                }
                        }
                    }
                }
            }

            var datas = new List<string>();

            // 读取生成存储信息
            var fullName = CustomerConfig.CsvConfigFullName;
            if (File.Exists(fullName))
            {
                using (var stream = new StreamReader(fullName))
                {
                    var line = "";
                    while ((line = stream.ReadLine()) != null)
                    {
                        datas.Add(line);
                    }
                }
            }

            var news = md5List.Except(datas).ToArray();
            var olds = datas.Except(md5List).ToArray();

            var count = 0f;
            // 先删除旧的生成脚本
            foreach (var item in olds)
            {
                var path = CustomerConfig.GenerateCsPath + item.Split('@')[0];
                var rowPath = path + "Row.cs";
                var csvPath = path + "Csv.cs";
                FileHelper.DeleteFile(rowPath);
                FileHelper.DeleteFile(csvPath);
                
                ++count;
                EditorUtility.DisplayProgressBar("正在删除旧的脚本文件", csvPath, count / olds.Length);
            }

            count = 0f;
            // 增加新的生成脚本
            foreach (var item in news)
            {
                var path = CustomerConfig.DynamicCsvPath + "/" + item.Split('@')[0] + ".csv";
                Csv2Cs.GenerateCs(path);
                
                ++count;
                EditorUtility.DisplayProgressBar("正在生成新的脚本文件", path, count / news.Length);
            }
            // 生成最新的配置文件
            var bytes = Encoding.UTF8.GetBytes(string.Join(Environment.NewLine, md5List));
            var configFullName = CustomerConfig.CsvConfigFullName;
            if (File.Exists(configFullName))
                File.Delete(configFullName);

            var configPath = CustomerConfig.ConfigPath;
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