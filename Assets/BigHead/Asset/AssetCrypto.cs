//
// = The script is part of BigHead and the framework is individually owned by Eric Lee.
// = Cannot be commercially used without the authorization.
//
//  Author  |  UpdateTime     |   Desc  
//  Eric    |  2020年12月17日  |   Crypto Salt.
//

using UnityEngine;

namespace BigHead.Asset
{
    public class AssetCrypto : AssetBase<AssetCrypto>
    {
        [SerializeField] public string MD5PreSalt = "PreSalt";
        [SerializeField] public string MD5PostSalt = "PostSalt";
    }
}