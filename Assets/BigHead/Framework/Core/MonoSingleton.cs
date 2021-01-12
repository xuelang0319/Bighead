//
// = The script is part of BigHead and the framework is individually owned by Eric Lee.
// = Cannot be commercially used without the authorization.
//
//  Author  |  UpdateTime     |   Desc  
//  Eric    |  2020年12月17日  |   Mono单例， 由于Unity以单线程运行，所以无需进行线程锁或其他方式优化。
//

using UnityEngine;

namespace BigHead.Framework.Core
{
    public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
    {
        private static T _instance;

        public static T Instance
        {
            get
            {
                if (Equals(null, _instance))
                {
                    var gameObject = new GameObject($"Singleton{typeof(T).Name}");
                    DontDestroyOnLoad(gameObject);
                    _instance = gameObject.AddComponent<T>();
                }

                return _instance;
            }
        }
    }
}