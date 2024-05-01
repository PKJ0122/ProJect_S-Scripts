using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SocialPlatforms;


public class AwardManager : SingletonMonoBase<AwardManager>
{
    const float TOTALTIME = 3f;

    public Transform awardZone;

    public Transform[] resultZone;
    public GameObject threeCamaer;
    public GameObject finalCamaer;

    Dictionary<int, GamblerData> gamblerDatas = new Dictionary<int, GamblerData>();
    Dictionary<int, Player> playerDic = new Dictionary<int, Player>();
    List<GamblerData> ranks = new List<GamblerData>();
    

    WaitForSeconds deley3s = new WaitForSeconds(3);
    WaitForSeconds deley = new WaitForSeconds(1);
    UIAward _ui;
    UIBlackLable _blackLable;

    protected override void Awake()
    {
        base.Awake();
        ObjectPoolingManager.instance.AddObjPool("Star", 1);

        foreach (Player player in PhotonNetwork.PlayerList)
        {
            playerDic.Add(player.ActorNumber, player);
        }
        _blackLable = UIManager.instance.Get<UIBlackLable>();
    }

    private void Start()
    {
        for (int i = 0; i < Enum.GetNames(typeof(CharEnum)).Length; i++)
        {
            ObjectPoolingManager.instance.AddObjPool($"CharModle/{(CharEnum)i}", 1);
        }
        gamblerDatas = DataSaver.instance.gamblers;
        _ui = UIManager.instance.Get<UIAward>();

        _ui.Show();
        StartCoroutine(AwardLogic());
    }

    public IEnumerator AwardLogic()
    {
        yield return deley3s;
        yield return deley3s;
        yield return StartCoroutine(DmageAward());
        yield return StartCoroutine(KillAward());
        yield return StartCoroutine(FinalAward());
        yield return deley3s;       
        yield return deley3s;
        _blackLable.Show();
        _ui.Hide();
        yield return deley3s;
        yield return StartCoroutine(LastZone());
        yield return deley3s;
        yield return deley3s; 
        yield return deley3s;
        foreach(KeyValuePair<int, Player> player in playerDic)
        {
            if(player.Value == PhotonNetwork.MasterClient)
            {
                PhotonNetwork.LoadLevel("Lobby");
            }
        }

    }

    GameObject CharAward(int actNum)
    {
        Player awardChar = playerDic[actNum];
        int charnum = (int)awardChar.CustomProperties["isChar"];
        GameObject _obj = ObjectPoolingManager.instance.GetGo($"CharModle/{(CharEnum)charnum}");
        _obj.transform.position = awardZone.position;
        _obj.GetComponent<Animator>().SetInteger("State", (int)GamblerAnimation.StarGet);
        return _obj;
    }

    public IEnumerator StarAward(int actNum)
    {
        GameObject obj = CharAward(actNum);
        GamblerData data = gamblerDatas[actNum];
        data.Star++;
        gamblerDatas[actNum] = data;
        GameObject star = ObjectPoolingManager.instance.GetGo("Star");
        star.transform.position = awardZone.position + new Vector3(0, 10, 0);
        yield return deley;
        yield return StartCoroutine(StarDown(star, obj));      
    }

    public IEnumerator StarDown(GameObject star, GameObject obj)
    {
        float t = 0;
        Vector3 pos = star.transform.position;
        while (t <= TOTALTIME)
        {
            t += Time.deltaTime;

            // 선형 보간을 사용하여 현재 위치를 계산합니다.
            star.transform.position = Vector3.Lerp(pos, obj.transform.position, t / TOTALTIME);
            yield return null;
        }
        star.GetComponent<PoolAble>().ReleaseObject();
        yield return deley;
        obj.GetComponent<PoolAble>().ReleaseObject();
    }


    public IEnumerator DmageAward()
    {
        int first = 0;
        int actNum = 0;
        foreach (KeyValuePair<int, GamblerData> item in gamblerDatas)
        {
            if (first >= item.Value.Dmage)
            {

            }
            else
            {
                first = item.Value.Dmage;
                actNum = item.Key;
            }
        }
        _ui.SetInfo(playerDic[actNum].NickName, "데미지 어워드", "플레이어에게 가장 많은 데미지를 입힌 플레이어.");
        yield return StartCoroutine(StarAward(actNum));
    }

    IEnumerator KillAward()
    {
        int first = 0;
        int actNum = 0;
        foreach (KeyValuePair<int, GamblerData> item in gamblerDatas)
        {
            if (first >= item.Value.Kill)
            {

            }
            else
            {
                first = item.Value.Kill;
                actNum = item.Key;
            }
        }
        _ui.SetInfo(playerDic[actNum].NickName, "킬 어워드", "플레이어를 가장 많이 살해한 플레이어.");
        yield return StartCoroutine(StarAward(actNum));
    }

    IEnumerator FinalAward()
    {
        int first = 0;
        int actNum = PhotonNetwork.LocalPlayer.ActorNumber;
        foreach (KeyValuePair<int, GamblerData> item in gamblerDatas)
        {
            if (first > item.Value.Star)
            {

            }
            else if (first == item.Value.Star)
            {
                if (gamblerDatas[actNum].Coin < item.Value.Coin)
                {
                    first = item.Value.Star;
                    actNum = item.Key;
                }
                else if(gamblerDatas[actNum].Coin == item.Value.Coin)
                {
                    first = item.Value.Star;
                    actNum = item.Key;
                }
            }
            else
            {
                first = item.Value.Star;
                actNum = item.Key;
            }
        }
        _ui.SetInfo($"{playerDic[actNum].NickName} 우승 !!", "최종 우승", "고난과 역경을 딛고 최종 우승을 한 플레이어 !!");
        CharAward(actNum);
        SetRank();
        yield return null;        
    }

    public IEnumerator LastZone()
    {
        threeCamaer.SetActive(false);
        finalCamaer.SetActive(true);
        yield return deley;
        _blackLable.Hide();

        for (int i = 0; i < ranks.Count; i++)
        {
            Player awardChar = playerDic[ranks[i].Id];
            int charnum = (int)awardChar.CustomProperties["isChar"];
            GameObject _obj = ObjectPoolingManager.instance.GetGo($"CharModle/{(CharEnum)charnum}");
            _obj.transform.position = resultZone[i].position;
            if (i == ranks.Count - 1)
            {
                _obj.transform.position = resultZone[resultZone.Length - 1].position;
            }
            _obj.GetComponent<Animator>().SetInteger("State", (int)GamblerAnimation.StarGet);
        }

        yield return null;
    }

    void SetRank()
    {
        foreach (KeyValuePair<int, GamblerData> item in gamblerDatas)
        {
            ranks.Add(item.Value);
        }
        ranks = ranks.OrderByDescending(p => p.Star)
                   .ThenByDescending(p => p.Coin)
                   .ThenByDescending(p => p.Kill)
                   .ThenByDescending(p => p.Dmage)
                   .ThenByDescending(p => p.Id)
                   .ToList();
    }
}
