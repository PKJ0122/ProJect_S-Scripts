using Photon.Pun;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(PhotonView))]
public class ItemEvent_TwoDice : MonoBehaviour, IItemEvent
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
        Gambler.Get(PhotonNetwork.LocalPlayer.ActorNumber).diceEa += 1;
        yield return deley;
        PhotonNetwork.Destroy(gameObject);
    }
}

