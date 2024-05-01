using Photon.Pun;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UITurnDecide : UIScreensBase
{
    Image[] images = new Image[PhotonNetwork.PlayerList.Length];


    protected override void Awake()
    {
        base.Awake();
        for (int i = 0; i < images.Length; i++)
        {
            images[i] = transform.Find($"Panel - Order{i}/Image - Order{i}").GetComponent<Image>();
        }
        Show();
    }

    public override void Show()
    {
        base.Show();
    }

    YieldInstruction delay = new WaitForSeconds(1f);

    public IEnumerator TrunUISet()
    {
        for (int i = 0; i < images.Length; i++)
        {
            images[i].gameObject.SetActive(true);
        }
        yield return delay;



        for (int i = 0; i < images.Length; i++)
        {
            int num = (int)Gambler.Get(GameManager.instance.turnOrder[i]).view.Owner.CustomProperties["isChar"];
            Sprite sprite = Resources.Load<Sprite>($"UI/CharImage/re - Char{num}");
            images[i].sprite = sprite;
            yield return delay;
        }
        Hide();
    }
}
