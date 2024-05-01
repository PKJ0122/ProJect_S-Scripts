using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIWinner : UIScreensBase
{
    float t = 0;


    public override void Show()
    {
        base.Show();
        t = 0;
    }

    private void Update()
    {
        t += Time.deltaTime;

        if (t >= 2f)
        {
            UIManager.instance.Get<UIWinner>().Hide();
        }
    }
}
