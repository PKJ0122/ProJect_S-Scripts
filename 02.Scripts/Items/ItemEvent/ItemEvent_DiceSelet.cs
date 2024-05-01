using Photon.Pun;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(PhotonView))]
public class ItemEvent_DiceSelet : MonoBehaviour, IItemEvent
{
    public bool isActive
    {
        set => _isActive = value;
    }
    bool _isActive;

    public int isNum
    {
        set => _isNum = value;
    }

    int _isNum;
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
            StartCoroutine(ItemEvent(_isNum));
            ItemsManager.instance.isUse = true;
            _isActive = false;
        }
    }

    public IEnumerator ItemEvent()
    {
        Debug.Log("이벤트 실행");
        yield return deley;
        Gambler.Get(PhotonNetwork.LocalPlayer.ActorNumber).diceNum = 1;
    }

    public IEnumerator ItemEvent(int num)
    {
        Debug.Log("이벤트 실행");
        Gambler gambler = Gambler.Get(PhotonNetwork.LocalPlayer.ActorNumber);
        gambler.choseNum = num;
        yield return deley;
        PhotonNetwork.Destroy(gameObject);
    }
}

