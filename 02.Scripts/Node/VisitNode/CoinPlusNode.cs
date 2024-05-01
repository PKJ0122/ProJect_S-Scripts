using Photon.Pun;
using System.Collections;
using UnityEngine;

public class CoinPlusNode : NodeBase
{
    PhotonView _view;

    YieldInstruction delay = new WaitForSeconds(1);


    private void Awake()
    {
        _view = GetComponent<PhotonView>();
    }

    public override IEnumerator Visit(int gamblerId)
    {
        yield return delay;

        Gambler gambler = Gambler.Get(gamblerId);

        _view.RPC("CoinGetCall", RpcTarget.Others, gamblerId);
        yield return StartCoroutine(CoinGet(gamblerId));
        gambler.statistic.coin += 10;
    }

    [PunRPC]
    private void CoinGetCall(int gamblerId)
    {
        StartCoroutine(CoinGet(gamblerId));
    }

    IEnumerator CoinGet(int gamblerId)
    {
        Gambler gambler = Gambler.Get(gamblerId);
        GameObject go = ObjectPoolingManager.instance.GetGo("CoinEF");
        go.transform.position = gambler.transform.position + new Vector3(0, 3f, 0);

        yield return delay;
    }
}
