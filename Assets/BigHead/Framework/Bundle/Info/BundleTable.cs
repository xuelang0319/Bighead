//
// = The script is part of BigHead and the framework is individually owned by Eric Lee.
// = Cannot be commercially used without the authorization.
//
//  Author  |  UpdateTime     |   Desc  
//  Eric    |  2021年1月13日   |   打包汇总表
//

using System;
using System.Collections.Generic;

namespace BigHead.Framework.Bundle.Info
{
    [Serializable]
    public class BundleTable
    {
        public List<BundleInfo> BundleInfos { get; set; }
    }
}