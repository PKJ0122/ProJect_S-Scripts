using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisiterExceptCoinPlus : MonoBehaviour, ICoinRouletteEvent
{
    public string eventName => "�湮�� ��� ���� ���� ȹ��";

    public IEnumerator CoinEvent(int gamblerId, PhotonView view)
    {
        view.RPC("VisiterExceptCoinPlusEventCall", RpcTarget.All, gamblerId);
        Gambler.Get(gamblerId).SetAnimation(GamblerAnimation.Sad);
        yield return null;
    }
}