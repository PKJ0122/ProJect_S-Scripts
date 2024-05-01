public class UIScreensBase : UIBase
{
    protected override void Awake()
    {
        base.Awake();
        UIManager.instance.RegisterScreen(this);
    }

    public override void Show()
    {
        base.Show();
        UIManager.instance.PushScreen(this);
    }

    public override void Hide()
    {
        base.Hide();
        UIManager.instance.PopScreen(this);
    }
}

