//
// = The script is part of BigHead and the framework is individually owned by Eric Lee.
// = Cannot be commercially used without the authorization.
//
//  Author  |  UpdateTime     |   Desc  
//  Eric    |  2021年5月30日   |   Panel
//

using UnityEngine;
using UnityEngine.UI;

public interface IPanel
{
    void OnMessage(string str);
    void OnPause();
    void OnResume();
    void OnEnter();
    void OnExist();
}

public abstract class BasePanel<T> : MonoGlobalSingleton<T>, IPanel where T : BasePanel<T>
{
    protected Canvas _canvas;
    private CanvasGroup _canvasGroup;

    protected void Awake()
    {
        _canvas = GetComponentInParent<Canvas>();
        _canvasGroup = gameObject.AddComponent<CanvasGroup>();
        var image = GetComponent<Image>();
        if (image)
        {
            image.sprite = null;
            image.color = Color.white;
        }
        OnAwake();
    }

    protected abstract void OnAwake();

    public virtual void OnPause() =>
        _canvasGroup.blocksRaycasts = false;

    public virtual void OnMessage(string str)
    {
        
    }

    public virtual void OnResume() => 
        _canvasGroup.blocksRaycasts = true;

    public virtual void OnEnter()
    {
        OnResume();
        gameObject.SetActive(true);
        transform.SetAsLastSibling();
    }

    public virtual void OnExist() =>
        gameObject.SetActive(false);


    /// <summary>
    /// Get game object view path.
    /// </summary>
    public string GetPath()
    {
        var t = transform;
        var str = "";
        while (t)
        {
            str = "/" + t.name + str;
            t = t.parent;
        }

        return str.TrimStart('/');
    }

    public string GetCanvasName() => 
        _canvas.name;

}