using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UICoinRoulette : UIScreensBase
{
    Transform _location;
    public List<GameObject> coinEventGameObjects { get; } = new List<GameObject>();


    protected override void Awake()
    {
        base.Awake();
        _location = transform.Find("Panel").GetComponent<Transform>();
    }

    void Start()
    {
        StartCoroutine(CoinRouletteSet());
    }

    IEnumerator CoinRouletteSet()
    {
        yield return new WaitUntil(() => CoinRouletteNode.coinEvents.Count != 0);

        foreach (KeyValuePair<int, ICoinRouletteEvent> item in CoinRouletteNode.coinEvents)
        {
            GameObject prefab = Resources.Load<GameObject>("Panel - CoinEvent");
            GameObject go = Instantiate(prefab, _location);
            TMP_Text detail = go.transform.Find("Text (TMP) - Name").GetComponent<TMP_Text>();
            detail.text = item.Value.eventName;
            coinEventGameObjects.Add(go);
        }
    }
}
