using ExitGames.Client.Photon.StructWrapping;
using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToLasterCoinGiveing : MonoBehaviour , ICoinRouletteEvent
{
    const int AMOUNT = 7;

    public string eventName => "꼴등에게 코인 기부";

    public IEnumerator CoinEvent(int gamblerId, PhotonView view)
    {
        int lister = -1;
        int coin = 0;

        foreach (KeyValuePair<int,Gambler> item in Gambler.spawned)
        {
            if (item.Value.statistic.ranke == Gambler.spawned.Count - 1)
            {
                lister = item.Key;
                break;
            }
        }
        
        List<int> list = new List<int>();

        foreach (KeyValuePair<int, Gambler> item in Gambler.spawned)
        {
            if (item.Key == lister)
                continue;

            coin += Mathf.Min(item.Value.statistic.coin, AMOUNT);
            item.Value.statistic.coin -= Mathf.Min(item.Value.statistic.coin, AMOUNT);
            list.Add(item.Key);
            list.Add(item.Value.statistic.coin);
        }

        Gambler.Get(lister).statistic.coin += coin;
        list.Add(lister);
        list.Add(Gambler.Get(lister).statistic.coin);

        int[] array = list.ToArray();

        view.RPC("ToLasterCoinGiveingEventCall", RpcTarget.All, array);

        yield return null;
    }
}
