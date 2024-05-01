using Photon.Pun;
using System.Collections;
using UnityEngine;

public class ArmoryNode : NodeBase, IPassby
{
    PhotonView _view;
    WaitForSeconds delay = new WaitForSeconds(1);
    YieldInstruction delay05f = new WaitForSeconds(0.5f);


    private void Start()
    {
        _view = GetComponent<PhotonView>();
    }

    public IEnumerator PassBy(int gamblerId)
    {
        yield return delay;

        Gambler gambler = Gambler.Get(gamblerId);

        int item = Random.Range(12, 14);
        gambler.ItemGetting((ItemCode)item);

        gambler.SetAnimation(GamblerAnimation.ItemGet);

        _view.RPC("ArmoryGetCall", RpcTarget.Others, new object[] { item, gamblerId });
        yield return ArmoryGet(item, gamblerId);

        int nextStarNodeIndex = NodeCreater.instance.starNodeIndex;

        while (nextStarNodeIndex == NodeCreater.instance.armoryNodeIndex ||
            nextStarNodeIndex == NodeCreater.instance.starNodeIndex)
        {
            nextStarNodeIndex = Random.Range(0, NodeCreater.instance.nodeLength);
        }

        int nodeKind = Random.Range(0, NodeCreater.instance.nodeKindLength - 3);

        _view.RPC("ArmoryMove", RpcTarget.Others, new object[]
                                                       { nextStarNodeIndex , nodeKind , gamblerId });
        yield return StartCoroutine(NodeCreater.instance.NextArmoryNodeCreat
                                                        (nextStarNodeIndex, this, nodeKind, gamblerId));
    }

    [PunRPC]
    private void ArmoryMove(int nodeindex, int nodeKind, int gamblerId)
    {
        StartCoroutine(NodeCreater.instance.NextArmoryNodeCreat(nodeindex, this, nodeKind, gamblerId));
    }

    [PunRPC]
    private void ArmoryGetCall(int thing, int gamblerId)
    {
        StartCoroutine(ArmoryGet(thing, gamblerId));
    }

    IEnumerator ArmoryGet(int thing, int gamblerId)
    {
        Gambler gambler = Gambler.Get(gamblerId);

        GameObject go = ObjectPoolingManager.instance.GetGo($"ItemModle_{(ItemCode)thing}");
        go.transform.position = gambler.transform.position + new Vector3(0, 15f, 0);
        go.transform.localScale = Vector3.one;

        float t = 0;
        while (t <= 1.5f)
        {
            t += Time.deltaTime;
            go.transform.position = Vector3.Lerp(gambler.transform.position + new Vector3(0, 15f, 0),
                                                 gambler.transform.position + new Vector3(0, 5f, 0),
                                                 t / 1.5f);
            yield return null;
        }

        yield return delay05f;

        t = 0;
        while (t <= 1f)
        {
            t += Time.deltaTime;
            go.transform.position = Vector3.Lerp(gambler.transform.position + new Vector3(0, 5f, 0),
                                                 gambler.transform.position + new Vector3(0, 2f, 0), 
                                                 t / 1f);
            yield return null;
        }

        go.GetComponent<PoolAble>().ReleaseObject();
    }
}
