using Photon.Pun;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(PhotonView))]
public class ItemEvent_StarMove : MonoBehaviour, IItemEvent
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
            StartCoroutine(ItemEvent());
        }
    }

    public IEnumerator ItemEvent()
    {
        Debug.Log("이벤트 실행");

        yield return deley;
        Gambler.Get(PhotonNetwork.LocalPlayer.ActorNumber).statistic.coin += 20;
        yield return deley;

        PhotonNetwork.Destroy(gameObject);
    }
}

