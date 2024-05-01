using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AllCoinDistribution : MonoBehaviour, ICoinRouletteEvent
{
    public string eventName => "모든 코인 균등 분배";

    YieldInstruction delay05f = new WaitForSeconds(0.5f);
    YieldInstruction delay3 = new WaitForSeconds(3);



    public IEnumerator CoinEvent(int gamblerId , PhotonView view)
    {
        int allCoin = 0;

        foreach (KeyValuePair<int, Gambler> item in Gambler.spawned)
        {
            allCoin += item.Value.statistic.coin;
        }

        int amount = allCoin / Gambler.spawned.Count;

        view.RPC("AllCoinDistributionEventCall", RpcTarget.All, 
            new object[] { gamblerId, amount });

        yield return delay3;
    }

    public IEnumerator AllCoinDistributionEvent(int gamblerId,int allcoin)
    {
        Gambler gambler = Gambler.Get(gamblerId);
        GameObject go = ObjectPoolingManager.instance.GetGo("Coin");
        go.transform.position = gambler.transform.position;
        go.transform.localScale = gambler.transform.localScale;

        float t = 0;
        while (t <= 1f)
        {
            t += Time.deltaTime;
            go.transform.position = Vector3.Lerp(gambler.transform.position + new Vector3(0, 2, 0),
                                                 gambler.transform.position + new Vector3(0, 15, 0),
                                                 t / 1f);
            yield return null;
        }
        yield return delay05f;
        t = 0;
        while (t <= 1f)
        {
            t += Time.deltaTime;
            go.transform.position = Vector3.Lerp(gambler.transform.position + new Vector3(0, 15, 0),
                                                 gambler.transform.position + new Vector3(0, 2, 0),
                                                 t / 1f);
            yield return null;
        }
        go.GetComponent<PoolAble>().ReleaseObject();
        Gambler.Get(PhotonNetwork.LocalPlayer.ActorNumber).statistic.coin = allcoin;
    }
}
