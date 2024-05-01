using ExitGames.Client.Photon.StructWrapping;
using Photon.Pun;
using Photon.Pun.Demo.Asteroids;
using System.Collections;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class ExchangeNode : NodeBase
{
    PhotonView _view;
    YieldInstruction delay02f = new WaitForSeconds(0.5f);


    private void Awake()
    {
        _view = GetComponent<PhotonView>();
    }

    public override IEnumerator Visit(int gamblerId)
    {
        int thing = Random.Range(0, 2);
        int targetGambler1 = Random.Range(0, Gambler.spawned.Count);
        int targetGambler2 = targetGambler1;

        while (targetGambler1 == targetGambler2)
        {
            targetGambler2 = Random.Range(0, Gambler.spawned.Count);
        }

        int targetGambler1Id = PhotonNetwork.PlayerList[targetGambler1].ActorNumber;
        int targetGambler2Id = PhotonNetwork.PlayerList[targetGambler2].ActorNumber;

        int thingAmount1Id = -1;
        int thingAmount2Id = -1;

        switch (thing)
        {
            case 0:
                thingAmount1Id = Gambler.Get(targetGambler1Id).statistic.star;
                thingAmount2Id = Gambler.Get(targetGambler2Id).statistic.star;
                break;
            case 1:
                thingAmount1Id = Gambler.Get(targetGambler1Id).statistic.coin;
                thingAmount2Id = Gambler.Get(targetGambler2Id).statistic.coin;
                break;
        }

        _view.RPC("TradeCall", RpcTarget.Others, new object[] 
        { thing, targetGambler1Id, targetGambler2Id , thingAmount1Id, thingAmount2Id });
        yield return StartCoroutine(Trade(thing, targetGambler1Id, targetGambler2Id, thingAmount1Id, thingAmount2Id));

        yield return null;
    }

    [PunRPC]
    private void TradeCall(int thing, int targetGambler1, int targetGambler2,int thingAmount1Id,int thingAmount2Id)
    {
        StartCoroutine(Trade(thing, targetGambler1, targetGambler2, thingAmount1Id, thingAmount2Id));
    }

    IEnumerator Trade(int thing, int targetGambler1Id, int targetGambler2Id, int thingAmount1Id, int thingAmount2Id)
    {
        if (thing == 0)
        {
            Gambler targetGambler1 = Gambler.Get(targetGambler1Id);
            Gambler targetGambler2 = Gambler.Get(targetGambler2Id);

            CameraManager.instance.ChangePointOfView(targetGambler1Id);
            yield return delay02f;

            GameObject go = ObjectPoolingManager.instance.GetGo("Star");
            go.transform.position = targetGambler1.transform.position;
            go.transform.localScale = new Vector3(2,2,2);

            float t = 0;
            while (t <= 2f)
            {
                t += Time.deltaTime;
                go.transform.position = Vector3.Lerp(targetGambler1.transform.position,
                                                     targetGambler1.transform.position + new Vector3(0, 15f, 0),
                                                     t / 2f);
                yield return null;
            }
            targetGambler1.statistic.star = 0;
            CameraManager.instance.ChangePointOfView(targetGambler2Id);
            yield return delay02f;
            go.transform.position = targetGambler2.transform.position;
            t = 0;
            while (t <= 2f)
            {
                t += Time.deltaTime;
                go.transform.position = Vector3.Lerp(targetGambler2.transform.position,
                                                     targetGambler2.transform.position + new Vector3(0, 15f, 0),
                                                     t / 2f);
                yield return null;
            }
            targetGambler2.statistic.star = 0;
            go.SetActive(false);
            yield return delay02f;
            go.SetActive(true);
            t = 0;
            while (t <= 2f)
            {
                t += Time.deltaTime;
                go.transform.position = Vector3.Lerp(targetGambler2.transform.position + new Vector3(0, 15f, 0),
                                                     targetGambler2.transform.position,
                                                     t / 2f);
                yield return null;
            }
            targetGambler2.statistic.star = thingAmount1Id;
            CameraManager.instance.ChangePointOfView(targetGambler1Id);
            yield return delay02f;
            t = 0;
            while (t <= 2f)
            {
                t += Time.deltaTime;
                go.transform.position = Vector3.Lerp(targetGambler1.transform.position + new Vector3(0, 15f, 0),
                                                     targetGambler1.transform.position,
                                                     t / 2f);
                yield return null;
            }
            targetGambler1.statistic.star = thingAmount2Id;
            go.GetComponent<PoolAble>().ReleaseObject();
        }
        else
        {
            Gambler targetGambler1 = Gambler.Get(targetGambler1Id);
            Gambler targetGambler2 = Gambler.Get(targetGambler2Id);

            CameraManager.instance.ChangePointOfView(targetGambler1Id);
            yield return delay02f;

            GameObject go = ObjectPoolingManager.instance.GetGo("Coin");
            go.transform.position = targetGambler1.transform.position;
            go.transform.localScale = Vector3.one;

            float t = 0;
            while (t <= 2f)
            {
                t += Time.deltaTime;
                go.transform.position = Vector3.Lerp(targetGambler1.transform.position,
                                                     targetGambler1.transform.position + new Vector3(0, 15f, 0),
                                                     t / 2f);
                yield return null;
            }
            targetGambler1.statistic.coin = 0;
            CameraManager.instance.ChangePointOfView(targetGambler2Id);
            yield return delay02f;
            go.transform.position = targetGambler2.transform.position;
            t = 0;
            while (t <= 2f)
            {
                t += Time.deltaTime;
                go.transform.position = Vector3.Lerp(targetGambler2.transform.position,
                                                     targetGambler2.transform.position + new Vector3(0, 15f, 0),
                                                     t / 2f);
                yield return null;
            }
            targetGambler2.statistic.coin = 0;
            go.SetActive(false);
            yield return delay02f;
            go.SetActive(true);
            t = 0;
            while (t <= 2f)
            {
                t += Time.deltaTime;
                go.transform.position = Vector3.Lerp(targetGambler2.transform.position + new Vector3(0, 15f, 0),
                                                     targetGambler2.transform.position,
                                                     t / 2f);
                yield return null;
            }
            targetGambler2.statistic.coin = thingAmount1Id;
            CameraManager.instance.ChangePointOfView(targetGambler1Id);
            yield return delay02f;
            t = 0;
            while (t <= 2f)
            {
                t += Time.deltaTime;
                go.transform.position = Vector3.Lerp(targetGambler1.transform.position + new Vector3(0, 15f, 0),
                                                     targetGambler1.transform.position,
                                                     t / 2f);
                yield return null;
            }
            targetGambler1.statistic.coin = thingAmount2Id;
            go.GetComponent<PoolAble>().ReleaseObject();
        }
    }
}
