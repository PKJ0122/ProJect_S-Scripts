using Photon.Pun;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(PhotonView))]
public class ItemEvent_HealPack : MonoBehaviour, IItemEvent
{
    public bool isActive
    {
        set => _isActive = value;
    }
    bool _isActive;

    PhotonView _photonView;
    WaitForSeconds deley = new WaitForSeconds(1);


    private void Awake()
    {
        _photonView = GetComponent<PhotonView>();
    }

    private void Update()
    {
        if (!_photonView.IsMine)
        {
            return;
        }
        if (_isActive)
        {
            UIManager.instance.Get<UIKeyInfo>().Hide();
            _isActive = false;
            ItemsManager.instance.isUse = true;
            UIManager.instance.Get<UITurnAction>().Hide();
            StartCoroutine(ItemEvent());
        }
    }

    public IEnumerator ItemEvent()
    {
        Debug.Log("이벤트 실행");
        yield return deley;
        Gambler.Get(PhotonNetwork.LocalPlayer.ActorNumber).statistic.hp += 5;
        yield return deley;
        Gambler.Get(PhotonNetwork.LocalPlayer.ActorNumber).statistic.hp += 5;
        yield return deley;
        Gambler.Get(PhotonNetwork.LocalPlayer.ActorNumber).statistic.hp += 5;
        UIManager.instance.Get<UITurnAction>().Show();
        PhotonNetwork.Destroy(gameObject);
    }
}

