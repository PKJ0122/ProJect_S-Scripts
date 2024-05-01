using UnityEngine.UI;

public class UIKeyInfo : UIScreensBase
{
    public bool eKey
    {
        set => _eKey.gameObject.SetActive(value);
    }
    public bool escKey
    {
        set => _escKey.gameObject.SetActive(value);
    }
    public bool wasdKey
    {
        set => _wasdKey.gameObject.SetActive(value);
    }
    public bool mouseKey
    {
        set => _mouseKey.gameObject.SetActive(value);
    }

    Image _eKey;
    Image _escKey;
    Image _wasdKey;
    Image _mouseKey;


    protected override void Awake()
    {
        base.Awake();
        _eKey = transform.Find("Panel/Image - EKey").GetComponent<Image>();
        _escKey = transform.Find("Panel/Image - ESCKey").GetComponent<Image>();
        _wasdKey = transform.Find("Panel/Image - MoveKey").GetComponent<Image>();
        _mouseKey = transform.Find("Panel/Image - MouseKey").GetComponent<Image>();
    }

    public override void Show()
    {
        eKey = true;
        escKey = true;
        wasdKey = false;
        mouseKey = false;
        base.Show();
    }
}
