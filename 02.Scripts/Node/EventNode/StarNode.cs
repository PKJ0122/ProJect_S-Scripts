using Photon.Pun;
using System.Collections;
using System.Drawing;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;

public class StarNode : NodeBase, IPassby
{
    const int STAR_PRICE = 30;

    PhotonView _view;
    YieldInstruction delay = new WaitForSeconds(2f);


    private void Start()
    {
        _view = GetComponent<PhotonView>();
    }

    public IEnumerator PassBy(int gamblerId)
    {
        yield return null;

        Gambler gambler = Gambler.Get(gamblerId);

        if (gambler.statistic.coin < STAR_PRICE)
        {
            gambler.SetAnimation(GamblerAnimation.Sad);
            yield return delay;
            yield break;
        }

        gambler.SetAnimation(GamblerAnimation.StarGet);

        _view.RPC("StarGetCall", RpcTarget.Others);
        yield return StartCoroutine(StarGet());

        gambler.statistic.coin -= STAR_PRICE;
        gambler.statistic.star++;

        int nextStarNodeIndex = NodeCreater.instance.armoryNodeIndex;

        while (nextStarNodeIndex == NodeCreater.instance.armoryNodeIndex ||
            nextStarNodeIndex == NodeCreater.instance.starNodeIndex)
        {
            nextStarNodeIndex = Random.Range(0, NodeCreater.instance.nodeLength);
        }

        int nodeKind = Random.Range(0, NodeCreater.instance.nodeKindLength - 3);

        _view.RPC("StarMove", RpcTarget.Others, new object[]
                                                       { nextStarNodeIndex , nodeKind , gamblerId });
        yield return StartCoroutine(NodeCreater.instance.NextStarNodeCreat
                                                        (nextStarNodeIndex, this, nodeKind, gamblerId));
    }

    [PunRPC]
    private void StarMove(int nodeindex, int nodeKind , int gamblerId)
    {
        StartCoroutine(NodeCreater.instance.NextStarNodeCreat(nodeindex,this,nodeKind,gamblerId));
    }

    IEnumerator StarGet()
    {
        yield return delay;

        float t = 0;
        while (t <= 3f)
        {
            t += Time.deltaTime;
            Transform starGo = NodeCreater.instance.starGameObject.transform;
            Vector3 starGoV = starGo.position;
            starGo.position = Vector3.Lerp(starGoV, starGoV - new Vector3(0, 4, 0), t / 3f);
            yield return null;
        }
    }

    [PunRPC]
    void StarGetCall()
    {
        StartCoroutine(StarGet());
    }
}
