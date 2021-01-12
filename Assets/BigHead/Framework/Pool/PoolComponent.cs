//
// = The script is part of BigHead and the framework is individually owned by Eric Lee.
// = Cannot be commercially used without the authorization.
//
//  Author  |  UpdateTime     |   Desc  
//  Eric    |  2020年12月18日  |   Mono对象池组件
//

using BigHead.Framework.Core;
using UnityEngine;

namespace BigHead.Framework.Pool
{
    public class PoolComponent : MonoBehaviour
    {
        private Callback _destroyFoo;
        
        public void Initialize(Callback destroyFoo)
        {
            _destroyFoo = destroyFoo;
        }

        private void OnDestroy()
        {
            _destroyFoo?.Invoke();
        }
    }
}