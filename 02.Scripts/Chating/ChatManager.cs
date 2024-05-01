using ExitGames.Client.Photon;
using Photon.Chat;
using Photon.Pun;
using System;
using System.Collections.Generic;
using UnityEngine;

public class ChatManager : SingletonMonoBase<ChatManager>, IChatClientListener
{
    public ChatClient ChatClient
    {
        get => _chatClient;
    }
    private ChatClient _chatClient;
    //private Dictionary<string, TMP_Text> _chatLogs = new Dictionary<string, TMP_Text>();
    private List<string> chats = new List<string>();


    public event Action<string> chatevent;
    public static List<string> _roomNames = new List<string>();

    private void Start()
    {
        ChatConnect();
    }

    public void ChatConnect()
    {
        _chatClient = new ChatClient(this);
        _chatClient.Connect("477689da-c19e-4bc5-b7c3-c2de6acc18f7",
                                PhotonNetwork.AppVersion,
                                new Photon.Chat.AuthenticationValues(NicknameInfo.myNickname));

    }

    void OnEnable()
    {
        PhotonNetwork.AddCallbackTarget(this);
    }

    protected override void Awake()
    {
        base.Awake();
    }

    private void Update()
    {
        _chatClient.Service();
    }

    public void PublishMessage(string message)
    {
        /*string channel = PhotonNetwork.CurrentRoom.Name;
        _chatClient.PublishMessage(channel, message);*/
        //Debug.Log(_chatClient.State);
        _chatClient.PublishMessage("chating", message);
    }

    public void OnConnected()
    {
        /*string[] roomnames = new string[_roomNames.Count];

        for(int i = 0; i < roomnames.Length; i++)
        {
            roomnames[i] = _roomNames[i];
        }*/
        //_chatClient.Subscribe(roomnames);
        if(_chatClient.Subscribe(new string[] { "chating" }))
        {
            Debug.Log("구독성공");
        }
        
    }

    public void OnGetMessages(string channelName, string[] senders, object[] messages)
    {
        //if(channelName == PhotonNetwork.CurrentRoom.Name)
        //if (channelName == "chating")
        //{
        Debug.Log("채팅 호출");
        for (int i = 0; i < senders.Length; i++)
        {
            chats.Add($"{senders[i]} : {messages[i]}");
            chatevent?.Invoke($"{senders[i]} : {messages[i]}");
        }
    }

    public void DebugReturn(DebugLevel level, string message)
    {

    }

    public void OnChatStateChange(ChatState state)
    {

    }

    public void OnDisconnected()
    {
    }

    public void OnPrivateMessage(string sender, object message, string channelName)
    {
    }

    public void OnStatusUpdate(string user, int status, bool gotMessage, object message)
    {
    }

    public void OnSubscribed(string[] channels, bool[] results)
    {
        Debug.Log("OnSubscribed");
    }

    public void OnUnsubscribed(string[] channels)
    {
    }

    public void OnUserSubscribed(string channel, string user)
    {
    }

    public void OnUserUnsubscribed(string channel, string user)
    {
    }
}