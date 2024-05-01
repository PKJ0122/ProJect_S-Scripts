using Photon.Pun;
using System.Collections;
using UnityEngine;
using UnityEngine.Profiling;

public class WarpNode : NodeBase, IPassby
{
    PhotonView _view;
    NodeInfo _nodeInfo;

    YieldInstruction delay = new WaitForSeconds(2f);

    [SerializeField] Transform floor;

    void Awake()
    {
        _view = GetComponent<PhotonView>();
        _nodeInfo = GetComponent<NodeInfo>();

        ObjectPoolingManager.instance.AddObjPool("Pipe", 1);
    }

    public IEnumerator PassBy(int gamblerId)
    {
        int nodeId = -1;
        bool isok = false;

        while (!isok || _nodeInfo.nodeId == nodeId)
        {
            nodeId = Random.Range(0, NodeCreater._nodes.Count);
            if (NodeCreater._nodes.TryGetValue(nodeId, out NodeInfo nodeInfo))
            {
                isok = true;
            }
        }

        Gambler.Get(gamblerId).SetAnimationNo(GamblerAnimation.Jump);
        _view.RPC("WarpLogicCall", RpcTarget.Others, new object[] { gamblerId, nodeId });
        yield return StartCoroutine(WarpLogic(gamblerId, nodeId));
    }

    [PunRPC]
    public void WarpLogicCall(int gamblerId, int nodeId)
    {
        StartCoroutine(WarpLogic(gamblerId, nodeId));
    }

    YieldInstruction delay05f = new WaitForSeconds(0.5f);

    IEnumerator WarpLogic(int gamblerId, int nodeId)
    {
        Gambler gambler = Gambler.Get(gamblerId);
        NodeInfo node = NodeCreater.Get(nodeId);
        Vector3 midle = Vector3.Lerp(transform.position, floor.position, 0.5f) + new Vector3(0, 3f, 0);

        gambler.navMeshAgent.enabled = false;
        gambler.currentNode = node;

        yield return delay05f;

        float t = 0;

        while (t <= 2f)
        {
            t += Time.deltaTime;
            gambler.transform.position = Vector3.Lerp(
                Vector3.Lerp(transform.position, midle, t / 2f),
                Vector3.Lerp(midle, floor.position, t / 2f)
                , t / 2f);
            yield return null;
        }

        t = 0;

        while (t <= 0.7f)
        {
            t += Time.deltaTime;
            gambler.transform.position = Vector3.Lerp(floor.position,
                                                      floor.position - new Vector3(0, 8, 0),
                                                      t / 0.7f);
            yield return null;
        }
        GameObject go = ObjectPoolingManager.instance.GetGo("Pipe");
        go.transform.position = node.transform.position + new Vector3(0, 10, 0);

        CameraManager.instance.ChangePointOfView(node.transform);

        t = 0;
        yield return delay;

        while (t <= 1f)
        {
            t += Time.deltaTime;
            gambler.transform.position = Vector3.Lerp(go.transform.position,
                                                      node.transform.position,
                                                      t / 1f);
            yield return null;
        }
        yield return delay05f;

        go.GetComponent<PoolAble>().ReleaseObject();
        CameraManager.instance.ChangePointOfView();
        Gambler.Get(gamblerId).navMeshAgent.enabled = true;
    }
}