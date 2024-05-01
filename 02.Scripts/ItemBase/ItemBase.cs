using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemBase : MonoBehaviour, IItems
{
    [field: SerializeField] public string ItemName { get; set; }
    [field: SerializeField] public string Discription { get; set; }

    public int Count 
    { 
        get => _count;
        set 
        { 
            _count = value;
            onCountChanged?.Invoke(value);
        }
    }
    public ItemCode itemtype
    {
        get => _itemtype;
        set
        {
            _itemtype = value;
        }
    }

    private ItemCode _itemtype;
    private Button _button;
    private TMP_Text _countText;
    private int _count = 0;

    protected UIKeyInfo _key;
    public event Action<int> onCountChanged;

    protected virtual void Awake()
    {
        ItemsManager.instance.RegisterItems(this);
        _button = GetComponent<Button>();
        _countText = transform.Find("Text (TMP) - Count").GetComponent<TMP_Text>();
        _button.onClick.AddListener(() =>
        {
            if(Count > 0)
            {
                _key.Show();
                Use();
            }
        });
        _countText.text = $"{Count}";
        onCountChanged += (value =>
        {
            _countText.text = $"{value}";
        });
    }

    private void Start()
    {
        _key = UIManager.instance.Get<UIKeyInfo>();
    }

    protected virtual void Update()
    {

    }

    public virtual void Use()
    {
        Count--;
        UIManager.instance.Get<UISettingPopup>().itemOn = true;
    }
}
