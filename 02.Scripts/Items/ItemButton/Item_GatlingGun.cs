﻿using Photon.Pun;
using UnityEngine;


public class Item_GatlingGun : ItemBase
{
    bool _itemUse;

    protected override void Awake()
    {
        itemtype = ItemCode.AllBlue;
        base.Awake();
        Count = 1;
    }

    protected override void Update()
    {
        base.Update();

        if (!_itemUse)
        {
            return;
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                _key.escKey = false;
                _key.eKey = false;
                UIManager.instance.Get<UITurnAction>().Show();
                _itemUse = false;
                UIManager.instance.Get<UISettingPopup>().itemOn = false;
                Count++;
            }

            if (Input.GetKeyDown(KeyCode.E))
            {
                _key.escKey = false;
                _key.eKey = false;
                _itemUse = false;
                UIManager.instance.Get<UISettingPopup>().itemOn = _itemUse;
                Vector3 pos = Gambler.Get(PhotonNetwork.LocalPlayer.ActorNumber).transform.position;
                pos += new Vector3(0, 5, 0);
                PhotonNetwork.Instantiate("Items/Item_AllBlue", pos, Quaternion.identity)
                        .GetComponent<ItemEvent_GatlingGun>().isActive = true;
            }
        }
    }

    public override void Use()
    {
        base.Use();
        UIManager.instance.Get<UIInventoryPopup>().Hide();
        Debug.Log(Count);
        _itemUse = true;
    }
}

