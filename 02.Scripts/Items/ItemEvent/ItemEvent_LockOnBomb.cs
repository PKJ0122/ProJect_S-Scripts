using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;

[RequireComponent(typeof(PhotonView), typeof(PhotonTransformView))]
public class ItemEvent_LockOnBomb : MonoBehaviour, IItemEvent
{
    const int DMAGE = 12;
    public bool isActive
    {
        set => _isActive = value;
    }
    bool _isActive;

    int _maxindex;
    int index = 0;
    PhotonView _photonView;
    List<Vector3> _playerTransform = new List<Vector3>();
    WaitForSeconds deley = new WaitForSeconds(1);


    private void Awake()
    {
        _photonView = GetComponent<PhotonView>();
        _playerTransform = new List<Vector3>();
        CameraManager.instance.ChangePointOfView(transform);
    }

    private void Start()
    {
        for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
        {
            _playerTransform.Add(Gambler.Get(PhotonNetwork.PlayerList[i].ActorNumber).gameObject.transform.position);
        }
        _maxindex = _playerTransform.Count - 1;
    }

    private void Update()
    {
        if (!_photonView.IsMine)
        {
            return;
        }
        if (_isActive)
        {
            if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
            {
                if (index == _maxindex)
                {
                    index = -1;
                }
                index++;
                gameObject.transform.position = _playerTransform[index];
            }
            if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
            {
                if (index == 0)
                {
                    index = _maxindex + 1;
                }
                index--;
                gameObject.transform.position = _playerTransform[index];
            }
            if (Input.GetKeyDown(KeyCode.E))
            {
                UIManager.instance.Get<UIKeyInfo>().Hide();
                _isActive = false;
                ItemsManager.instance.isUse = true;
                StartCoroutine(ItemEvent());
            }
        }
    }

    public IEnumerator ItemEvent()
    {
        Debug.Log("이벤트 실행");
        Gambler gambler = Gambler.Get(PhotonNetwork.PlayerList[index].ActorNumber);
        Gambler.Get(PhotonNetwork.LocalPlayer.ActorNumber).statistic.dmage += gambler.statistic.isPuppet ? 0 : DMAGE;
        Gambler.Get(PhotonNetwork.LocalPlayer.ActorNumber).statistic.kill += (gambler.statistic.hp - DMAGE > 0) ? 0 : 1;
        _photonView.RPC("DamgeEvent", RpcTarget.All, PhotonNetwork.PlayerList[index].ActorNumber);
        yield return deley;
        _photonView.RPC("ReturnCamare", RpcTarget.All);
        PhotonNetwork.Destroy(gameObject);
    }

    [PunRPC]
    public void DamgeEvent(int clientId)
    {
        Gambler gambler = Gambler.Get(clientId);
        GameObject effect = ObjectPoolingManager.instance.GetGo("LockBombEft");

        effect.transform.position = transform.position + new Vector3(0, 1f, 0);
        if (gambler.statistic.isPuppet)
        {
            gambler.statistic.isPuppet = false;
        }
        else
        {
            gambler.statistic.hp -= DMAGE;
        }
    }

    [PunRPC]
    public void ReturnCamare()
    {
        CameraManager.instance.ChangePointOfView();
    }
}