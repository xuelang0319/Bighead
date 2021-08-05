//
// = The script is part of BigHead and the framework is individually owned by Eric Lee.
// = Cannot be commercially used without the authorization.
//
//  Author  |  UpdateTime     |   Desc  
//  Eric    |  2021年8月4日    |   Tmx生成Txt文件
//

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using BigHead.Framework.Core;
using BigHead.Framework.Utility.Crypto;
using BigHead.Framework.Utility.Helper;
using UnityEditor;
using UnityEngine;
using static BigheadConfig;

namespace BigHead.Editor.Generate.GenTmx
{
    /*
     * Txt生成格式：（目前不能解析无限Tmx格式数据）
     *
     * 第一行为宽、高，用 ‘，’ 分割， 即：width, height
     * 例： 100,120
     *
     * 其它行为层级名称和具体数据，层级名称和数据使用 '|' 分割，数据遵循Tmx格式分割。 层级名称|数据
     * 例： layerName|0,0,0,0,0,0
     */
    
    public static class Tmx2Txt
    {
        public static void ClearAll()
        {
            DeleteConfig();
            DeleteTxt();
            AssetDatabase.Refresh();
        }

        private static void DeleteTxt()
        {
            var path = TxtPath;
            DirectoryHelper.ClearDirectory(path);
            FileHelper.DeleteMeta(path);
        }
        
        private static void DeleteConfig()
        {
            var configPath = TmxConfigFullName;
            FileHelper.DeleteUnityFile(configPath);
        }
        
        public static void Generate()
        {
            try
            {
                AnalysisTmx(TmxPath, ConfigPath, TmxConfigName, TmxOldReplace, TmxNewReplace);
                AssetDatabase.Refresh();
                GC.Collect();
            }
            catch (Exception e)
            {  
                EditorUtility.ClearProgressBar();
                e.Exception();
                AssetDatabase.Refresh();
                GC.Collect();
                ClearAll();
            }
        }

        public static void AnalysisTmx(string tmxPath, string configPath, string configName, string tmxOldReplace, string tmxNewReplace)
        {
            DirectoryHelper.ForceCreateDirectory(tmxPath);
            
            var tmxFiles = Directory.GetFiles(tmxPath, "*", SearchOption.AllDirectories);
            var tmxPaths = tmxFiles.Where(name => name.EndsWith(".tmx"))
                .Select(name => name.Replace('/', '\\')).ToList();

            // 上一次生成存储的MD5数据
            var oldDatas = new Dictionary<string, string>();
            // 此次生成存储的MD5数据
            var newDatas = new Dictionary<string, string>();

            // 读取生成存储信息
            var configFullname = configPath.TrimEnd('/').TrimEnd('\\') + @"\" + configName;
            if (File.Exists(configFullname))
            {
                using (var stream = new StreamReader(configFullname))
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

            var pathsLength = 0;
            var totalLength = tmxPaths.Count;
            foreach (var path in tmxPaths)
            {
                ++pathsLength;
                EditorUtility.DisplayProgressBar("Processing", path, pathsLength / (float)totalLength);
                
                var fileInfo = new FileInfo(path);
                var content = fileInfo.ReadFile();

                tmxPath = tmxPath.Replace('/', '\\');
                var localPath = path.Replace('/', '\\').Replace(tmxPath, "");
                var contentMd5 = BigHeadCrypto.MD5Encode(content);

                // 添加正确数据，直接修正，但对发生了变化或新增的文件进行记录
                newDatas.Add(localPath, contentMd5);

                // 说明曾经存在文件
                if (oldDatas.ContainsKey(localPath))
                {
                    var oldMd5 = oldDatas[localPath];
                    
                    // 删除已经处理的数据
                    oldDatas.Remove(localPath);
                    
                    // 说明数据没有变化
                    if (Equals(oldMd5, contentMd5))
                        continue;

                    // 说明曾经存在，但数据出现了变化，需要将原数据删除
                    FileHelper.DeleteUnityFile(path);
                }
                
                ReadTmx(content, out var width, out var height, out var firstIds, out var layers);
                StringBuilder fileBuilder = new StringBuilder();
                fileBuilder.Append(width).Append(",").Append(height).AppendLine();

                var count = 0;
                foreach (var layer in layers)
                {
                    var dataBuilder = new StringBuilder();
                    var value = layer.Value;
                    for (int i = 0; i < value.Length; i++)
                    {
                        var realId = Equals(value[i], 0) ? 0 : GetRealId(value[i], firstIds);
                        dataBuilder.Append(realId).Append(",");
                    }

                    fileBuilder.Append(layer.Key).Append("|").Append(dataBuilder.ToString().TrimEnd(','));

                    ++count;
                    if (!Equals(count, layers.Count)) fileBuilder.AppendLine();
                }

                var replacePath = path
                    .Replace(tmxOldReplace, tmxNewReplace)
                    .Replace(".tmx", ".txt");
                
                DirectoryHelper.ForceCreateDirectory(Path.GetDirectoryName(replacePath));
                var data = Encoding.UTF8.GetBytes(fileBuilder.ToString());
                using (var fileStream = new FileStream(replacePath, FileMode.Create))
                {
                    fileStream.Write(data, 0, data.Length);
                }
            }
            
            EditorUtility.DisplayProgressBar("Processing", "处理配置文件", 1);
            var strs = newDatas.Select(kv => $"{kv.Key}@{kv.Value}").ToArray();
            var info = string.Join(Environment.NewLine, strs);
            
            var bytes = Encoding.UTF8.GetBytes(info);
            
            // 删除旧的配置文件
            if (File.Exists(configFullname))
                FileHelper.DeleteUnityFile(configFullname);

            // 生成新的配置文件
            
            if (!Directory.Exists(configPath))
                Directory.CreateDirectory(configPath);
            
            using (var fileStream = new FileStream(configFullname, FileMode.Create))
                fileStream.Write(bytes, 0, bytes.Length);
            
            // 删除废弃的文件
            EditorUtility.DisplayProgressBar("Processing", "删除废弃文件", 1);

            var headPath = tmxPath.TrimEnd('/').TrimEnd('\\') + @"\";
            tmxOldReplace = tmxOldReplace.Replace('/', '\\');
            tmxNewReplace = tmxNewReplace.Replace('/', '\\');
            headPath = headPath.Replace(tmxOldReplace, tmxNewReplace);
            foreach (var localPath in oldDatas.Keys)
            {
                var path = headPath + localPath.TrimStart('\\').Replace(".tmx",".txt");
                FileHelper.DeleteUnityFile(path);
            }
            
            EditorUtility.ClearProgressBar();
        }  
        
        private static int GetRealId(int tmxId, int[] firstIds)
        {
            for (var i = 0; i < firstIds.Length; i++)
                if (tmxId > firstIds[i]) return tmxId - firstIds[i];

            $"There is something wrong about get tmx real id. param id is {tmxId}, first ids : {string.Join(",", firstIds)}".Exception();
            return 0;
        }
        
        private static void ReadTmx(string info, out int width, out int height, out int[] firstIds, out Dictionary<string, int[]> layers)
        {
            // 读取TXM的数据
            var xml = new XmlDocument();
            xml.LoadXml(info);
        
            var mapNode = xml.SelectSingleNode("map").Attributes;
            int.TryParse(mapNode["width"].Value, out width);
            int.TryParse(mapNode["height"].Value, out height);
        
            var firstIdNodes = xml.SelectNodes("map/tileset");
            var idList = (from XmlNode firstIdNode in firstIdNodes 
                select firstIdNode.Attributes["firstgid"].Value into value 
                select int.Parse(value)).ToList();
            idList.Sort((id1, id2) => id2 - id1);
            firstIds = idList.ToArray();

            layers = new Dictionary<string, int[]>();
            var layerNodes = xml.SelectNodes("map/layer");
            foreach (XmlNode layer in layerNodes)
            {
                var attributes = layer.Attributes;
                var data = layer.SelectSingleNode("data").InnerText;
                try
                {
                    layers.Add(attributes["name"].Value, Array.ConvertAll(data.Split(','), int.Parse));
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                    Debug.LogError($"Name : {attributes["name"].Value}");
                    Debug.LogError($"Data : {data}");
                }
            }
        }
    }
}