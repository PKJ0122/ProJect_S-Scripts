using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class UILobby : UIScreensBase, ILobbyCallbacks, IMatchmakingCallbacks
{
    public const int NOT_SELECTED = -1;
    public int roomListSlotIndexSeleted
    {
        get => _roomListSlotIndexSeleted;
        set
        {
            _roomListSlotIndexSeleted = value;
            _joinButton.interactable = value > NOT_SELECTED;
        }
    }

    int _roomListSlotIndexSeleted = NOT_SELECTED;
    RoomListSlot _roomListSlot;
    List<RoomListSlot> _roomListSlots = new List<RoomListSlot>(20);
    Transform _roomListContent;
    Button _createButton;
    Button _joinButton;
    List<RoomInfo> _localRoomList;

    // RoomOptionPanel
    GameObject _roomOptionPanel;
    TMP_InputField _roomName;
    Button _confirmRoomOption;
    Button _cancleRoomOption;


    protected override void Awake()
    {
        base.Awake();

        _roomListSlot = Resources.Load<RoomListSlot>("UI/RoomListSlot");
        _roomListContent = transform.Find("Panel/Scroll View - RoomList/Viewport/Content").GetComponent<RectTransform>();
        _joinButton = transform.Find("Panel/Button - JoinRoom").GetComponent<Button>();
        _createButton = transform.Find("Panel/Button - CreateRoom").GetComponent<Button>();
        _joinButton.interactable = false;
        _joinButton.onClick.AddListener(() =>
        {
            if (PhotonNetwork.JoinRoom(_localRoomList[_roomListSlotIndexSeleted].Name))
            {
                UIManager.instance.Get<UILodingWindow>().Show();
            }
            else
            {
                
            }
        });
        _createButton.onClick.AddListener(() =>
        {
            _roomName.text = string.Empty;
            _roomOptionPanel.SetActive(true);
        });

        _roomOptionPanel = transform.Find("Panel - RoomOption").gameObject;
        _roomName = transform.Find("Panel - RoomOption/BG/InputField (TMP) - RoomName").GetComponent<TMP_InputField>();
        _confirmRoomOption = transform.Find("Panel - RoomOption/BG/Button - Confirm").GetComponent<Button>();
        _cancleRoomOption = transform.Find("Panel - RoomOption/BG/Button - Cancle").GetComponent<Button>();
        _confirmRoomOption.interactable = false;

        _roomName.onValueChanged.AddListener(value =>
        {
            _confirmRoomOption.interactable = value.Length > 1;
        });

        _confirmRoomOption.onClick.AddListener(() =>
        {
            if (PhotonNetwork.CreateRoom(_roomName.text, new RoomOptions
            {
                MaxPlayers = 4,
                PublishUserId = true,
            }))
            {
                UIManager.instance.Get<UILodingWindow>().Show();
            }
        });
        _cancleRoomOption.onClick.AddListener(() => _roomOptionPanel.SetActive(false));
    }

    private void Start()
    {
        UIManager.instance.Get<UILodingWindow>().Show();
        if (PhotonNetwork.InRoom)
        {
            UIManager.instance.Get<UIGameRoom>().Show();
            PhotonNetwork.LocalPlayer.SetCustomProperties(new Hashtable
            {
                            { "isReady", false }
            });
            UIManager.instance.Get<UIGameRoom>().RefreshPlayerSlots();
            
            UIManager.instance.Get<UILodingWindow>().Hide();
        }
    }

    private void OnEnable()
    {
        PhotonNetwork.AddCallbackTarget(this);
        StartCoroutine(C_JoinLobby());
    }

    private void OnDisable()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    IEnumerator C_JoinLobby()
    {
        yield return new WaitUntil(() => PhotonNetwork.NetworkClientState == ClientState.ConnectedToMasterServer);
        PhotonNetwork.JoinLobby();
    }

    public void OnJoinedLobby()
    {
        Debug.Log("[UILobby] : 로비 조인");
        UIManager.instance.Get<UILodingWindow>().Hide();
    }

    public void OnLeftLobby()
    {
        Debug.Log("[UILobby] : 로비에서 나감");
    }


    public void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        Debug.Log(roomList.Count);
        _localRoomList = roomList;

        for (int i = 0; i < _roomListSlots.Count; i++)
        {
            Destroy(_roomListSlots[i].gameObject);
        }
        _roomListSlots.Clear();
        for (int i = 0; i < roomList.Count; i++)
        {
            if (roomList[i].RemovedFromList)
            {
                continue;
            }
            RoomListSlot slot = Instantiate(_roomListSlot, _roomListContent);
            if(slot == null)
            {
                Debug.Log("Nullzz");
            }
            slot.roomIndex = i;
            slot.Refresh(roomList[i].Name, roomList[i].PlayerCount, roomList[i].MaxPlayers);
            slot.onSelected += (index) => roomListSlotIndexSeleted = index;
            _roomListSlots.Add(slot);
        }
    }

    public void OnLobbyStatisticsUpdate(List<TypedLobbyInfo> lobbyStatistics)
    {
        throw new System.NotImplementedException();
    }

    public void OnFriendListUpdate(List<FriendInfo> friendList)
    {
        throw new System.NotImplementedException();
    }

    public void OnCreatedRoom()
    {
        UIManager.instance.Get<UILodingWindow>().Hide();
        PhotonNetwork.CurrentRoom.SetCustomProperties(new Hashtable
                {
                     { "isMap", (int)MapCode.Ocean }
                });
        Debug.Log("[UILobby] : 방 생성 완료");
        ChatManager._roomNames.Add(PhotonNetwork.CurrentRoom.Name);
        _roomOptionPanel.SetActive(false);
    }

    public void OnCreateRoomFailed(short returnCode, string message)
    {
        UIManager.instance.Get<UILodingWindow>().Hide();
    }

    public void OnJoinedRoom()
    {
        UIManager.instance.Get<UIGameRoom>().Show();
        Debug.Log("[UILobby] : 방 접속 성공");
        //PhotonNetwork.LeaveLobby();
        UIManager.instance.Get<UILodingWindow>().Hide();
    }

    public void OnJoinRoomFailed(short returnCode, string message)
    {
        Show();
        UIManager.instance.Get<UILodingWindow>().Hide();
    }

    public void OnJoinRandomFailed(short returnCode, string message)
    {
        Show();
        UIManager.instance.Get<UILodingWindow>().Hide();
    }

    public void OnLeftRoom()
    {
        Debug.Log("[UILobby] : 방에서 나옴");
        StartCoroutine(C_JoinLobby());
        Show();
    }
}

