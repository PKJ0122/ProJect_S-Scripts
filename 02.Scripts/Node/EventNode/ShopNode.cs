using JetBrains.Annotations;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopNode : NodeBase, IPassby
{
    PhotonView _view;
    int buyItem = -1;

    WaitForSeconds deley = new WaitForSeconds(1);
    YieldInstruction delay05f = new WaitForSeconds(0.5f);

    private void Awake()
    {
        UIManager.instance.Get<UIShop>().onBuyItem += value =>
        {
            buyItem = value;
        };
        _view = GetComponent<PhotonView>();
    }

    public IEnumerator PassBy(int gamblerId)
    {
        buyItem = -1;
        yield return deley;
        UIManager.instance.Get<UIShop>().Show();
        yield return new WaitUntil(() => UIManager.instance.Get<UIShop>().isShow == false);

        if (buyItem == -1)
        {
            yield break;
        }

        Gambler.Get(gamblerId).Front();
        _view.RPC("BuyCall", RpcTarget.Others, new object[] { buyItem, gamblerId });
        yield return StartCoroutine(Buy(buyItem, gamblerId));
    }

    [PunRPC]
    private void BuyCall(int item,int gamblerId)
    {
        StartCoroutine(Buy(item, gamblerId));
    }

    IEnumerator Buy(int item, int gamblerId)
    {
        Gambler gambler = Gambler.Get(gamblerId);
        GameObject go = ObjectPoolingManager.instance.GetGo($"ItemModle_{(ItemCode)item}");
        go.transform.position = gambler.transform.position;
        go.transform.localScale = gambler.transform.localScale;

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
        while (t <= 1.5f)
        {
            t += Time.deltaTime;
            go.transform.position = Vector3.Lerp(gambler.transform.position + new Vector3(0, 5f, 0),
                                                 gambler.transform.position + new Vector3(0, 2f, 0),
                                                 t / 1.5f);
            yield return null;
        }
        go.GetComponent<PoolAble>().ReleaseObject();
    }
}
