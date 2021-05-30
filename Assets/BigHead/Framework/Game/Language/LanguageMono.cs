//
// = The script is part of BigHead and the framework is individually owned by Eric Lee.
// = Cannot be commercially used without the authorization.
//
//  Author  |  UpdateTime     |   Desc  
//  Eric    |  2021年5月30日   |   多语言继承Mono基类（仅实现自动注册功能）
//

using UnityEngine;

public abstract class LanguageMono : MonoBehaviour, ILanguage
{
    public virtual void Awake()
    {
        LanguageHandler.Instance.Register(this);
    }

    public void OnDestroy()
    {
        LanguageHandler.Instance.UnRegister(this);
    }

    public abstract void OnLanguageChanged();
}