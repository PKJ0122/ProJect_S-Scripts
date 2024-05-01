using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIDiceSelet : UIPopupBase
{
    DiceSeletButton[] _buttons;

    protected override void Awake()
    {
        base.Awake();
        _buttons = GetComponentsInChildren<DiceSeletButton>();

        for(int i = 0; i < _buttons.Length; i++)
        {
            _buttons[i].index = i;
        }
    }
}
