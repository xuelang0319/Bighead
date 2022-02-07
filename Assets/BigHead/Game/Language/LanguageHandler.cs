//
// = The script is part of BigHead and the framework is individually owned by Eric Lee.
// = Cannot be commercially used without the authorization.
//
//  Author  |  UpdateTime     |   Desc  
//  Eric    |  2021年5月30日   |
//

using System.Collections.Generic;
using BigHead.Framework.Game;

public class LanguageHandler : MonoGlobalSingleton<LanguageHandler>
{
    private readonly List<ILanguage> _languages = new List<ILanguage>();
        
    public void Awake()
    {
        GameListener.Instance.Add(SystemListenerKeys.Language,OnLanguageChanged);
    }

    /// <summary>
    /// 注册方法
    /// </summary>
    public void Register(ILanguage item)
    {
        _languages.AddValueAndLogError(item);
    }

    /// <summary>
    /// 注销方法
    /// </summary>
    public void UnRegister(ILanguage item)
    {
        _languages.Remove(item);
    }

    public void OnLanguageChanged(object[] str)
    {
        foreach (var language in _languages)
        {
            language.OnLanguageChanged();
        }
    }

    public void OnDestroy()
    {
        GameListener.Instance.Remove(SystemListenerKeys.Language,OnLanguageChanged);
    }
}