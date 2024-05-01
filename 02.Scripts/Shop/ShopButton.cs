using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopButton : MonoBehaviour
{
    public string itemName
    {
        set => _name.text = value;
    }
    public string itemPrice
    {
        get => _price.text;
        set => _price.text = value;
    }
    public Button button
    {
        get => _button;
    }

    Button _button;
    TMP_Text _name;
    TMP_Text _price;

    private void Awake()
    {
        _button = GetComponent<Button>();
        _name = transform.Find("Text (TMP) - ItemName").GetComponent<TMP_Text>();
        _price = transform.Find("Text (TMP) - ItemPrice").GetComponent<TMP_Text>();
    }
}
