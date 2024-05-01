using Photon.Pun;
using Photon.Pun.Demo.Asteroids;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirsterCoinLasterGiveing : MonoBehaviour, ICoinRouletteEvent
{
    YieldInstruction delay02f = new WaitForSeconds(0.2f);
    const int AMOUNT = 10;
    YieldInstruction delay3 = new WaitForSeconds(3);


    public string eventName => "1등 코인 꼴등한테 강제기부";


    public IEnumerator CoinEvent(int gamblerId, PhotonView view)
    {
        int firster = -1;
        int laster = -1;

        foreach (KeyValuePair<int, Gambler> item in Gambler.spawned)
        {
            if (item.Value.statistic.ranke == 0)
                firster = item.Key;
            if (item.Value.statistic.ranke == Gambler.spawned.Count - 1)
                laster = item.Key;
        }

        int firstercoin = Gambler.Get(firster).statistic.coin - 
                                              Mathf.Min(Gambler.Get(firster).statistic.coin, AMOUNT);
        int lastercoin = Gambler.Get(laster).statistic.coin + 
                                              Mathf.Min(Gambler.Get(firster).statistic.coin, AMOUNT);

        view.RPC("FirsterCoinLasterGiveingEventCall", RpcTarget.All,
                                 new object[] { firster, laster, firstercoin, lastercoin });

        yield return delay3;
        yield return delay3;
    }

    public IEnumerator FirsterCoinLasterGiveingEvent(int firsterId, int lasterId, int firstercoin, int lastercoin)
    {
        Gambler firster = Gambler.Get(firsterId);
        Gambler laster = Gambler.Get(lasterId);

        CameraManager.instance.ChangePointOfView(firsterId);
        yield return delay02f;

        GameObject go = ObjectPoolingManager.instance.GetGo("Coin");
        go.transform.position = firster.transform.position;

        float t = 0;
        while (t <= 1f)
        {
            t += Time.deltaTime;
            go.transform.position = Vector3.Lerp(firster.transform.position,
                                                 firster.transform.position + new Vector3(0, 15f, 0),
                                                 t / 1f);
            yield return null;
        }
        Gambler.Get(firsterId).statistic.coin = 0;
        CameraManager.instance.ChangePointOfView(lasterId);
        yield return delay02f;
        go.transform.position = laster.transform.position;
        t = 0;
        while (t <= 1f)
        {
            t += Time.deltaTime;
            go.transform.position = Vector3.Lerp(laster.transform.position,
                                                 laster.transform.position + new Vector3(0, 15f, 0),
                                                 t / 1f);
            yield return null;
        }
        Gambler.Get(lasterId).statistic.coin = 0;
        go.SetActive(false);
        yield return delay02f;
        go.SetActive(true);
        t = 0;
        while (t <= 1f)
        {
            t += Time.deltaTime;
            go.transform.position = Vector3.Lerp(laster.transform.position + new Vector3(0, 15f, 0),
                                                 laster.transform.position,
                                                 t / 1f);
            yield return null;
        }
        Gambler.Get(lasterId).statistic.coin = lastercoin;
        CameraManager.instance.ChangePointOfView(firsterId);
        yield return delay02f;
        t = 0;
        while (t <= 1f)
        {
            t += Time.deltaTime;
            go.transform.position = Vector3.Lerp(firster.transform.position + new Vector3(0, 15f, 0),
                                                 firster.transform.position,
                                                 t / 1f);
            yield return null;
        }
        Gambler.Get(firsterId).statistic.coin = firstercoin;
        go.GetComponent<PoolAble>().ReleaseObject();
    }
}
