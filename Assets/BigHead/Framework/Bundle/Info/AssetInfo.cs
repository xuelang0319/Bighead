//
// = The script is part of BigHead and the framework is individually owned by Eric Lee.
// = Cannot be commercially used without the authorization.
//
//  Author  |  UpdateTime     |   Desc  
//  Eric    |  2021年1月15日   |   资源信息
//

using System;
using System.Collections.Generic;

namespace BigHead.Framework.Bundle.Info
{
    [Serializable]
    public class AssetInfo
    {
        public string AssetName { get; set; }
        public string Path { get; set; }
        public string Extension { get; set; }
        public string BundleName { get; set; }
        public List<string> Depends { get; set; }
    }
}