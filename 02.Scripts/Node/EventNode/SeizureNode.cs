using ExitGames.Client.Photon.StructWrapping;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SeizureNode : NodeBase, IPassby
{
    List<ItemCode> _items = new List<ItemCode>();
    PhotonView _view;
    WaitForSeconds deley = new WaitForSeconds(1);
    YieldInstruction delay05f = new WaitForSeconds(0.5f);

    void Awake()
    {
        _view = GetComponent<PhotonView>();
    }

    public IEnumerator PassBy(int gamblerId)
    {
        _items.Clear();

        for (int i = 0; i < System.Enum.GetNames(typeof(ItemCode)).Length; i++)
        {
            if (ItemsManager.instance.Get((ItemCode)i).Count > 0)
            {
                _items.Add((ItemCode)i);
            }
        }
        yield return deley;
        if (_items.Count == 0)
        {
            yield break;
        }
        int random = Random.Range(0, _items.Count);
        Gambler gambler = Gambler.Get(gamblerId);
        gambler.SetAnimation(GamblerAnimation.Sad);
        _view.RPC("SeizureCall", RpcTarget.Others, new object[] { (int)_items[random] , gamblerId });
        yield return StartCoroutine(Seizure((int)_items[random], gamblerId));
        gambler.ItemDelet(_items[random]);
        yield return deley;
    }

    [PunRPC]
    public void SeizureCall(int item , int gamblerId)
    {
        StartCoroutine(Seizure(item, gamblerId));
    }

    IEnumerator Seizure(int item, int gamblerId)
    {
        Gambler gambler = Gambler.Get(gamblerId);
        GameObject go = ObjectPoolingManager.instance.GetGo($"ItemModle_{(ItemCode)item}");
        go.transform.position = gambler.transform.position;
        go.transform.localScale = Vector3.one;

        float t = 0;
        while (t <= 1.5f)
        {
            t += Time.deltaTime;
            go.transform.position = Vector3.Lerp(gambler.transform.position,
                                                 gambler.transform.position + new Vector3(0, 5f, 0),
                                                 t / 1.5f);
            yield return null;
        }
        t = 0;
        yield return delay05f;
        while (t <= 1.5f)
        {
            t += Time.deltaTime;
            go.transform.position = Vector3.Lerp(gambler.transform.position + new Vector3(0, 5f, 0),
                                                 gambler.transform.position + new Vector3(0, 15f, 0),
                                                 t / 1.5f);
            yield return null;
        }
        go.GetComponent<PoolAble>().ReleaseObject();
    }
}