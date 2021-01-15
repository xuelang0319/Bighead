//
// = The script is part of BigHead and the framework is individually owned by Eric Lee.
// = Cannot be commercially used without the authorization.
//
//  Author  |  UpdateTime     |   Desc  
//  Eric    |  2021年1月15日   |   AssetInfo及BundleInfo总表
//

using System.Collections.Generic;

namespace BigHead.Framework.Bundle.Info
{
    public class ABTable
    {
        public int Version { get; set; }
        public List<AssetInfo> AssetsInfo { get; set; }
        public List<BundleInfo> BundleInfos { get; set; }
    }
}