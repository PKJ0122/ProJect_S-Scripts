using System;
using UnityEngine.UI;

public class UIVolunteer : UIScreensBase
{
    Button _give;
    Button _notgive;

    public event Action<bool> onButtonClick;


    protected override void Awake()
    {
        base.Awake();
        _give = transform.Find("Button - give").GetComponent<Button>();
        _notgive = transform.Find("Button - notgive").GetComponent<Button>();

        _give.onClick.AddListener(() =>
        {
            onButtonClick.Invoke(true);
            Hide();
        });
        _notgive.onClick.AddListener(() =>
        {
            onButtonClick.Invoke(false);
            Hide();
        });
    }
}