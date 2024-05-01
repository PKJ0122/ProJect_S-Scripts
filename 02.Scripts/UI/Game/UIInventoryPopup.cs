using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIInventoryPopup : UIScreensBase
{
    public bool isShow
    {
        get => _isShow;

        set
        {
            _isShow = value;
            isShowEvent?.Invoke(_isShow);
        }
    }
    bool _isShow;
    Button _closeButton;

    Transform _itemDiscriptionObj;
    TMP_Text _itemName;
    TMP_Text _itemDiscription;
    PointerEventData pointerEventData;

    public event Action<bool> isShowEvent;


    protected override void Awake()
    {
        base.Awake();
        _closeButton = transform.Find("Panel/Button - Close").GetComponent<Button>();
        _closeButton.onClick.AddListener(() =>
        {
            Hide();
            UIManager.instance.Get<UITurnAction>().Show();
        });
        _itemDiscriptionObj = transform.Find("Panel - ItemDiscription").GetComponent<Transform>();
        _itemName = _itemDiscriptionObj.transform.Find("Text (TMP) - ItemName").GetComponent<TMP_Text>();
        _itemDiscription = _itemDiscriptionObj.transform.Find("Text (TMP) - ItemDiscription").GetComponent<TMP_Text>();
        pointerEventData = new PointerEventData(EventSystem.current);
    }

    public override void Show()
    {
        base.Show();
        isShow = true;
    }
    public override void Hide()
    {
        base.Hide();
        isShow = false;
    }

    public override void InputAction()
    {
        base.InputAction();
    }

    public void Update()
    {
        pointerEventData.position = Input.mousePosition;

        List<RaycastResult> results = new List<RaycastResult>();
        _module.Raycast(pointerEventData, results);

        if (results.Count > 0 && results[0].gameObject.TryGetComponent<ItemBase>(out ItemBase item))
        {
            _itemDiscriptionObj.transform.position = Input.mousePosition;
            _itemDiscriptionObj.gameObject.SetActive(true);
            _itemName.text = item.ItemName;
            _itemDiscription.text = item.Discription;
        }
        else
        {
            _itemDiscriptionObj.gameObject.SetActive(false);
        }
    }
}
