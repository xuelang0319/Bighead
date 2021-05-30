//
// = The script is part of BigHead and the framework is individually owned by Eric Lee.
// = Cannot be commercially used without the authorization.
//
//  Author  |  UpdateTime     |   Desc  
//  Eric    |  2021年5月30日   |   Panel管理器
//

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public abstract class BasePanelManager<T> : MonoGlobalSingleton<T> where T : BasePanelManager<T>
{
    /// <summary> 用来储存已实例化的Panel,如果没有则加载并实例 </summary>
    public Dictionary<string, IPanel> _panels = new Dictionary<string, IPanel>();

    /// <summary> Panel堆，用来存储目前场景的Stack </summary>
    public Stack<IPanel> _stack = new Stack<IPanel>();

    /// <summary> 全局唯一Canvas </summary>
    public Canvas _canvas;

    /// <summary> 全局唯一事件系统 </summary>
    public EventSystem _eventSystem;

    public virtual void Awake()
    {
        _canvas = FindObjectOfType<Canvas>();
        if (!_canvas) _canvas = GetCanvas();
    }

    public virtual void Push(string name, bool peekPause = true, string message = "")
    {
        GetPanel(name, panel =>
        {
            if (_stack.Count > 0)
            {
                var lastPanel = _stack.Peek();
                if (lastPanel == panel) return;
                if (peekPause) lastPanel.OnPause();
            }

            panel.OnEnter();
            panel.OnMessage(message);
            _stack.Push(panel);
        });
    }

    public virtual bool Pop()
    {
        if (_stack.Count == 0)
            return false;

        _stack.Pop().OnExist();

        if (_stack.Count != 0)
            _stack.Peek().OnResume();
        return true;
    }

    public virtual void Only(string name)
    {
        GetPanel(name, panel =>
        {
            panel.OnEnter();
            while (_stack.Count > 0)
            {
                var p = _stack.Pop();
                p.OnExist();
            }

            _stack.Push(panel);
        });
    }

    public IPanel Peek() =>
        _stack.Peek();

    public abstract void GetPanel(string name, Action<IPanel> callback);

    public bool IsTouchOnUI()
    {
#if UNITY_ANDROID && UNITY_IPHONE
            return Input.touchCount > 0 
                   && Input.GetTouch(0).phase == TouchPhase.Began 
                   && _eventSystem.IsPointerOverGameObject();
#else
        return _eventSystem.IsPointerOverGameObject();
#endif
    }

    private Canvas GetCanvas()
    {
        var canvas = new GameObject("Canvas").AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceCamera;
        canvas.worldCamera = Camera.main;
        canvas.sortingLayerName = "UI";
        canvas.sortingOrder = (int) UIOrder.MainCanvas;

        var scaler = canvas.gameObject.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(BigheadConfig.DesignWidth, BigheadConfig.DesignHeight);
        scaler.matchWidthOrHeight = BigheadConfig.CanvasMatch;
        canvas.gameObject.AddComponent<GraphicRaycaster>();
        DontDestroyOnLoad(canvas);

        _eventSystem = FindObjectOfType<EventSystem>();
        if (_eventSystem)
            return canvas;

        _eventSystem = new GameObject("EventSystem").AddComponent<EventSystem>();
        _eventSystem.gameObject.AddComponent<StandaloneInputModule>();

        DontDestroyOnLoad(_eventSystem.gameObject);
        return canvas;
    }
}