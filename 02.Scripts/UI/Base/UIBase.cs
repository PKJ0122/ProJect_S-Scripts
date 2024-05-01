using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


[RequireComponent(typeof(Canvas))]
public abstract class UIBase : MonoBehaviour, IUI
{
    public int sortingOrder
    {
        get => _canvas.sortingOrder;
        set => _canvas.sortingOrder = value;
    }

    public bool inputActionEnable
    {
        get => _inputActionEnable;
        set
        {
            if (_inputActionEnable == value)
                return;
            _inputActionEnable = value;
            onInputActionEnableChanged?.Invoke(value);
        }
    }

    public event Action<bool> onInputActionEnableChanged;

    private bool _inputActionEnable;
    protected Canvas _canvas;
    protected GraphicRaycaster _module;
    private EventSystem _eventSystem;


    protected virtual void Awake()
    {
        _canvas = GetComponent<Canvas>();
        _module = GetComponent<GraphicRaycaster>();
        _eventSystem = EventSystem.current;
    }

    public virtual void Show()
    {
        _canvas.enabled = true;
    }

    public virtual void Hide()
    {
        _canvas.enabled = false;
    }

    public virtual void InputAction()
    {
        
    }

    public bool Raycast(List<RaycastResult> results)
    {
        int count = results.Count;
        PointerEventData pointerEventData = new PointerEventData(_eventSystem);
        pointerEventData.position = Input.mousePosition;
        _module.Raycast(pointerEventData, results);

        return count < results.Count;
    }
}
