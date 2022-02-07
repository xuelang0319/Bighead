//
// = The script is part of BigHead and the framework is individually owned by Eric Lee.
// = Cannot be commercially used without the authorization.
//
//  Author  |  UpdateTime     |   Desc  
//  Eric    |  2022年2月5日    |   Tmx配置文件
//

using UnityEngine;

namespace BigHead.Tmx
{
    public class TmxConfig
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
    
        /* ------------------
         * Tmx Generate .txt part.
         * ------------------*/
        /// <summary> Tmx 配置文件存储路径 </summary>
        public static readonly string TmxPath = GameDataPath + "TmxData";
        /// <summary> 生成的Txt文件路径 </summary>
        public static readonly string TxtPath = GameDataPath + "TxtData";
        /// <summary> tmx信息存储文件名称 </summary>
        public static readonly string TmxConfigName = "TmxConfig.bh";
        /// <summary> tmx生成信息存储 </summary>
        public static readonly string TmxConfigFullName = ConfigPath + TmxConfigName;
    
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
    }
}