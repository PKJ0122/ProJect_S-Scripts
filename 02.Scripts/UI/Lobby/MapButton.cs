using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class MapButton : MonoBehaviour
{
    public int index
    {
        get => _index;
        set => _index = value;
    }

    public Image myImage
    {
        get => _image;
    }

    public Button button
    {
        get => _button;
    }

    int _index;
    Button _button;
    Image _image;


    private void Awake()
    {
        _button = GetComponent<Button>();
        _image = transform.GetComponent<Image>();
    }

    public void OnMapButtonEnter()
    {
        _button.onClick.AddListener(() =>
        {
            PhotonNetwork.CurrentRoom.SetCustomProperties(new Hashtable
            {
                { "isMap", (MapCode)_index }
            });
        });
    }

    public void ButtonColorChange(int value)
    {
        if (value == _index)
        {
            _image.color = Color.red;
        }
        else
        {
            _image.color = Color.white;
        }
    }
}
