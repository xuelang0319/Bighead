//
// = The script is part of BigHead and the framework is individually owned by Eric Lee.
// = Cannot be commercially used without the authorization.
//
//  Author  |  UpdateTime     |   Desc  
//  Eric    |  2020年12月18日  |   Mono对象池组件
//

using System;
using UnityEngine;

namespace BigHead.Framework.Pool
{
    public class PoolComponent : MonoBehaviour
    {
        private Action _destroyFoo;
        
        public void Initialize(Action destroyFoo)
        {
            _destroyFoo = destroyFoo;
        }

        private void OnDestroy()
        {
            _destroyFoo?.Invoke();
        }
    }
}