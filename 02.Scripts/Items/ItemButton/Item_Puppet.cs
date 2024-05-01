using Photon.Pun;
using UnityEngine;


public class Item_Puppet : ItemBase
{
    bool _itemUse;

    protected override void Awake()
    {
        itemtype = ItemCode.Puppet;
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
                _key.eKey = false;
                _itemUse = false;
                UIManager.instance.Get<UISettingPopup>().itemOn = false;
                Vector3 pos = Gambler.Get(PhotonNetwork.LocalPlayer.ActorNumber).transform.position;
                pos += new Vector3(0, 5, 0);
                PhotonNetwork.Instantiate("Items/Item_Puppet", pos, Quaternion.identity)
                        .GetComponent<ItemEvent_Puppet>().isActive = true;
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

