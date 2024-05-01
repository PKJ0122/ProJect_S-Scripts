using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class CoinRouletteNode : NodeBase
{
    public static Dictionary<int, ICoinRouletteEvent> coinEvents = new Dictionary<int, ICoinRouletteEvent>();
    YieldInstruction delay = new WaitForSeconds(1);
    YieldInstruction delay01f = new WaitForSeconds(0.1f);

    PhotonView _view;

    protected void Awake()
    {
        coinEvents.TryAdd(0, new AllCoinDistribution());
        coinEvents.TryAdd(1, new VisiterExceptCoinPlus());
        coinEvents.TryAdd(2, new FirsterCoinLasterGiveing());
        coinEvents.TryAdd(3, new ToLasterCoinGiveing());

        _view = GetComponent<PhotonView>();
    }

    public override IEnumerator Visit(int gamblerId)
    {
        yield return delay;

        int randomEvent = Random.Range(0, coinEvents.Count);
        _view.RPC("OtherCoinRoulette", RpcTarget.Others, new object[] { randomEvent , gamblerId });
        yield return StartCoroutine(Roulette(randomEvent, gamblerId));

        yield return StartCoroutine(coinEvents[randomEvent].CoinEvent(gamblerId, _view));
    }

    [PunRPC]
    private void OtherCoinRoulette(int eventNum , int gamblerId)
    {
        StartCoroutine(Roulette(eventNum, gamblerId));
    }

    IEnumerator Roulette(int eventNum, int gamblerId)
    {
        List<GameObject> rouletteobj = UIManager.instance.Get<UICoinRoulette>().coinEventGameObjects;
        UIManager.instance.Get<UICoinRoulette>().Show();

        for (int i = 0; i < 5; i++)
        {
            for (int j = 0; j < coinEvents.Count; j++)
            {
                rouletteobj[j].GetComponent<Image>().color = Color.yellow;
                yield return delay01f;
                rouletteobj[j].GetComponent<Image>().color = Color.blue;
            }
        }

        for (int i = 0; i < eventNum; i++)
        {
            rouletteobj[i].GetComponent<Image>().color = Color.yellow;
            yield return delay01f;
            rouletteobj[i].GetComponent<Image>().color = Color.blue;
        }

        rouletteobj[eventNum].GetComponent<Image>().color = Color.yellow;

        float t = 0;

        while (t <= 1f)
        {
            t += Time.deltaTime;
            rouletteobj[eventNum].transform.localScale = Vector3.Lerp(new Vector3(1.2f, 1.2f, 1.2f),
                                                                      Vector3.one,
                                                                      t / 1f);
            yield return null;
        }

        yield return delay;

        UIManager.instance.Get<UICoinRoulette>().Hide();
        rouletteobj[eventNum].GetComponent<Image>().color = Color.blue;
    }

    [PunRPC]
    void FirsterCoinLasterGiveingEventCall(int firsterId, int lasterId, int firstercoin, int lastercoin)
    {
        FirsterCoinLasterGiveing coinEvent = (FirsterCoinLasterGiveing)coinEvents[2];
        StartCoroutine(coinEvent.FirsterCoinLasterGiveingEvent(firsterId, lasterId, firstercoin, lastercoin));
    }

    [PunRPC]
    void AllCoinDistributionEventCall(int gamblerId ,int allcoin)
    {
        AllCoinDistribution coinEvent = (AllCoinDistribution)coinEvents[0];
        StartCoroutine(coinEvent.AllCoinDistributionEvent(gamblerId, allcoin));
    }

    [PunRPC]
    void ToLasterCoinGiveingEventCall(int[] gamblerData)
    {
        for (int i = 0; i < gamblerData.Length; i = i + 2)
        {
            Gambler.Get(gamblerData[i]).statistic.coin = gamblerData[i + 1];
        }
    }

    [PunRPC]
    void VisiterExceptCoinPlusEventCall(int gamblerId)
    {
        if (PhotonNetwork.LocalPlayer.ActorNumber != gamblerId)
        {
            Gambler.Get(PhotonNetwork.LocalPlayer.ActorNumber).statistic.coin += 10;
        }
    }
}
