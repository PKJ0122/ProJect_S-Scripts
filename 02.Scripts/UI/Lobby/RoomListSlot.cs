using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoomListSlot : MonoBehaviour
{
    public int roomIndex { get; set; }

    TMP_Text _title;
    TMP_Text _people;
    Button _select;

    public event Action<int> onSelected;

    private void Awake()
    {
        _title = transform.Find("Text (TMP) - Title").GetComponent<TMP_Text>();
        _people = transform.Find("Text (TMP) - People").GetComponent<TMP_Text>();
        _select = GetComponent<Button>();
        _select.onClick.AddListener(() => onSelected?.Invoke(roomIndex));
    }

    public void Refresh(string roomName, int currentPlayer, int maxPlayer)
    {
        _title.text = roomName;
        _people.text = $"{currentPlayer} / {maxPlayer}";
    }
}
