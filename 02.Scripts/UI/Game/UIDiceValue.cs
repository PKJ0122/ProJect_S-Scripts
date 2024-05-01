using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIDiceValue : UIScreensBase
{
    TMP_Text _diceValue;

    protected override void Awake()
    {
        base.Awake();
        _diceValue = transform.Find("Text (TMP) - DiceValue").GetComponent<TMP_Text>();

        StartCoroutine(SettingDiceValueText());
    }

    IEnumerator SettingDiceValueText()
    {
        yield return new WaitUntil(() => Gambler.spawned.Count == PhotonNetwork.PlayerList.Length);

        foreach (KeyValuePair<int, Gambler> item in Gambler.spawned)
        {
            item.Value.onDiceNumChange += value =>
            {
                if (value == 0)
                    Hide();

                _diceValue.text = value.ToString();
            };
        }
    }
}
