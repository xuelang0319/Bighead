//
// = The script is part of BigHead and the framework is individually owned by Eric Lee.
// = Cannot be commercially used without the authorization.
//
//  Author  |  UpdateTime     |   Desc  
//  Eric    |  2022年2月5日    |   曲线配置
//

using BigHead.Csv.Core;
using UnityEngine;

namespace BigHead.Csv.Curve
{
    public class CurveConfig
    {
        /* ------------------
         * Curve.csv part.
         * ------------------*/
        /// <summary> 曲线Csv配置文件名称 </summary>
        public static readonly string CurveCsvName = "Curve.csv";
    
        /// <summary> 曲线Csv配置文件路径 </summary>
        public static readonly string CurveCsvPath = CsvConfig.ConstCsvPath + "/" + CurveCsvName;
    
        /// <summary> 自动生成曲线脚本路径 </summary>
        public static readonly string GenerateCurveScriptPath = Application.dataPath + "/Scripts/GenerateCs/Curve/";

        public static readonly string GenerateCurveScriptName = "CurveAssistant.cs";
        
    }
}