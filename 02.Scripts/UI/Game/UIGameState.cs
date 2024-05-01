using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class UIGameState : UIScreensBase
{
    Transform _location;
    TMP_Text turn;
    List<Gambler> _list = new List<Gambler>();

    protected override void Awake()
    {
        base.Awake();
        _location = transform.Find("Panel - User/UserInfoLayout").GetComponent<Transform>();
        turn = transform.Find("Panel - Rule/Text (TMP) - Turn").GetComponent<TMP_Text>();
    }

    private void Start()
    {
        StartCoroutine(SetUI());
    }

    IEnumerator SetUI()
    {
        yield return new WaitUntil(() => GameManager.instance.turnOrder != null);

        GameManager.instance.onTurnChage += (value1, value2) =>
        {
            turn.text = $"{value1} / {value2}";
        };
        foreach (KeyValuePair<int, Gambler> item in Gambler.spawned)
        {
            GameObject prefab = Resources.Load<GameObject>("Panel - UserInfo");
            GameObject go = Instantiate(prefab, transform);
            go.transform.SetParent(_location);
            Image image = go.transform.Find("UserImage - Box/Image - UserImage").GetComponent<Image>();
            TMP_Text nickName = go.transform.Find("Text (TMP) - NickName").GetComponent<TMP_Text>();
            TMP_Text hp = go.transform.Find("Slider - HP/Text (TMP) - HPText").GetComponent<TMP_Text>();
            Slider hpBar = go.transform.Find("Slider - HP").GetComponent<Slider>();
            TMP_Text star = go.transform.Find("Text (TMP) - StarText").GetComponent<TMP_Text>();
            TMP_Text coin = go.transform.Find("Text (TMP) - CoinText").GetComponent<TMP_Text>();
            TMP_Text ranke = go.transform.Find("Text (TMP) - Rank").GetComponent<TMP_Text>();
            Gambler gambler = item.Value;
            Debug.Log((int)gambler.view.Owner.CustomProperties["isChar"]);
            image.sprite = Resources.Load<Sprite>($"UI/CharImage/re - char{(int)gambler.view.Owner.CustomProperties["isChar"]}");
            nickName.text = gambler.view.Owner.NickName;
            gambler.statistic.onHpChanged += (value =>
            {
                hp.text = value.ToString();
                hpBar.value = value;
            });
            gambler.statistic.onStarChanged += (value =>
            {
                star.text = value.ToString();
                SetRanke();
            });
            gambler.statistic.onCoinChanged += (value =>
            {
                coin.text = value.ToString();
                SetRanke();
            });
            gambler.statistic.onRankeChanged += (value =>
            {
                ranke.text = $"{value}µî";
            });
        }
        for (int i = 0; i < GameManager.instance.turnOrder.Length; i++)
        {
            Image obj = transform.Find($"Panel - Rule/Turn/Image - Order{i}").GetComponent<Image>();
            obj.gameObject.SetActive(true);
            int num = (int)(Gambler.Get(GameManager.instance.turnOrder[i])
                                   .view.Owner.CustomProperties["isChar"]);
            obj.sprite = Resources.Load<Sprite>($"UI/CharImage/re - char{num}");
        }
    }

    private void SetRanke()
    {
        _list.Clear();
        foreach (KeyValuePair<int, Gambler> item in Gambler.spawned)
        {
            _list.Add(item.Value);
        }
        _list = _list.OrderByDescending(p => p.statistic.star)
                   .ThenByDescending(p => p.statistic.coin)
                   .ThenByDescending(p => GameManager.instance.turnOrder.Contains(p.view.OwnerActorNr))
                   .ToList();
        for (int i = 0; i < _list.Count; i++)
        {
            _list[i].statistic.ranke = i;
        }
    }
}
