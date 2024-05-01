using Photon.Pun;
using System.Collections;
using UnityEngine;
using UnityEngine.Profiling;

public class VolunteerNode : NodeBase, IPassby
{
    const int price = 5;
    bool select;
    bool isGive;
    GameObject _beggarGameObject;

    PhotonView _view;

    YieldInstruction delay = new WaitForSeconds(1);
    YieldInstruction delay05f = new WaitForSeconds(0.5f);


    string[] luckyreward = new string[] { "StarGive", "ItemGive", "ItemGive", "ItemGive", "ItemGive", "ItemGive", "RareItemGive" };
    string[] badreward = new string[] { "MoneySeizure", "MoneySeizure", "MoneySeizure", "StarSeizure" };


    public void Awake()
    {
        _view = GetComponent<PhotonView>();

        UIManager.instance.Get<UIVolunteer>().onButtonClick += (value =>
        {
            select = true;
            isGive = value;
        });
    }

    public IEnumerator PassBy(int gamblerId)
    {
        select = false;
        isGive = false;

        Gambler gambler = Gambler.Get(gamblerId);
        int nodeId = gambler.currentNode.nodeId;

        _view.RPC("BeggarCreat", RpcTarget.All, nodeId);

        yield return delay;
        gambler.transform.LookAt(_beggarGameObject.transform);

        if (gambler.statistic.coin < price)
        {
            _view.RPC("BeggarRemove", RpcTarget.All);
            yield break;
        }

        UIManager.instance.Get<UIVolunteer>().Show();

        yield return new WaitUntil(() => select);

        if (!isGive)
        {
            _view.RPC("BeggarRemove", RpcTarget.All);
            yield break;
        }

        gambler.statistic.coin -= price;
        int thing = Random.Range(0, 10);
        int thing2 = Reward(thing);
        int itemCode = thing2 == luckyreward.Length - 1 ? Random.Range(12, 14) : Random.Range(0, 12);

        _view.RPC("BeggarActionStart", RpcTarget.Others, new object[] 
        { thing, thing2, gamblerId , itemCode});
        yield return StartCoroutine(BeggarAction(thing, thing2, gamblerId, itemCode));

        string result = thing < 8 ? luckyreward[thing2] : badreward[thing2];

        switch (result)
        {
            case "ItemGive":
                gambler.ItemGetting((ItemCode)itemCode);
                break;
            case "RareItemGive":
                gambler.ItemGetting((ItemCode)itemCode);
                break;
            case "StarGive":
                gambler.statistic.star++;
                break;
            case "MoneySeizure":
                gambler.statistic.coin = 0;
                break;
            case "StarSeizure":
                if (gambler.statistic.star > 0)
                    gambler.statistic.star--;
                break;
        }
    }

    private int Reward(int thing)
    {
        int reward = -1;

        if (thing < 8)
        {
            reward = Random.Range(0, luckyreward.Length);
            return reward;
        }
        else
        {
            reward = Random.Range(0, badreward.Length);
            return reward;
        }
    }

    [PunRPC]
    private void BeggarCreat(int nodeId)
    {
        _beggarGameObject = ObjectPoolingManager.instance.GetGo("Beggar");
        NodeInfo beggarNode = NodeCreater.Get(nodeId);
        Vector3 node1 = beggarNode.transform.position;
        Vector3 node2 = beggarNode.nextNode.transform.position;
        _beggarGameObject.transform.position = Vector3.Lerp(node1, node2, 0.5f);
        _beggarGameObject.transform.LookAt(node1);
    }

    [PunRPC]
    private void BeggarRemove()
    {
        _beggarGameObject.GetComponent<PoolAble>().ReleaseObject();
        _beggarGameObject = null;
    }


    [PunRPC]
    private void BeggarActionStart(int thing, int thing2, int gamblerId,int itemCode)
    {
        StartCoroutine(BeggarAction(thing, thing2, gamblerId, itemCode));
    }

    IEnumerator BeggarAction(int thing, int thing2, int gamblerId , int itemCode)
    {
        if (thing < 8)
        {
            GameObject go = ObjectPoolingManager.instance.GetGo("Drool");
            go.transform.position = _beggarGameObject.transform.position + new Vector3(0, 7, 0);
            #region º° È¹µæ ·ÎÁ÷
            if (thing2 == 0)
            {
                yield return delay;
                GameObject stargo = ObjectPoolingManager.instance.GetGo("Star");
                stargo.transform.position = _beggarGameObject.transform.position + new Vector3(0, 15, 0);
                stargo.transform.localScale = new Vector3(2, 2, 2);
                float t = 0;
                while (t <= 1.5f)
                {
                    t += Time.deltaTime;
                    stargo.transform.position =
                        Vector3.Lerp(_beggarGameObject.transform.position + new Vector3(0, 15, 0),
                                     _beggarGameObject.transform.position + new Vector3(0, 7, 0),
                                     t / 1.5f);
                    yield return null;
                }
                yield return delay05f;
                t = 0;
                Vector3 midle = Vector3.Lerp(stargo.transform.position,
                                             Gambler.Get(gamblerId).transform.position + new Vector3(0, 7, 0),
                                             0.5f) + new Vector3(0, 3, 0);
                while (t <= 1.5f)
                {
                    t += Time.deltaTime;
                    stargo.transform.position =
                        Vector3.Lerp(Vector3.Lerp(_beggarGameObject.transform.position + new Vector3(0, 7, 0),
                                                  midle,
                                                  t / 1.5f),
                                     Vector3.Lerp(midle,
                                                  Gambler.Get(gamblerId).transform.position + new Vector3(0, 7, 0),
                                                  t / 1.5f),
                                     t / 1.5f);
                    yield return null;
                }
                t = 0;
                Gambler.Get(gamblerId).SetAnimation(GamblerAnimation.StarGet);
                while (t <= 3f)
                {
                    t += Time.deltaTime;
                    Vector3 starGoV = stargo.transform.position;
                    Vector3 starGoS = stargo.transform.localScale;
                    stargo.transform.position = Vector3.Lerp(starGoV, starGoV - new Vector3(0, 4, 0), t / 3f);
                    stargo.transform.localScale = Vector3.Lerp(starGoS, new Vector3(0.5f, 0.5f, 0.5f), t / 3f);
                    yield return null;
                }
                stargo.GetComponent<PoolAble>().ReleaseObject();
            }
            #endregion
            #region ¾ÆÀÌÅÛ È¹µæ ·ÎÁ÷
            else
            {
                yield return delay;
                GameObject itemgo = ObjectPoolingManager.instance.GetGo($"ItemModle_{(ItemCode)itemCode}");
                itemgo.transform.position = _beggarGameObject.transform.position + new Vector3(0, 15, 0);
                itemgo.transform.localScale = Vector3.one;
                Gambler.Get(gamblerId).SetAnimation(GamblerAnimation.ItemGet);
                float t2 = 0;
                while (t2 <= 1.5f)
                {
                    t2 += Time.deltaTime;
                    itemgo.transform.position =
                        Vector3.Lerp(_beggarGameObject.transform.position + new Vector3(0, 15, 0),
                                     _beggarGameObject.transform.position + new Vector3(0, 7, 0),
                                     t2 / 1.5f);
                    yield return null;
                }
                
                yield return delay05f;
                t2 = 0;
                Vector3 midle2 = Vector3.Lerp(itemgo.transform.position,
                                             Gambler.Get(gamblerId).transform.position + new Vector3(0, 7, 0),
                                             0.5f) + new Vector3(0, 3, 0);
                while (t2 <= 1.5f)
                {
                    t2 += Time.deltaTime;
                    itemgo.transform.position =
                        Vector3.Lerp(Vector3.Lerp(_beggarGameObject.transform.position + new Vector3(0, 7, 0),
                                                  midle2,
                                                  t2 / 1.5f),
                                     Vector3.Lerp(midle2,
                                                  Gambler.Get(gamblerId).transform.position + new Vector3(0, 7, 0),
                                                  t2 / 1.5f),
                                     t2 / 1.5f);
                    yield return null;
                }
                t2 = 0;
                while (t2 <= 1.5f)
                {
                    t2 += Time.deltaTime;
                    Vector3 starGoV = itemgo.transform.position;
                    Vector3 starGoS = itemgo.transform.localScale;
                    itemgo.transform.position = Vector3.Lerp(starGoV, starGoV - new Vector3(0, 4, 0), t2 / 1.5f);
                    itemgo.transform.localScale = Vector3.Lerp(starGoS, new Vector3(0.5f, 0.5f, 0.5f), t2 / 1.5f);
                    yield return null;
                }
                itemgo.GetComponent<PoolAble>().ReleaseObject();
            }
            #endregion
        }
        else
        {
            GameObject go = ObjectPoolingManager.instance.GetGo("Devil");
            go.transform.position = _beggarGameObject.transform.position + new Vector3(0, 7, 0);
            Gambler gambler = Gambler.Get(gamblerId);
            gambler.SetAnimation(GamblerAnimation.Sad);
            #region º° »¯±â´Â ·ÎÁ÷
            if (thing2 == badreward.Length - 1)
            {
                float t = 0;
                if (gambler.statistic.star > 0)
                {
                    GameObject stargo = ObjectPoolingManager.instance.GetGo("Star");
                    stargo.transform.position = gambler.transform.position;
                    stargo.transform.localScale = new Vector3(2,2,2);
                    while (t <= 2f)
                    {
                        t += Time.deltaTime;
                        stargo.transform.position = Vector3.Lerp(gambler.transform.position,
                                                                 gambler.transform.position + new Vector3(0, 15, 0),
                                                                 t / 2f);
                        yield return null;
                    }
                    stargo.GetComponent<PoolAble>().ReleaseObject();
                }
                else
                {
                    yield return delay;
                }
            }
            #endregion
            #region µ·»¯±â´Â ·ÎÁ÷
            else
            {
                float t = 0;
                GameObject coingo = ObjectPoolingManager.instance.GetGo("Coin");
                coingo.transform.position = gambler.transform.position;
                coingo.transform.localScale = Vector3.one;
                while (t <= 2f)
                {
                    t += Time.deltaTime;
                    coingo.transform.position = Vector3.Lerp(gambler.transform.position,
                                                             gambler.transform.position + new Vector3(0, 15, 0),
                                                             t / 2f);
                    yield return null;
                }
                coingo.GetComponent<PoolAble>().ReleaseObject();
            }
            #endregion
        }
        yield return delay;
        _beggarGameObject.GetComponent<PoolAble>().ReleaseObject();
        _beggarGameObject = null;
    }
}