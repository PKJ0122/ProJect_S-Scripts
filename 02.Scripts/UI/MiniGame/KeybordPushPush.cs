using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class KeybordPushPush : MiniGameBase
{
    float _isDamage;
    int[] _arrows;

    Slider _penalty;

    public float isDamage
    {
        get => _isDamage;
        set
        {
            _isDamage = value;
            onIsDamageChaned?.Invoke(value);
        }
    }

    PhotonView view;

    GamblerStatistic _statistic;
    Sprite[] _arrowsSprite = new Sprite[4];

    YieldInstruction delay = new WaitForSeconds(1);
    YieldInstruction delay5 = new WaitForSeconds(5);

    public override MiniGameKind miniGameKind => MiniGameKind.KeybordPushPush;
    public event Action<float> onIsDamageChaned;

    protected override void Awake()
    {
        base.Awake();
        view = GetComponent<PhotonView>();
        for (int i = 0; i < _arrowsSprite.Length; i++)
        {
            _arrowsSprite[i] = Resources.Load<Sprite>($"Arrows/Arrow{i}");
        }

        _penalty = transform.Find("Slider - Penalty").GetComponent<Slider>();
        onIsDamageChaned += value =>
        {
            if (value <= 0)
            {
                _penalty.gameObject.SetActive(false);
                return;
            }
            if (value == 1f)
            {
                _penalty.gameObject.SetActive(true);
            }

            _penalty.value = value;
        };
    }

    void Update()
    {
        if (!isGameing)
            return;

        if (isDamage > 0)
        {
            isDamage -= Time.deltaTime;
            return;
        }

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (_arrows[_statistic.MiniGameScore] == 0)
                _statistic.MiniGameScore++;
            else
                isDamage = 1f;
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if (_arrows[_statistic.MiniGameScore] == 1)
                _statistic.MiniGameScore++;
            else
                isDamage = 1f;
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (_arrows[_statistic.MiniGameScore] == 2)
                _statistic.MiniGameScore++;
            else
                isDamage = 1f;
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            if (_arrows[_statistic.MiniGameScore] == 3)
                _statistic.MiniGameScore++;
            else
                isDamage = 1f;
        }
    }

    public override IEnumerator MiniGame()
    {
        isGameing = false;
        isDamage = 0;

        if (PhotonNetwork.IsMasterClient)
        {
            int[] arr = new int[200];
            for (int i = 0; i < arr.Length; i++)
            {
                arr[i] = Random.Range(0, 4);
            }

            view.RPC("SetArrow", RpcTarget.All, arr);
        }

        yield return new WaitUntil(() => _arrows != null);

        GameSetting();
        foreach (KeyValuePair<int, Gambler> item in Gambler.spawned)
            item.Value.statistic.MiniGameScore = 0;
        GameTime = 30f;
        UIManager.instance.Get<UIKeybordPushPush>().Show();

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

        UIManager.instance.Get<UIKeybordPushPush>().Hide();

        foreach (KeyValuePair<int, Gambler> item in Gambler.spawned)
            item.Value.statistic.ClearMiniGameEvent();

        _arrows = null;
    }

    void GameSetting()
    {
        _statistic = Gambler.Get(PhotonNetwork.LocalPlayer.ActorNumber).statistic;
        Transform gamer = transform.Find("MyGambler").GetComponent<Transform>();
        Image[] arrows = new Image[4];
        for (int i = 0; i < arrows.Length; i++)
        {
            arrows[i] = gamer.transform.Find($"Arrow{i}").GetComponent<Image>();
        }
        TMP_Text score = gamer.transform.Find($"Text (TMP) - Score").GetComponent<TMP_Text>();
        Image face = gamer.transform.Find($"Face").GetComponent<Image>();
        int num = (int)_statistic.GetComponent<Gambler>().view.Owner.CustomProperties["isChar"];
        face.sprite = Resources.Load<Sprite>($"UI/CharImage/re - char{num}");
        _statistic.onMiniGameScoreChanged += value =>
        {
            arrows[0].sprite = _arrowsSprite[_arrows[value]];
            arrows[1].sprite = _arrowsSprite[_arrows[value + 1]];
            arrows[2].sprite = _arrowsSprite[_arrows[value + 2]];
            arrows[3].sprite = _arrowsSprite[_arrows[value + 3]];
            score.text = $"{value}";
        };
        Transform[] ohterGamer = new Transform[3];
        int j = 0;
        foreach (KeyValuePair<int, Gambler> item in Gambler.spawned)
        {
            if (item.Key == PhotonNetwork.LocalPlayer.ActorNumber)
                continue;

            ohterGamer[j] = transform.Find($"OtherGambler{j}").GetComponent<Transform>();
            Image ohterface = ohterGamer[j].transform.Find($"Face").GetComponent<Image>();
            int ohternum = (int)item.Value.view.Owner.CustomProperties["isChar"];
            ohterface.sprite = Resources.Load<Sprite>($"UI/CharImage/re - char{ohternum}");
            TMP_Text ohterScore = ohterGamer[j].transform.Find($"Text (TMP) - Score").GetComponent<TMP_Text>();
            Image[] ohterarrows = new Image[4];
            for (int i = 0; i < ohterarrows.Length; i++)
            {
                ohterarrows[i] = ohterGamer[j].transform.Find($"Arrow{i}").GetComponent<Image>();
            }
            item.Value.statistic.onMiniGameScoreChanged += value =>
            {
                ohterarrows[0].sprite = _arrowsSprite[_arrows[value]];
                ohterarrows[1].sprite = _arrowsSprite[_arrows[value + 1]];
                ohterarrows[2].sprite = _arrowsSprite[_arrows[value + 2]];
                ohterarrows[3].sprite = _arrowsSprite[_arrows[value + 3]];
                ohterScore.text = $"{value}";
            };
            Transform bulkhead = ohterGamer[j].transform.Find($"Bulkhead").GetComponent<Transform>();
            bulkhead.gameObject.SetActive(false);
            j++;
        }
    }

    [PunRPC]
    void SetArrow(int[] arr)
    {
        _arrows = arr;
    }
}
