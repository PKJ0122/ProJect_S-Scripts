using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UINodeName : UIScreensBase
{
    const float OFF_TIME = 1.0f;

    TMP_Text nodeName;
    bool isOn;
    float time;


    protected override void Awake()
    {
        base.Awake();
        nodeName = transform.Find("Text (TMP) - NodeName").GetComponent<TMP_Text>();
    }

    void Update()
    {
        if (!isOn)
            return;

        time += Time.deltaTime;
        if (time >= OFF_TIME)
        {
            Hide();
        }
    }

    public void Show(string Name)
    {
        base.Show();
        nodeName.text = Name;
        isOn = true;
        time = 0;
    }
}
