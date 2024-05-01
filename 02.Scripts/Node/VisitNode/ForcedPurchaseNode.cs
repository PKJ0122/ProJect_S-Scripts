using Photon.Pun;
using System.Collections;
using UnityEngine;

public class ForcedPurchaseNode : NodeBase
{
    PhotonView _view;

    WaitForSeconds deley = new WaitForSeconds(1);
    WaitForSeconds deley05f = new WaitForSeconds(0.5f);


    private void Awake()
    {
        _view = GetComponent<PhotonView>();
    }

    public override IEnumerator Visit(int gamblerId)
    {
        if (Gambler.Get(gamblerId).statistic.coin > 0)
        {
            int randomitems = Random.Range(0, System.Enum.GetNames(typeof(ItemCode)).Length - 2);
            yield return deley;
            _view.RPC("ForcedPurchaseCall", RpcTarget.Others, new object[] { randomitems, gamblerId });
            yield return StartCoroutine(ForcedPurchase(randomitems, gamblerId));
            ItemsManager.instance.Get((ItemCode)randomitems).Count++;
        }
        yield return deley;
    }

    [PunRPC]
    void ForcedPurchaseCall(int item, int gamblerId)
    {
        StartCoroutine(ForcedPurchase(item, gamblerId));
    }

    IEnumerator ForcedPurchase(int item, int gamblerId)
    {
        float t = 0;
        GameObject coingo = ObjectPoolingManager.instance.GetGo("Coin");
        Gambler gambler = Gambler.Get(gamblerId);
        coingo.transform.position = gambler.transform.position;
        gambler.SetAnimation(GamblerAnimation.Sad);
        while (t <= 1f)
        {
            t += Time.deltaTime;
            coingo.transform.position = Vector3.Lerp(gambler.transform.position,
                                                     gambler.transform.position + new Vector3(0, 15, 0),
                                                     t / 1f);
            yield return null;
        }
        coingo.GetComponent<PoolAble>().ReleaseObject();
        gambler.statistic.coin = 0;
        t = 0;
        GameObject itemgo = ObjectPoolingManager.instance.GetGo($"ItemModle_{(ItemCode)item}");
        while (t <= 1.5f)
        {
            t += Time.deltaTime;
            itemgo.transform.position = Vector3.Lerp(gambler.transform.position + new Vector3(0, 15, 0),
                                                     gambler.transform.position + new Vector3(0, 5f, 0),
                                                     t / 1.5f);
            yield return null;
        }
        yield return deley05f;
        t = 0;
        while (t <= 1.5f)
        {
            t += Time.deltaTime;
            itemgo.transform.position = Vector3.Lerp(gambler.transform.position + new Vector3(0, 5f, 0),
                                                     gambler.transform.position + new Vector3(0, 2f, 0),
                                                     t / 1.5f);
            yield return null;
        }
        itemgo.GetComponent<PoolAble>().ReleaseObject();
    }
}
