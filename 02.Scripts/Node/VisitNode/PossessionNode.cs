using Photon.Pun;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class PossessionNode : NodeBase
{
    const int TOLL_FEE = 10;

    int _possessioner = -1;
    PhotonView _view;
    YieldInstruction delay02f = new WaitForSeconds(0.2f);


    protected void Awake()
    {
        _view = GetComponent<PhotonView>();
    }

    public override IEnumerator Visit(int gamblerId)
    {
        if (_possessioner == gamblerId)
        {
            yield break;
        }
        if (_possessioner == -1)
        {
            _view.RPC("SetPossessioner", RpcTarget.All, gamblerId);
            yield break;
        }

        if (Gambler.Get(gamblerId).statistic.coin <= 0)
            yield break;

        _view.RPC("TollFeeGiveCall", RpcTarget.Others, gamblerId);
        yield return StartCoroutine(TollFeeGive(gamblerId));

        yield return null;
    }

    [PunRPC]
    private void SetPossessioner(int gamblerId)
    {
        _possessioner = gamblerId;
        Transform[] gos = transform.GetComponentsInChildren<Transform>();
        for (int i = 1; i < gos.Length; i++)
        {
            gos[i].gameObject.SetActive(false);
        }
        CharEnum charEnum = (CharEnum)Gambler.Get(gamblerId).view.Owner.CustomProperties["isChar"];
        GameObject go = Resources.Load<GameObject>($"Nodes/{charEnum}Node");
        Instantiate(go, transform);
    }

    [PunRPC]
    private void TollFeeGiveCall(int visiter)
    {
        StartCoroutine(TollFeeGive(visiter));
    }

    IEnumerator TollFeeGive(int visiterId)
    {
        Gambler visiter = Gambler.Get(visiterId);
        Gambler possessioner = Gambler.Get(_possessioner);

        GameObject go = ObjectPoolingManager.instance.GetGo("Coin");
        go.transform.position = visiter.transform.position;
        go.transform.localScale = Vector3.one;

        float t = 0;

        visiter.SetAnimation(GamblerAnimation.Sad);
        while (t <= 1f)
        {
            t += Time.deltaTime;
            go.transform.position = Vector3.Lerp(visiter.transform.position,
                                                 visiter.transform.position + new Vector3(0, 15, 0),
                                                 t / 1f);
            yield return null;
        }
        int tollFee = Mathf.Min(visiter.statistic.coin, TOLL_FEE);

        visiter.statistic.coin -= tollFee;
        CameraManager.instance.ChangePointOfView(_possessioner);
        
        yield return delay02f;
        t = 0;
        possessioner.SetAnimation(GamblerAnimation.ItemGet);
        while (t <= 1f)
        {
            t += Time.deltaTime;
            go.transform.position = Vector3.Lerp(possessioner.transform.position + new Vector3(0, 15, 0),
                                                 possessioner.transform.position,
                                                 t / 1f);
            yield return null;
        }
        possessioner.statistic.coin += tollFee;

        go.GetComponent<PoolAble>().ReleaseObject();
        yield return delay02f;
    }
}
