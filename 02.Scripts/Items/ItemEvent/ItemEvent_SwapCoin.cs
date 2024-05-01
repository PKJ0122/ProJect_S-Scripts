using Photon.Pun;
using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(PhotonView))]
public class ItemEvent_SwapCoin : MonoBehaviour, IItemEvent
{

    WaitForSeconds deley = new WaitForSeconds(1);
    public bool isActive
    {
        set => _isActive = value;
    }
    bool _isActive;

    PhotonView _photonView;


    private void Awake()
    {
        _photonView = GetComponent<PhotonView>();
    }

    private void Update()
    {
        if (!_photonView.IsMine)
        {
            return;
        }
        if (_isActive)
        {
            UIManager.instance.Get<UIKeyInfo>().Hide();
            _isActive = false;
            ItemsManager.instance.isUse = true;
            StartCoroutine(ItemEvent());
        }
    }

    public IEnumerator ItemEvent()
    {
        Debug.Log("이벤트 실행");

        Gambler localgmabler = Gambler.Get(PhotonNetwork.LocalPlayer.ActorNumber);

        while (true)
        {
            int index = Random.Range(0, PhotonNetwork.PlayerList.Length);
            
            if (PhotonNetwork.PlayerList[index].ActorNumber != PhotonNetwork.LocalPlayer.ActorNumber)
            {
                _photonView.RPC("DamgeEvent", RpcTarget.All, new object[2] { PhotonNetwork.PlayerList[index].ActorNumber, PhotonNetwork.LocalPlayer.ActorNumber });
                break;
            }
        }
        yield return deley;
        PhotonNetwork.Destroy(gameObject);

    }

    [PunRPC]
    public void DamgeEvent(int clientId, int localId)
    {
        Gambler gambler = Gambler.Get(clientId);
        Gambler localgmabler = Gambler.Get(localId);
        if (gambler.statistic.isPuppet)
        {
            gambler.statistic.isPuppet = false;
        }
        else
        {
            (localgmabler.statistic.coin, gambler.statistic.coin) = (gambler.statistic.coin, localgmabler.statistic.coin);
        }
    }
}

