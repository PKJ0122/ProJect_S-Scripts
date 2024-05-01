using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class UIGameRoom : UIScreensBase, IInRoomCallbacks
{
    bool canStartGamePlay
    {
        get => _canStartGamePlay;
        set
        {
            _canStartGamePlay = value;
            if (PhotonNetwork.IsMasterClient) // 방장인지 체크
            {
                _start.interactable = value;
            }
        }
    }

    //public event Action<int> charnumchange;

    bool _canStartGamePlay;
    bool checkCharacter;
    ReadyPlayRoomPlayerSlot[] _playerSlots;
    Button _start;
    Button _end;
    Button _roomSetting;
    Toggle _ready;
    UICharToggle[] _charButtons = new UICharToggle[6];
    TMP_Text _roomname;
    MapButton[] _mapButtons;
    string _mapName;

    protected override void Awake()
    {
        base.Awake();

        _playerSlots = transform.Find("Panel/PlayerList").GetComponentsInChildren<ReadyPlayRoomPlayerSlot>();
        _start = transform.Find("Panel/Button - Start").GetComponent<Button>();
        _end = transform.Find("Panel/Button - Exit").GetComponent<Button>();
        _roomSetting = transform.Find("Panel/Button - RoomSetting").GetComponent<Button>();
        _ready = transform.Find("Panel/Toggle - Ready").GetComponent<Toggle>();
        for (int i = 0; i < _charButtons.Length; i++)
        {
            _charButtons[i] = transform.Find($"Panel/Panel - Character/CharPanel/Image - CharBox{i}/Toggle - Char{i}").GetComponent<UICharToggle>();            
        }
        _roomname = transform.Find("Panel/Panel - Roomname/Text (TMP) - Roomname").GetComponent<TMP_Text>();
        _mapButtons = transform.Find("Panel/Panel - MapSelet").GetComponentsInChildren<MapButton>();

        _canStartGamePlay = false;
        _start.interactable = false;
        _start.onClick.AddListener(() =>
        {
            if (PhotonNetwork.IsMasterClient == false)
                return;
            if (canStartGamePlay == false)
                return;
            
            int code = (int)PhotonNetwork.CurrentRoom.CustomProperties["isMap"];
            _mapName = $"{(MapCode)code}";
            PhotonNetwork.LoadLevel($"Map-{_mapName}");
        });

        _ready.onValueChanged.AddListener(value =>
        {
            PhotonNetwork.LocalPlayer.SetCustomProperties(new Hashtable
                        {
                            { "isReady", value }
                        });
            foreach (var _char in _charButtons)
            {
                _char.button.interactable = !value;
            }
        });

        _roomSetting.onClick.AddListener(() =>
        {
            UIManager.instance.Get<UIRoomSetting>().Show();
        });

        _end.onClick.AddListener(() =>
        {
            UIManager.instance.Get<UIGameRoom>().Hide();
            UIManager.instance.Get<UILodingWindow>().Show();
            _ready.isOn = false;
            PhotonNetwork.LeaveRoom();
            RefreshPlayerSlots();
        });
    }
    private void Start()
    {
        for (int i = 0; i < _charButtons.Length; i++)
        {
            _charButtons[i].index = i;
            _charButtons[i].OnButtonEnter();
        }
        for (int i = 0; i < _mapButtons.Length; i++)
        {
            _mapButtons[i].index = i;
        }

        foreach (MapButton _maps in _mapButtons)
        {
            _maps.OnMapButtonEnter();

            if (!PhotonNetwork.LocalPlayer.IsMasterClient)
            {
                _maps.button.interactable = false;
            }
        }       
    }

    public override void Show()
    {
        base.Show();
        StartCoroutine(C_Init());
    }

    private void OnEnable()
    {
        PhotonNetwork.AddCallbackTarget(this);
    }
    private void OnDisable()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    private IEnumerator C_Init()
    {
        _roomname.text = PhotonNetwork.CurrentRoom.Name;
        yield return new WaitUntil(() => PhotonNetwork.NetworkClientState == ClientState.Joined);
        yield return StartCoroutine(C_RefreshPlayerSlots(PhotonNetwork.LocalPlayer));
        _start.gameObject.SetActive(PhotonNetwork.IsMasterClient);  // 내가 방장이면 start 버튼 활성화
        _ready.gameObject.SetActive(!PhotonNetwork.IsMasterClient); // 아니면 ready 토글 활성화
        _roomSetting.interactable = PhotonNetwork.IsMasterClient;
    }

    public void OnMasterClientSwitched(Player newMasterClient)
    {
        _start.gameObject.SetActive(newMasterClient.IsLocal);  // 내가 방장이면 start 버튼 활성화
        _ready.gameObject.SetActive(!newMasterClient.IsLocal); // 아니면 ready 토글 활성화
        _roomSetting.interactable = newMasterClient.IsLocal;
        RefreshPlayerSlots();
    }

    public void OnPlayerEnteredRoom(Player newPlayer)
    {
        StartCoroutine(C_RefreshPlayerSlots(newPlayer));
        Debug.Log("방에 들어옴");
    }

    public void OnPlayerLeftRoom(Player otherPlayer)
    {
        Debug.Log($"{otherPlayer.IsInactive}");
        Debug.Log($"{otherPlayer.NickName} 방에서 나감");
        RefreshPlayerSlots();
    }

    public void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        // 닉네임 값으로 딕셔너리 만들면 효율적일듯
        for (int i = 0; i < _playerSlots.Length; i++)
        {
            if (_playerSlots[i].nickname.Equals(targetPlayer.NickName))
            {
                if (changedProps.TryGetValue("isReady", out object value))
                {
                    _playerSlots[i].isReady = (bool)value;
                }
                if (changedProps.TryGetValue("isChar", out object num))
                {
                    _playerSlots[i].isModle = (int)num;
                }
            }
        }
        CanReady();
        CanStartGamePlay();
    }


    public void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
    {
        int mapname = 0;
        if(propertiesThatChanged.TryGetValue("isMap", out object value))
        {
            mapname = (int)value;
        }
        foreach (MapButton map in _mapButtons)
        {
            map.ButtonColorChange(mapname);
        }
    }

    public void RefreshPlayerSlots()
    {
        for (int i = 0; i < _playerSlots.Length; i++)
        {
            if (i < PhotonNetwork.PlayerList.Length)
            {
                _playerSlots[i].nickname = PhotonNetwork.PlayerList[i].NickName;
                _playerSlots[i].isReady = (bool)PhotonNetwork.PlayerList[i].CustomProperties["isReady"];
                _playerSlots[i].isModle = (int)PhotonNetwork.PlayerList[i].CustomProperties["isChar"];
                _playerSlots[i].isMaster = PhotonNetwork.PlayerList[i].IsMasterClient;
            }
            else
            {
                _playerSlots[i].nickname = string.Empty;
                _playerSlots[i].isReady = false;
                _playerSlots[i].isMaster = false;
                _playerSlots[i].isModle = -1;
            }
        }
        int mapname = 0;
        if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("isMap", out object value))
        {
            mapname = (int)value;
        }

        for (int i = 0; i < _mapButtons.Length; i++)
        {
            _mapButtons[i].ButtonColorChange(mapname);
            if (PhotonNetwork.LocalPlayer.IsMasterClient)
            {
                _mapButtons[i].button.interactable = true;
            }
            else
            {
                _mapButtons[i].button.interactable = false;
            }
        }
    }



    /// <summary>
    /// 새로 들어온 플레이어는 아직 isReady 에 대한 커스텀 프로퍼티 값 설정이 완료되지 않을수도 있으므로 완료를 기다린 후 슬롯갱신 실행 
    /// </summary>
    private IEnumerator C_RefreshPlayerSlots(Player newPlayer)
    {
        yield return new WaitUntil(() => newPlayer.CustomProperties.ContainsKey("isReady"));
        yield return new WaitUntil(() => newPlayer.CustomProperties.ContainsKey("isChar"));
        RefreshPlayerSlots();
    }

    private void CanReady()
    {
        if ((int)PhotonNetwork.LocalPlayer.CustomProperties["isChar"] >= 0)
        {
            _ready.interactable = true;
        }
        else
        {
            _ready.interactable = false;
            return;
        }
        for (int i = 0; i < PhotonNetwork.CurrentRoom.PlayerCount; i++)
        {
            Debug.Log(PhotonNetwork.PlayerList[i].NickName);
            if (PhotonNetwork.PlayerList[i].NickName.Equals(PhotonNetwork.LocalPlayer.NickName))
            {
                continue;
            }
            else
            {
                if ((int)PhotonNetwork.PlayerList[i].CustomProperties["isChar"] == (int)PhotonNetwork.LocalPlayer.CustomProperties["isChar"])
                {
                    Debug.Log((int)PhotonNetwork.LocalPlayer.CustomProperties["isChar"]);
                    checkCharacter = false;
                    _ready.interactable = false;
                }
                else
                {
                    _ready.interactable = true;
                    checkCharacter = true;
                }
            }
        }
    }

    /// <summary>
    /// 모든 플레이어가 Ready를 했는지 체크
    /// </summary>
    private void CanStartGamePlay()
    {
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            if (player.IsMasterClient)
            {
                if (player.CustomProperties.TryGetValue("isChar", out object numn))
                {
                    if ((int)numn < 0)
                    {
                        Debug.Log("Char is -1");
                        canStartGamePlay = false;
                        return;
                    }
                }
                continue;
            }

            if (player.CustomProperties.TryGetValue("isReady", out object isReady))
            {
                if ((bool)isReady == false)
                {
                    canStartGamePlay = false;
                    return;
                }
            }
            else
            {
                canStartGamePlay = false;
                return;
            }
            if (player.CustomProperties.TryGetValue("isChar", out object ischar))
            {
                if ((int)ischar < 0)
                {
                    Debug.Log("Char is -1");
                    canStartGamePlay = false;
                    return;
                }
            }
            else
            {
                canStartGamePlay = false;
                return;
            }
        }
        if (checkCharacter == false)
        {
            canStartGamePlay = false;
            return;
        }
        canStartGamePlay = true;
    }
}
