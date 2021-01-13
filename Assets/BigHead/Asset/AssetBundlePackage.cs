//
// = The script is part of BigHead and the framework is individually owned by Eric Lee.
// = Cannot be commercially used without the authorization.
//
//  Author  |  UpdateTime     |   Desc  
//  Eric    |  2021年1月13日   |   打包配置
//

using UnityEngine;

namespace BigHead.Asset
{
    public class AssetBundlePackage : AssetBase<AssetBundlePackage>
    {
        [SerializeField]
        [Tooltip("在当前Application.dataPath下的相对路径")]
        public string ResPath = "GameData";
    }
}