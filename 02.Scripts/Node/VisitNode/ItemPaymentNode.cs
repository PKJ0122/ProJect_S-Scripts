using Photon.Pun;
using System.Collections;
using UnityEngine;

public class ItemPaymentNode : NodeBase
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
        Gambler.Get(gamblerId).SetAnimation(GamblerAnimation.ItemGet);
        int random = Random.Range(0, System.Enum.GetNames(typeof(ItemCode)).Length - 2);
        Gambler.Get(gamblerId).ItemGetting((ItemCode)random);

        _view.RPC("PaymentCall", RpcTarget.Others, new object[] { gamblerId, random });
        yield return StartCoroutine(Payment(gamblerId, random));
    }

    [PunRPC]
    void PaymentCall(int gamblerId, int itemCode)
    {
        StartCoroutine(Payment(gamblerId, itemCode));
    }

    IEnumerator Payment(int gamblerId, int itemCode)
    {
        Gambler gambler = Gambler.Get(gamblerId);

        GameObject go = ObjectPoolingManager.instance.GetGo($"ItemModle_{(ItemCode)itemCode}");
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
