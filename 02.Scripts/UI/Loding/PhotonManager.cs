using Photon.Pun;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class PhotonManager : MonoBehaviourPunCallbacks
{
    #region Singleton
    public static PhotonManager instance
    {
        get
        {
            if (s_instance == null)
            {
                s_instance = new GameObject(typeof(PhotonManager).Name).AddComponent<PhotonManager>();
                DontDestroyOnLoad(s_instance.gameObject);
            }
            return s_instance;
        }
    }
    private static PhotonManager s_instance;
    #endregion

    private void Awake()
    {
        if (PhotonNetwork.IsConnected == false)
        {
            bool isConnected = PhotonNetwork.ConnectUsingSettings();
            Debug.Assert(isConnected, "[PhotonManager] : Failed to photon server");
        }
    }

    public override void OnConnectedToMaster()
    {
        base.OnConnected();
        PhotonNetwork.AutomaticallySyncScene = true; // PhotonNetWork.LoadLevel() 호출시 현재 동일한 방에있는 모든 클라이언트의 씬을 동기화하는 옵션
        PhotonNetwork.NickName = NicknameInfo.myNickname;
    }

    public override void OnJoinedLobby()
    {
        base.OnJoinedLobby();
        Debug.Log($"[PhotonManager] : Join Lobby");
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();

        PhotonNetwork.LocalPlayer.SetCustomProperties(new Hashtable
            {
                { "isReady", false },
                { "isChar", -1 },
                { "isLoading" , false }
            });
    }

    public override void OnLeftRoom()
    {
        base.OnLeftRoom();

        PhotonNetwork.LocalPlayer.SetCustomProperties(new Hashtable
        {
            { "isReady", false }, { "isChar", -1 }
        });
    }
}
