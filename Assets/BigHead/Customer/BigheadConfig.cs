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
    /// <summary> Speaker封装类打印开关，非调试期请关闭. </summary>
    public const bool Debug = true;


    /* ------------------
     * Load asset part.
     * ------------------*/
    /// <summary> 预制体资源加载 </summary>
    public const bool LoadInBundle_Prefab = false;
    /// <summary> 图片资源加载 </summary>
    public const bool LoadInBundle_Sprite = false;
    /// <summary> 配置资源加载 </summary>
    public const bool LoadInBundle_Config = false;
    /// <summary> 音效资源加载 </summary>
    public const bool LoadInBundle_Sound = false;
    
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
     * Generate .cs part.
     * ------------------*/
    /// <summary> Excel 配置文件存储路径 </summary>
    public static readonly string ExcelPath = GameDataPath + "ExcelData";
    /// <summary> 自动生成代码文件路径 </summary>
    public static readonly string GenerateCsPath = Application.dataPath + "/Scripts/GenerateCs/Csv/";
    /// <summary> 动态生成Csv配置文件存储路径 </summary>
    public static readonly string DynamicCsvPath = GameDataPath + "CsvData/DynamicCsv";
    /// <summary> 非生成Csv配置文件存储路径 </summary>
    public static readonly string ConstCsvPath = GameDataPath + "CsvData/ConstCsv";
    /// <summary> Resources配置文件路径 </summary>
    public static readonly string ResourcesCsvPath = Application.dataPath + "/Resources/DynamicCsv";
    /// <summary> 同步生成Csv文件在Resources文件夹中 </summary>
    public const bool GenerateCsvInResources = true;

        
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