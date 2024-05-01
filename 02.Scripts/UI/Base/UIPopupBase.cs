public class UIPopupBase : UIBase
{
    protected override void Awake()
    {
        base.Awake();
        UIManager.instance.RegisterPopup(this);
    }

    public override void Show()
    {
        base.Show();
        UIManager.instance.PushPopup(this);
    }
    public override void Hide()
    {
        base.Hide();
        UIManager.instance.PopPopup(this);
    }
}

