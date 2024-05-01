using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MinigameNode : NodeBase
{
    

    PhotonView _view;
    YieldInstruction delay = new WaitForSeconds(1);
    YieldInstruction delay05f = new WaitForSeconds(0.5f);


    private void Awake()
    {
        _view = GetComponent<PhotonView>();
    }

    public override IEnumerator Visit(int gamblerId)
    {
        yield return delay;

        int randomGame = Random.Range(0, System.Enum.GetNames(typeof(MiniGameKind)).Length);

        _view.RPC("GameStartCall", RpcTarget.Others, randomGame);
        yield return StartCoroutine(MiniGameManager.minigames[(MiniGameKind)randomGame].MiniGame());

        _view.RPC("WinnerAwardsCall", RpcTarget.Others);
        yield return StartCoroutine(WinnerAwards());
    }

    public IEnumerator WinnerAwards()
    {
        yield return null;

        List<Gambler> list = new List<Gambler>();
        foreach (KeyValuePair<int,Gambler> item in Gambler.spawned)
        {
            list.Add(item.Value);
        }

        list = list.OrderByDescending(p => p.statistic.MiniGameScore)
                   .ThenByDescending(p => GameManager.instance.turnOrder.Contains(p.view.OwnerActorNr))
                   .ToList();

        Gambler winner = list[0];

        CameraManager.instance.ChangePointOfView(winner.view.OwnerActorNr);
        winner.SetAnimation(GamblerAnimation.StarGet);
        UIManager.instance.Get<UIWinner>().Show();
        yield return delay05f;

        GameObject go = ObjectPoolingManager.instance.GetGo("Star");
        go.transform.position = winner.transform.position + new Vector3(0, 15, 0);
        go.transform.localScale = new Vector3(2, 2, 2);

        float t = 0;

        while (t <= 2.5f)
        {
            t += Time.deltaTime;
            go.transform.position = Vector3.Lerp(winner.transform.position + new Vector3(0, 15, 0),
                                                 winner.transform.position,
                                                 t / 2.5f);
            yield return null;
        }
        
        if (winner.view.IsMine)
            winner.statistic.star++;

        go.GetComponent<PoolAble>().ReleaseObject();

        yield return delay;
    }


    [PunRPC]
    private void GameStartCall(int minigameKind)
    {
        StartCoroutine(MiniGameManager.minigames[(MiniGameKind)minigameKind].MiniGame());
    }

    [PunRPC]
    private void WinnerAwardsCall()
    {
        StartCoroutine(WinnerAwards());
    }
}
