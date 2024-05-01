using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpaceShipRace : MiniGameBase
{
    public override MiniGameKind miniGameKind => MiniGameKind.KeybordPushPush;
    GamblerStatistic statistic;
    RectTransform[] spaceShips = new RectTransform[4];

    YieldInstruction delay = new WaitForSeconds(1);
    YieldInstruction delay5 = new WaitForSeconds(5);


    protected override void Awake()
    {
        base.Awake();
        for (int i = 0; i < spaceShips.Length; i++)
        {
            spaceShips[i] = transform.Find($"spaceShip{i}").GetComponent<RectTransform>();
        }
    }

    void Update()
    {
        if (!isGameing)
            return;

        if (Input.GetKeyDown(KeyCode.Space))
            statistic.MiniGameScore++;
    }

    public override IEnumerator MiniGame()
    {
        isGameing = false;

        GameSetting();
        foreach (KeyValuePair<int, Gambler> item in Gambler.spawned)
            item.Value.statistic.MiniGameScore = 0;

        statistic.MiniGameScore = 0;

        GameTime = 30f;
        UIManager.instance.Get<UISpaceShipRace>().Show();

        for (int i = 3; i >= 0; i--)
        {
            StartCount = i;
            yield return delay;
        }

        isGameing = true;

        while (GameTime > 0)
        {
            GameTime -= Time.deltaTime;
            yield return null;
        }

        isGameing = false;
        yield return delay5;

        UIManager.instance.Get<UISpaceShipRace>().Hide();

        foreach (KeyValuePair<int, Gambler> item in Gambler.spawned)
            item.Value.statistic.ClearMiniGameEvent();
    }

    void GameSetting()
    {
        statistic = Gambler.Get(PhotonNetwork.LocalPlayer.ActorNumber).statistic;

        int myspaceShip = Array.IndexOf(GameManager.instance.turnOrder, PhotonNetwork.LocalPlayer.ActorNumber);

        for (int i = 0; i < GameManager.instance.turnOrder.Length; i++)
        {
            spaceShips[i].gameObject.SetActive(true);
            Gambler gambler = Gambler.Get(GameManager.instance.turnOrder[i]);
            //Image spaceShipFace = spaceShips[i].Find($"spaceShip{i}").GetComponent<Image>();
            Image spaceShipFace = spaceShips[i].GetComponent<Image>();
            int num = (int)gambler.view.Owner.CustomProperties["isChar"];
            spaceShipFace.sprite = Resources.Load<Sprite>($"UI/CharImage/re - char{num}");

            if (myspaceShip == i)
            {
                gambler.statistic.onMiniGameScoreChanged += value =>
                {
                    for (int j = 0; j < GameManager.instance.turnOrder.Length; j++)
                    {
                        if (myspaceShip == j)
                            continue;

                        int myScore = value;
                        int otherScore = Gambler.Get(GameManager.instance.turnOrder[j]).statistic.MiniGameScore;

                        spaceShips[j].anchoredPosition =
                        new Vector2(spaceShips[j].anchoredPosition.x,
                                    spaceShips[myspaceShip].anchoredPosition.y
                                    + (otherScore - myScore) * 3);
                    }
                };
                return;
            }
            gambler.statistic.onMiniGameScoreChanged += value =>
            {
                int myScore = Gambler.Get(GameManager.instance.turnOrder[myspaceShip]).statistic.MiniGameScore;
                int otherScore = value;

                spaceShips[i].anchoredPosition =
                new Vector2(spaceShips[i].anchoredPosition.x,
                            spaceShips[myspaceShip].anchoredPosition.y
                            + (otherScore - myScore) * 3);
            };
        }


    }
}
