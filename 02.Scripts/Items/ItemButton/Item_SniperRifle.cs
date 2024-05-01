using Photon.Pun;
using UnityEngine;

public class Item_SniperRifle : ItemBase
{
    bool _itemUse;

    protected override void Awake()
    {
        itemtype = ItemCode.SpuerNova;
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
                Count++;
                UIManager.instance.Get<UISettingPopup>().itemOn = false;
            }

            if (Input.GetKeyDown(KeyCode.E))
            {
                _key.escKey = false;
                _key.wasdKey = true;
                _itemUse = false;
                UIManager.instance.Get<UISettingPopup>().itemOn = false;
                Vector3 pos = Gambler.Get(PhotonNetwork.LocalPlayer.ActorNumber).transform.position;
                PhotonNetwork.Instantiate("Items/Item_SpuerNova", pos, Quaternion.identity)
                        .GetComponent<ItemEvent_SniperRifle>().isActive = true;
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

