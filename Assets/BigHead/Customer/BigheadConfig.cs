//
// = The script is part of BigHead and the framework is individually owned by Eric Lee.
// = Cannot be commercially used without the authorization.
//
//  Author  |  UpdateTime     |   Desc  
//  Eric    |  2021年1月8日    |   框架配置类
//

public static class BigheadConfig
{
    /// <summary> Speaker封装类打印开关，非调试期请关闭. </summary>
    public const bool Debug = true;


    /// <summary> 预制体资源加载 </summary>
    public const bool LoadInBundle_Prefab = false;

    /// <summary> 图片资源加载 </summary>
    public const bool LoadInBundle_Sprite = false;

    /// <summary> 配置资源加载 </summary>
    public const bool LoadInBundle_Config = false;

    /// <summary> 音效资源加载 </summary>
    public const bool LoadInBundle_Sound = false;
}