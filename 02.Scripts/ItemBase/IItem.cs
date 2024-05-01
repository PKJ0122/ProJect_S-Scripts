using System;

public interface IItems
{
    ItemCode itemtype { get; set; }

    int Count { get; set; }

    public event Action<int> onCountChanged;

    void Use();
} 