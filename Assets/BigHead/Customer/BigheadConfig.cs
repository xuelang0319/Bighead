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
    public static readonly string CsvConfigFullName = ConfigPath + "CsvConfig.bh";
    /// <summary> tmx信息存储文件名称 </summary>
    public static readonly string TmxConfigName = "TmxConfig.bh";
    /// <summary> tmx生成信息存储 </summary>
    public static readonly string TmxConfigFullName = ConfigPath + TmxConfigName;
        
        
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
     * Tmx Generate .txt part.
     * ------------------*/
    /// <summary> Tmx 配置文件存储路径 </summary>
    public static readonly string TmxPath = GameDataPath + "TmxData";
    /// <summary> 生成的Txt文件路径 </summary>
    public static readonly string TxtPath = GameDataPath + "TxtData";
    
    /*
     * 这里的设置类似于：
     * 源文件Tmx路径： C:/xxx/xxx/Assets/Editor/GameData/TmxData/1/2/3/xxxx.tmx;
     * 生成后Txt路径： C:/xxx/xxx/Assets/Resources/GameData/Gen/TmxData/1/2/3/xxxx.txt;
     * 则 ~ 
     * TmxOldReplace为：Editor/GameData
     * TmxNewReplace为：Resources/GameData/Gen
     */
    /// <summary> 相对于TmxPath, TxtPath需要删除的旧路径 </summary>
    public static readonly string TmxOldReplace = "TmxData";
    /// <summary> 相对于TmxPath, TxtPath需要填写的新路径 </summary>
    public static readonly string TmxNewReplace = "TxtData";
    
    /* ------------------
     * Curve.csv part.
     * ------------------*/
    /// <summary> 曲线Csv配置文件名称 </summary>
    public static readonly string CurveCsvName = "Curve.csv";
    
    /// <summary> 曲线Csv配置文件路径 </summary>
    public static readonly string CurveCsvPath = ConstCsvPath + "/" + CurveCsvName;

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