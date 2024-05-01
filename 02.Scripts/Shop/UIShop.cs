using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class UIShop : UIPopupBase
{
    public bool isShow
    {
        get => _isShow;
    }

    bool _isShow;
    ShopButton[] shopButtons;
    Button _exitButton;

    public event Action<int> onBuyItem;


    protected override void Awake()
    {
        base.Awake();
        shopButtons = GetComponentsInChildren<ShopButton>();
        _exitButton = transform.Find("Panel/Button - Exit").GetComponent<Button>();

        _exitButton.onClick.AddListener(Hide);
    }

    public override void Show()
    {
        Gambler gambler = Gambler.Get(PhotonNetwork.LocalPlayer.ActorNumber);
        foreach (ShopButton b in shopButtons)
        {           
            b.button.onClick.RemoveAllListeners();
            int[] x = RandomItems();
            b.itemName = $"{(ItemCode)x[0]}";
            b.itemPrice = $"{x[1]}";
            b.button.interactable = gambler.statistic.coin >= x[1];

            b.button.onClick.AddListener(() =>
            {
                if(gambler.statistic.coin >= x[1])
                {
                    gambler.statistic.coin -= x[1];
                    gambler.ItemGetting((ItemCode)x[0]);
                    onBuyItem?.Invoke(x[0]);
                    Hide();
                }
            });
        }
        base.Show();
        _isShow = true;
    }

    public override void Hide()
    {
        base.Hide();
        _isShow = false;
    }

    int[] RandomItems()
    {
        int randomItems = Random.Range(0, System.Enum.GetNames(typeof(ItemCode)).Length - 2);
        int randomPrice = Random.Range(3, 41);

        return new int[2] { randomItems, randomPrice };
    }
}
