using System;
using System.Collections;
using TMPro;
using UnityEngine;

public abstract class MiniGameBase : MonoBehaviour
{
    public abstract MiniGameKind miniGameKind { get; }
    protected TMP_Text gameTimer;
    protected Transform startcountBox;
    protected TMP_Text startCounter;
    protected Transform endGame;
    protected int startCount = 3;
    protected float gameTime = 30f;
    protected bool isGameing;


    protected virtual int StartCount
    {
        get => startCount;
        set
        {
            startCount = value;
            onStartCountChanged?.Invoke(startCount);
        }
    }

    protected virtual float GameTime
    {
        get => gameTime;
        set
        {
            if (value <= 0)
            {
                gameTime = 0;
                onGameTimerChanged?.Invoke(0);
                return;
            }

            gameTime = value;
            onGameTimerChanged?.Invoke(gameTime);
        }
    }

    protected virtual event Action<int> onStartCountChanged;
    protected virtual event Action<float> onGameTimerChanged;


    protected virtual void Awake()
    {
        MiniGameManager.minigames.Add(miniGameKind, this);
        gameTimer = transform.Find("Text (TMP) - Timer").GetComponent<TMP_Text>();
        startcountBox = transform.Find("Box - StartCounter").GetComponent<Transform>();
        startCounter = transform.Find("Box - StartCounter/Text (TMP) - StartCounter").GetComponent<TMP_Text>();
        endGame = transform.Find("Box - MiniGameEnd").GetComponent<Transform>();
        onGameTimerChanged += value =>
        {
            gameTimer.text = $"{value:0.00}";
            endGame.gameObject.SetActive(value == 0 ? true : false);
        };
        onStartCountChanged += value =>
        {
            startcountBox.gameObject.SetActive(value == 0 ? false : true);
            startCounter.text = value.ToString();
        };
    }

    public virtual IEnumerator MiniGame()
    {
        yield return null;
    }
}
