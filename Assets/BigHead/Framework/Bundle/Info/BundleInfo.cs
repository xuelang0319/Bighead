//
// = The script is part of BigHead and the framework is individually owned by Eric Lee.
// = Cannot be commercially used without the authorization.
//
//  Author  |  UpdateTime     |   Desc  
//  Eric    |  2021年1月13日   |   包信息
//

using System;

namespace BigHead.Framework.Bundle.Info
{
    [Serializable]
    public class BundleInfo
    {
        public string Path { get; set; }

        public int Version { get; set; }

        public string CheckCode { get; set; }
    }
}