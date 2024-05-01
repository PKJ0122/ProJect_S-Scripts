using Photon.Pun;
using UnityEngine;


public class Item_DiceSelet : ItemBase
{
    bool _itemUse;

    protected override void Awake()
    {        
        itemtype = ItemCode.DiceSelet;
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
                UIManager.instance.Get<UIDiceSelet>().Show();
                _itemUse = false;
                UIManager.instance.Get<UISettingPopup>().itemOn = false;
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

