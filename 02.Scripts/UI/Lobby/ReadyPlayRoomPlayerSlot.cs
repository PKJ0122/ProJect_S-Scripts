using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class ReadyPlayRoomPlayerSlot : MonoBehaviour
{
    public bool isReady
    {
        get => _isReady.enabled;
        set
        {
            _isReady.enabled = value;
            if (value)
            {
                _readyBG.color = Color.yellow;
            }
            else
            {
                _readyBG.color = Color.white;
            }
        }
    }

    public bool isMaster
    {
        set 
        {
            _master.gameObject.SetActive(value);
            if (value)
            {
                _isReady.enabled = false;
                _readyBG.color = Color.white;
            }
        }
    }

    public string nickname
    {
        get => _nickname.text;
        set
        {
            _nickname.text = value;
        }
    }

    public int isModle
    {
        get => _isModle;
        set
        {
            if(value < 0)
            {
                _modleImg.sprite = null;
                _isModle = -1;
                return;
            }
            _isModle = value;
            _modleImg.sprite = _loadedImages[value];
        }
    }

    private TMP_Text _isReady;
    private TMP_Text _nickname;
    private TMP_Text _master;
    private Image _readyBG;
    private Image _modleImg;
    private int _isModle;
    private Dictionary<int, Sprite> _loadedImages = new Dictionary<int, Sprite>();

    private void Awake()
    {
        _isReady = transform.Find("Text (TMP) - Ready").GetComponent<TMP_Text>();
        _nickname = transform.Find("Text (TMP) - NickName").GetComponent<TMP_Text>();
        _master = transform.Find("Text (TMP) - Master").GetComponent<TMP_Text>();
        _readyBG = transform.Find("ReadyBG").GetComponent<Image>();
        _modleImg = transform.Find("Modle").GetComponent<Image>();

        for (int i = 0; i < Enum.GetValues(typeof(CharEnum)).Length; i++)
        {
            _loadedImages.Add(i, Resources.Load<Sprite>($"UI/CharImage/re - char{i}"));
        }
    }

}

