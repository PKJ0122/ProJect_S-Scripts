using System;
using System.Collections.Generic;


public class ItemsManager : SingletonMonoBase<ItemsManager>
{
    public int totalItem
    {
        get => _totalItem;
    }

    public bool isUse
    {
        get => _isUse;
        set
        { 
            _isUse = value;
            onItemUse?.Invoke(value);
        }
    }

    bool _isUse;
    int _totalItem = 0;

    public event Action<bool> onItemUse;

    Dictionary<ItemCode, IItems> _items = new Dictionary<ItemCode, IItems>();


    protected override void Awake()
    {
        base.Awake();

    }

    public void RegisterItems(IItems items)
    {
        if (_items.TryAdd(items.itemtype, items)) 
        {
            items.onCountChanged += (value =>
            {
                _totalItem += value;
            });
        }
        else
        {
            throw new Exception($"[ItemsManager] : 이미 등록된 아이템.");
        }
    }

    public IItems Get(ItemCode itemCode)
    {
        return _items[itemCode];
    }
}
