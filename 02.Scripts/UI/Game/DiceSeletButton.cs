using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DiceSeletButton : MonoBehaviour
{
    public int index
    {
        set
        {
            _index = value;
        }
    }

    int _index;
    TMP_Text text;
    Button button;


    private void Awake()
    {
        text = transform.Find("Text (TMP)").GetComponent<TMP_Text>();
        button = GetComponent<Button>();
    }

    private void Start()
    {
        text.text = $"{_index+1}";
        ButtonUpdate();
    }

    public void ButtonUpdate()
    {
        button.onClick.AddListener(() =>
        {
            Vector3 pos = Gambler.Get(PhotonNetwork.LocalPlayer.ActorNumber).transform.position;
            pos += new Vector3(0, 5, 0);
            ItemEvent_DiceSelet a = PhotonNetwork.Instantiate("Items/Item_DiceSelet", pos, Quaternion.identity)
                    .GetComponent<ItemEvent_DiceSelet>();
            a.isNum = _index + 1;
            a.isActive = true;
            UIManager.instance.Get<UIDiceSelet>().Hide();
        });
    }
}
