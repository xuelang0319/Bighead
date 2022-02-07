//
// = The script is part of BigHead and the framework is individually owned by Eric Lee.
// = Cannot be commercially used without the authorization.
//
//  Author  |  UpdateTime     |   Desc  
//  Eric    |  2021年5月30日   |   世界画布
//

using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class WorldCanvas: MonoGlobalSingleton<WorldCanvas>
{
    /// <summary> 世界画布 </summary>
    private Canvas _canvas;
    /// <summary> 全局唯一事件系统 </summary>
    private EventSystem _eventSystem;

    private HashSet<GameObject> _objects = new HashSet<GameObject>();
        
    private void Awake()
    {
        _canvas = GetCanvas();
    }

    public void Push(GameObject obj, Vector3 worldPos)
    {
        obj.transform.SetParent(_canvas.transform, false);
        obj.transform.position = worldPos;
        _objects.Add(obj);
    }

    public void SetActive(bool active)
    {
        enabled = active;
    }

    public void Clear()
    {
        var array = _objects.ToArray();
        _objects.Clear();
        for (int i = 0; i < array.Length; i++)
            Destroy(array[i]);
    }

    private Canvas GetCanvas()
    {
        var canvas = new GameObject("Canvas").AddComponent<Canvas>();
        var scaler = canvas.gameObject.AddComponent<CanvasScaler>();
        canvas.gameObject.AddComponent<GraphicRaycaster>();
            
        canvas.renderMode = RenderMode.WorldSpace;
        canvas.worldCamera = Camera.main;
        var transform1 = canvas.transform;
        transform1.position = Vector3.zero;
        transform1.localScale = new Vector3(0.01f, 0.01f, 0.01f);
        canvas.sortingOrder = (int) UIOrder.WorldCanvas;

        _eventSystem = FindObjectOfType<EventSystem>();
        if (_eventSystem) 
            return canvas;
            
        _eventSystem = new GameObject("EventSystem").AddComponent<EventSystem>();
        _eventSystem.gameObject.AddComponent<StandaloneInputModule>();
            
        return canvas;
    }
}