using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIAward : UIScreensBase
{
    TMP_Text winerName;
    TMP_Text awardTitle;
    TMP_Text awardinfo;

    protected override void Awake()
    {
        base.Awake();
        awardTitle = transform.Find("Panel/AwardPanel/Text (TMP) - AwardTitle").GetComponent<TMP_Text>();
        awardinfo = transform.Find("Panel/AwardPanel/Text (TMP) - Introduction").GetComponent<TMP_Text>();
        winerName = transform.Find("Panel/Text (TMP) - WinnerText").GetComponent<TMP_Text>();
    }

    public void SetInfo(string nickName, string awardName, string info)
    {
        winerName.text = nickName;
        awardTitle.text = awardName;
        awardinfo.text = info;
    }
}
