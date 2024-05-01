using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PhotonView))]
public class ItemDonationNode : NodeBase
{
    PhotonView _photonView;
    List<ItemCode> _items = new List<ItemCode>();
    YieldInstruction delay02f = new WaitForSeconds(0.2f);
    YieldInstruction delay05f = new WaitForSeconds(0.5f);


    private void Awake()
    {
        _photonView = GetComponent<PhotonView>();
    }

    public override IEnumerator Visit(int gamblerId)
    {
        _items.Clear();

        for (int i = 0; i < System.Enum.GetNames(typeof(ItemCode)).Length; i++)
        {
            if (ItemsManager.instance.Get((ItemCode)i).Count > 0)
            {
                _items.Add((ItemCode)i);
            }
        }
        if (_items.Count == 0)
        {
            yield break;
        }

        int random = Random.Range(0, _items.Count);
        int receiverId = gamblerId;

        while (receiverId == gamblerId)
        {
            receiverId = GameManager.instance.turnOrder[Random.Range(0, GameManager.instance.turnOrder.Length)];
        }

        _photonView.RPC("ItemDonationCall", RpcTarget.Others,
            new object[] { gamblerId, receiverId, (int)_items[random] });
        yield return StartCoroutine(ItemDonation(gamblerId, receiverId, (int)_items[random]));
    }

    [PunRPC]
    public void ItemDonationCall(int gamblerId, int receiverId, int itemCode)
    {
        StartCoroutine(ItemDonation(gamblerId, receiverId, itemCode));
    }

    IEnumerator ItemDonation(int giverId, int receiverId, int itemCode)
    {
        Gambler giver = Gambler.Get(giverId);
        Gambler receiver = Gambler.Get(receiverId);

        float t = 0;

        GameObject go = ObjectPoolingManager.instance.GetGo($"ItemModle_{(ItemCode)itemCode}");
        go.transform.position = giver.transform.position;
        go.transform.localScale = Vector3.one;

        giver.SetAnimation(GamblerAnimation.Sad);

        yield return delay05f;

        while (t <= 1.5f)
        {
            t += Time.deltaTime;
            go.transform.position = Vector3.Lerp(giver.transform.position + new Vector3(0, 5f, 0),
                                                 giver.transform.position + new Vector3(0, 15, 0),
                                                 t / 1.5f);
            yield return null;
        }

        CameraManager.instance.ChangePointOfView(receiverId);
        yield return delay02f;

        t = 0;
        go.transform.position = receiver.transform.position + new Vector3(0, 15, 0);

        receiver.SetAnimation(GamblerAnimation.ItemGet);

        while (t <= 1f)
        {
            t += Time.deltaTime;
            go.transform.position = Vector3.Lerp(receiver.transform.position + new Vector3(0, 15, 0),
                                                 receiver.transform.position + new Vector3(0, 5f, 0),
                                                 t / 1f);
            yield return null;
        }
        giver.ItemDelet((ItemCode)itemCode);
        receiver.ItemGetting((ItemCode)itemCode);
        go.GetComponent<PoolAble>().ReleaseObject();
        yield return delay05f;
        yield return delay05f;
    }
}
