//
// = The script is part of Bighead and the framework is individually owned by Eric Lee.
// = Cannot be commercially used without the authorization.
//
//  Author  |  UpdateTime     |   Desc  
//  Eric    |  2021年1月8日    |   框架管理器
//

using BigHead.Framework.Core;

namespace BigHead
{
    public class BigHeadManager : MonoSingleton<BigHeadManager>
    {
        public event Callback DestroyEvent;

        private void OnDestroy()
        {
            DestroyEvent?.Invoke();
        }
    }
}