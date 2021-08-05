//
// = The script is part of BigHead and the framework is individually owned by Eric Lee.
// = Cannot be commercially used without the authorization.
//
//  Author  |  UpdateTime     |   Desc  
//  Eric    |  2021年8月5日    |   TMX解析方法
//

using System;
using System.Collections.Generic;
using System.Linq;
using BigHead.Framework.Core;
using UnityEngine;

namespace BigHead.Framework.Utility.GenCsv
{
    public static class TmxAssistant
    {
        public static void GetDataInResources(
            string localPath, 
            out int width, 
            out int height,
            out Dictionary<string, int[]> layersData)
        {
            var data = Res.Instance.ResourcesLoad<TextAsset>(localPath);
            if (!data)
            {
                $"Can not find tmx data in Resources. local path : {localPath}".Exception();
                goto Failed;
            }

            var strs = data.text.Split(Environment.NewLine.ToCharArray()).Where(str => !string.IsNullOrEmpty(str)).ToArray();
            if (strs.Length < 2)
            {
                "Analysis tmx data failed. The number of data rows is not sufficient".Exception();
                goto Failed;
            }

            var firstLine = strs[0].Split(',');
            width = int.Parse(firstLine[0]);
            height = int.Parse(firstLine[1]);
            layersData = new Dictionary<string, int[]>();
            for (int i = 1; i < strs.Length; i++)
            {
                var line = strs[i].Split('|');
                var key = line[0];
                var list = new List<int>(width * height);

                for (int j = 1; j < line.Length; j++)
                {
                    var pair = line[j].Split(',');
                    var num = int.Parse(pair[0]);
                    var count = int.Parse(pair[1]);
                    for (int k = 0; k < count; k++)
                    {
                        list.Add(num);
                    }
                }

                layersData.Add(key, list.ToArray());
            }

            return;
            
            Failed:
            localPath = "";
            width = 0;
            height = 0;
            layersData = null;
        }
    }
}