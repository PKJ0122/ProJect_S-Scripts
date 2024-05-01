using System.Collections;
using TMPro;
using UnityEngine;

public class UITrunStartWords : UIScreensBase
{
    TMP_Text _currentGambler;

    protected override void Awake()
    {
        base.Awake();
        _currentGambler = transform.Find("Text (TMP) - CurrentGambler").GetComponent<TMP_Text>();
    }

    public void Show(string nickName)
    {
        base.Show();
        StartCoroutine(CurrentGamblerAnimation(nickName));
    }

    IEnumerator CurrentGamblerAnimation(string nickName)
    {
        const float ANIMATION_END_TIME = 1f;
        float time = 0;

        _currentGambler.text = $"{nickName}´ÔÀÇ Â÷·ÊÀÔ´Ï´Ù.";

        while (time <= ANIMATION_END_TIME)
        {
            time += Time.deltaTime;
            _currentGambler.fontSize = Mathf.Lerp(75f, 100f, time / ANIMATION_END_TIME);
            yield return null;
        }

        Hide();
    }
}
