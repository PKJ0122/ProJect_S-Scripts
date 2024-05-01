using Photon.Pun;
using System.Collections;
using UnityEngine;
using UnityEngine.Profiling;

[RequireComponent(typeof(PhotonView))]
public class ItemEvent_Puppet : MonoBehaviour, IItemEvent
{
    public bool isActive
    {
        set => _isActive = value;
    }
    bool _isActive;

    PhotonView _photonView;
    WaitForSeconds deley;


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
        yield return deley;
        _photonView.RPC("PuppetEvent", RpcTarget.All, PhotonNetwork.LocalPlayer.ActorNumber);
        Gambler.Get(PhotonNetwork.LocalPlayer.ActorNumber).statistic.isPuppet = true;
        yield return deley;
        PhotonNetwork.Destroy(gameObject);
    }

    [PunRPC]
    public void PuppetEvent(int clientId)
    {
        Gambler gambler = Gambler.Get(clientId);
        gambler.statistic.isPuppet = true;
    }
}

