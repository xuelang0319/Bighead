//
// = The script is part of BigHead and the framework is individually owned by Eric Lee.
// = Cannot be commercially used without the authorization.
//
//  Author  |  UpdateTime     |   Desc  
//  Eric    |  2021年5月23日   |   场景Mono单例
//

using UnityEngine;

public class MonoSceneSingleton<T> : MonoBehaviour where T : MonoSceneSingleton<T>
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
                _instance = _object.AddComponent<T>();
            }

            return _instance;
        }
    }
}