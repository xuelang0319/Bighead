//
// = The script is part of BigHead and the framework is individually owned by Eric Lee.
// = Cannot be commercially used without the authorization.
//
//  Author  |  UpdateTime     |   Desc  
//  Eric    |  2021年1月8日    |   框架配置类
//

using UnityEngine;

public static class BigheadConfig
{
    /* ------------------
     * Game data part.
     * ------------------*/
    /// <summary> 游戏数据管理路径 </summary>
    private static readonly string GameDataPath = Application.dataPath + "/Editor/GameData/";
    
        
    /* ------------------
     * Configs directory full path.
     * ------------------*/
    /// <summary> 配置信息管理路径 </summary>
    public static readonly string ConfigPath = GameDataPath + "Configs/";
    /// <summary> csv生成信息存储 </summary>
    public static readonly string CsvConfigFullName = ConfigPath + "CsvConfig";
        
        
    /* ------------------
     * Csv Generate .cs part.
     * ------------------*/
    /// <summary> Excel 配置文件存储路径 </summary>
    public static readonly string ExcelPath = GameDataPath + "ExcelData";
    /// <summary> 自动生成代码文件路径 </summary>
    public static readonly string GenerateCsPath = Application.dataPath + "/Scripts/GenerateCs/Csv/";
    /// <summary> 动态生成Csv的路径 </summary>
    public static readonly string DynamicDirectory = "CsvData/DynamicCsv";
    /// <summary> 非动态生成Csv的路径 </summary>
    public static readonly string ConstDirectory = "CsvData/ConstCsv";
    /// <summary> 动态生成Csv配置文件存储路径 </summary>
    public static readonly string DynamicCsvPath = GameDataPath + DynamicDirectory;
    /// <summary> 非生成Csv配置文件存储路径 </summary>
    public static readonly string ConstCsvPath = GameDataPath + ConstDirectory;

        
    /* ------------------
     * App part
     * ------------------*/
    /// <summary> 设计宽 </summary>
    public const int DesignWidth = 1334;
    /// <summary> 设计高 </summary>
    public const int DesignHeight = 750;
    /// <summary> 宽高适配， 0 - 宽适配， 1 - 高适配 </summary>
    public const float CanvasMatch = 0;
}