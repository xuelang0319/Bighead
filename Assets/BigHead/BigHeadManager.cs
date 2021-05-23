//
// = The script is part of Bighead and the framework is individually owned by Eric Lee.
// = Cannot be commercially used without the authorization.
//
//  Author  |  UpdateTime     |   Desc  
//  Eric    |  2021年1月8日    |   框架管理器
//

using System;

namespace BigHead
{
    public class BigHeadManager : MonoGlobalSingleton<BigHeadManager>
    {
        public event Action DestroyEvent;

        private void OnDestroy()
        {
            DestroyEvent?.Invoke();
        }
    }
}