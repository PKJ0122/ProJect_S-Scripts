using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;

[RequireComponent(typeof(PhotonView))]
public class ItemEvent_GatlingGun : MonoBehaviour, IItemEvent
{
    const int DMAGE = 18;
    public bool isActive
    {
        set => _isActive = value;
    }
    bool _isActive;

    PhotonView _photonView;
    WaitForSeconds deley = new WaitForSeconds(1f);


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
            UIManager.instance.Get<UITurnAction>().Hide();
            StartCoroutine(ItemEvent());
        }
    }

    public IEnumerator ItemEvent()
    {
        Debug.Log("이벤트 실행");
        _photonView.RPC("EftCreat", RpcTarget.All, _photonView.OwnerActorNr);
        foreach (KeyValuePair<int,Gambler> item in Gambler.spawned)
        {
            if (item.Key != _photonView.OwnerActorNr)
            {
                Gambler.Get(PhotonNetwork.LocalPlayer.ActorNumber).statistic.dmage += item.Value.statistic.isPuppet ? 0 : DMAGE;
                Gambler.Get(PhotonNetwork.LocalPlayer.ActorNumber).statistic.kill += (item.Value.statistic.hp - DMAGE > 0) ? 0 : 1;
                _photonView.RPC("DamgeEvent", RpcTarget.All, item.Key);
            }
        }
        yield return deley;
        UIManager.instance.Get<UITurnAction>().Show();
        PhotonNetwork.Destroy(gameObject);
    }

    [PunRPC]
    public void EftCreat(int clientId)
    {
        Gambler gambler = Gambler.Get(clientId);
        GameObject eff = ObjectPoolingManager.instance.GetGo("MagicEft");
        eff.transform.position = gambler.transform.position + new Vector3(0, 5, 0);
    }

    [PunRPC]
    public void DamgeEvent(int clientId)
    {
        Gambler gambler = Gambler.Get(clientId);
        if (gambler.statistic.isPuppet)
        {
            gambler.statistic.isPuppet = false;
        }
        else
        {
            gambler.statistic.hp -= DMAGE;
        }
    }
}


