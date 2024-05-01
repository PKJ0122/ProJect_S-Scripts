using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class UICharToggle : MonoBehaviour
{
    public int index
    {
        get => _index;
        set
        {
            _index = value;
            _charImage.sprite = Resources.Load<Sprite>($"UI/CharImage/re - char{_index}");
        }
    }

    public bool interactableCheck
    {
        get => _interactableCheck;
        set
        {
            _interactableCheck = value;
            button.interactable = _interactableCheck;
        }
    }

    public Button button
    {
        get => _button;
    }

    int _index;
    bool _interactableCheck = true;
    Image _charImage;
    Button _button;

    
    private void Awake()
    {
        _charImage = GetComponent<Image>();
        _button = GetComponent<Button>();
    }

    public void OnButtonEnter()
    {
        _button.onClick.AddListener(() =>
        {
            PhotonNetwork.LocalPlayer.SetCustomProperties(new Hashtable
                {
                     { "isChar", _index }         
                });
            //interactableCheck = !interactableCheck;
        });
    }    
}
