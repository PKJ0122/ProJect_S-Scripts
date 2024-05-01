using TMPro;

public class UIOtherTurnIndicate : UIScreensBase
{
    TMP_Text _otherTurnIndicate;

    protected override void Awake()
    {
        base.Awake();
        _otherTurnIndicate = transform.Find("Text (TMP) - OtherTurnIndicate").GetComponent<TMP_Text>();
    }

    public void Show(string nickName)
    {
        base.Show();
        _otherTurnIndicate.text = $"{nickName}님의 턴이 진행중입니다...";
    }
}
