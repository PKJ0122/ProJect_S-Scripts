using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIChating : UIScreensBase
{
    Button _send;
    TMP_InputField _input;
    Transform _viewtransform;


    protected override void Awake()
    {
        base.Awake();
        ObjectPoolingManager.instance.AddObjPool("Messages", 7);
        _viewtransform = transform.Find("Panel/Scroll View/Viewport/Content").GetComponent<RectTransform>();
        _send = transform.Find("Panel/Button - Send").GetComponent<Button>();
        _input = transform.Find("Panel/InputField (TMP) - Message").GetComponent<TMP_InputField>();
        _input.onValueChanged.AddListener(value =>
        {
            _send.interactable = value != null;
        });
        _send.onClick.AddListener(() =>
        {
            ChatManager.instance.PublishMessage(_input.text);
            _input.text = null;
            _send.interactable = false;
        });

        ChatManager.instance.chatevent += value => OnGetMessages(value);
    }

    public void OnGetMessages(string value)
    {
        PoolAble[] tmptexts = _viewtransform.GetComponentsInChildren<PoolAble>();
        if (tmptexts.Length > 6)
        {
            for(int i = 0; i < tmptexts.Length - 1; i++)
            {
                tmptexts[i].GetComponent<TMP_Text>().text = tmptexts[i + 1].GetComponent<TMP_Text>().text;
            }          
            tmptexts[tmptexts.Length - 1].ReleaseObject();
        }
        GameObject text = ObjectPoolingManager.instance.GetGo("Messages");
        text.transform.SetParent(_viewtransform);
        text.GetComponent<TMP_Text>().text = value;
    }
}
