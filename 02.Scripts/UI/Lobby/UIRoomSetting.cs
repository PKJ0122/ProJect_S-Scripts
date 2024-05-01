using Photon.Pun;
using TMPro;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class UIRoomSetting : UIPopupBase
{
    Button _closebutton;
    Slider _slider;
    TMP_Text _valueText;
    int _maxturn;

    protected override void Awake()
    {
        base.Awake();
        _slider = transform.Find("Panel/BG/Slider - Trun").GetComponent<Slider>();
        _closebutton = transform.Find("Panel/BG/Button - Close").GetComponent<Button>();
        _valueText = transform.Find("Panel/BG/Slider - Trun/Handle Slide Area/Handle/Text (TMP) - Count").GetComponent<TMP_Text>();

        _slider.onValueChanged.AddListener(value =>
        {
            _maxturn = (int)value;
            _valueText.text = $"{value}";
        });

        _closebutton.onClick.AddListener(() =>
        {
            PhotonNetwork.CurrentRoom.SetCustomProperties(new Hashtable
            {
                     { "isTrunCount", _maxturn }
            });
            Hide();
        });
    }
}
