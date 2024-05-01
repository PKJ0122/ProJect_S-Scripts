using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public enum FontMode
{
    minSize,
    maxSize
}

public class UIDiceKey : UIScreensBase
{
    const float MIN_SIZE = 80;
    const float MAX_SIZE = 100;

    TMP_Text _diceKey;
    bool _isShow;
    FontMode _fontMode = FontMode.minSize;
    float _timeCheck;


    protected override void Awake()
    {
        base.Awake();
        _diceKey = transform.Find("Text (TMP) - DiceKey").GetComponent<TMP_Text>();
    }

    private void Update()
    {
        if (!_isShow)
            return;

        _timeCheck += Time.deltaTime;

        if (_fontMode == FontMode.minSize)
        {
            _diceKey.fontSize = Mathf.Lerp(MIN_SIZE, MAX_SIZE, _timeCheck / 1);
            if (_diceKey.fontSize >= MAX_SIZE)
            {
                _fontMode = FontMode.maxSize;
                _timeCheck = 0;
            }
        }
        else
        {
            _diceKey.fontSize = Mathf.Lerp(MAX_SIZE, MIN_SIZE, _timeCheck / 1);
            if (_diceKey.fontSize <= MIN_SIZE)
            {
                _fontMode = FontMode.minSize;
                _timeCheck = 0;
            }
        }
    }

    public override void Show()
    {
        base.Show();
        _isShow = true;
        _timeCheck = 0;
    }

    public override void Hide()
    {
        base.Hide();
        _isShow = false;
    }
}
