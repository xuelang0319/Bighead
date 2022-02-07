//
// = The script is part of BigHead and the framework is individually owned by Eric Lee.
// = Cannot be commercially used without the authorization.
//
//  Author  |  UpdateTime     |   Desc  
//  Eric    |  2020年12月17日  |   Mono全局单例， 由于Unity以单线程运行，所以无需进行线程锁或其他方式优化。
//

using UnityEngine;

public abstract class MonoGlobalSingleton<T> : MonoBehaviour where T : MonoGlobalSingleton<T>
{
    private static T _instance;

    private static GameObject _object;

    public static T Instance
    {
        get
        {
            if (!_object)
            {
                _object = new GameObject($"Singleton{typeof(T).Name}");
                DontDestroyOnLoad(_object);
                _instance = _object.AddComponent<T>();
            }

            return _instance;
        }
    }
}