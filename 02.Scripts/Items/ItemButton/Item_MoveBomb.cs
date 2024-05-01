using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item_MoveBomb : ItemBase
{
    bool _itemUse;   

    protected override void Awake()
    {
        itemtype = ItemCode.MoveBomb;
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
            if(Input.GetKeyDown(KeyCode.Escape))
            {
                _key.escKey = false;
                _key.eKey = false;
                UIManager.instance.Get<UITurnAction>().Show();
                _itemUse = false;
                Count++;
                UIManager.instance.Get<UISettingPopup>().itemOn = false;
            }

            if(Input.GetKeyDown(KeyCode.E))
            {
                _key.escKey = false;
                _key.eKey = false;
                _key.wasdKey = true;
                _itemUse = false;
                UIManager.instance.Get<UISettingPopup>().itemOn = false;
                Vector3 pos = Gambler.Get(PhotonNetwork.LocalPlayer.ActorNumber).transform.position;
                pos += new Vector3(0, 1, 0);
                PhotonNetwork.Instantiate("Items/Item_MoveBomb", pos, Quaternion.identity)
                        .GetComponent<ItemEvent_MoveBomb>().isActive = true;
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
