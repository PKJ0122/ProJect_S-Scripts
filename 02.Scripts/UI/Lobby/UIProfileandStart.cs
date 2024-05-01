using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIProfileandStart : MonoBehaviour
{
    TMP_InputField _nickname;
    Button _startButton;


    private void Awake()
    {
        _nickname = transform.Find("Panel/Panel - Profile/InputField (TMP) - Nickname").GetComponent<TMP_InputField>();
        _startButton = transform.Find("Panel/Panel - Profile/Button - Start").GetComponent<Button>();

        _startButton.interactable = false;
        _startButton.onClick.AddListener(() =>
        {
            NicknameInfo.myNickname = _nickname.text;
            SceneManager.LoadScene("Lobby");
        });

        _nickname.onValueChanged.AddListener(value =>
        {
            _startButton.interactable = IsValidNickname(value);
        });
    }

    private void Start()
    {
        SoundManager.instance.BGMPlay("Lobby");
    }

    private bool IsValidNickname(string nickname)
    {
        return (nickname.Length > 1) && (nickname.Length < 11);
    }
}
