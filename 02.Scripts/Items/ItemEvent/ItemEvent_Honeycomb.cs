using Photon.Pun;
using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(PhotonView), typeof(PhotonTransformView))]
public class ItemEvent_Honeycomb : MonoBehaviour, IItemEvent
{
    const int DMAGE = 4;

    public bool isActive
    {
        set => _isActive = value;
    }

    bool _isActive;
    float x;
    float z;
    float _radius = 3f;
    float _maxDistanceFromCenter = 25;
    WaitForSeconds _deley = new WaitForSeconds(1);
    Transform _playerTransform;
    LayerMask _playerMask;
    LineDrawer _lineDrawer;
    PhotonView _photonView;


    private void Awake()
    {
        _playerTransform = Gambler.Get(PhotonNetwork.LocalPlayer.ActorNumber).gameObject.GetComponent<Transform>();
        _playerMask = 1 << LayerMask.NameToLayer("Gambler");
        _lineDrawer = GetComponentInChildren<LineDrawer>();
        _photonView = GetComponent<PhotonView>();
        CameraManager.instance.ChangePointOfView(transform);
    }
    private void Start()
    {
        _lineDrawer.DrawCircle(_maxDistanceFromCenter);
    }

    private void Update()
    {
        if (!_photonView.IsMine)
        {
            return;
        }
        if (_isActive)
        {
            x = Input.GetAxis("Horizontal");
            z = Input.GetAxis("Vertical");
            Vector3 movement = new Vector3(x, 0, z);
            float currentDistanceFromCenter = Vector3.Distance(_playerTransform.position, transform.position);
            //float currentDistanceFromCenter = Vector3.Distance(Vector3.zero, transform.position);
            if (currentDistanceFromCenter + movement.magnitude * 7 * Time.deltaTime <= _maxDistanceFromCenter)
            {
                transform.Translate(7 * Time.deltaTime * movement);
            }
            else
            {
                // 최대 거리를 넘어선 경우, 현재 위치를 최대 거리까지 이동
                transform.position = Vector3.MoveTowards(transform.position, _playerTransform.position, _maxDistanceFromCenter - currentDistanceFromCenter);
            }
            if (Input.GetKeyDown(KeyCode.E))
            {
                UIManager.instance.Get<UIKeyInfo>().Hide();
                _isActive = false;
                ItemsManager.instance.isUse = true;
                             
                StartCoroutine(MoveStart());
            }
        }
    }

    public IEnumerator MoveStart()
    {
        while (transform.position.y > _playerTransform.position.y + 1)
        {
            transform.Translate(Time.deltaTime * 10 * new Vector3(0, -1, 0));
            yield return new WaitForEndOfFrame();
        }
        StartCoroutine(ItemEvent());
    }

    public IEnumerator ItemEvent()
    {
        Debug.Log("이벤트 실행");
        Vector3 _pos = new Vector3(transform.position.x, _playerTransform.position.y + 1, transform.position.z);
        _photonView.RPC("EffCreate", RpcTarget.All, new object[] { _pos.x, _pos.y, _pos.z });
        yield return _deley;

        Collider[] hitColliders = Physics.OverlapSphere(_pos, _radius, _playerMask);
        foreach (var hitCollider in hitColliders)
        {
            Gambler gambler = hitCollider.GetComponent<Gambler>();
            Gambler.Get(PhotonNetwork.LocalPlayer.ActorNumber).statistic.dmage += gambler.statistic.isPuppet ? 0 : DMAGE * 3;
            Gambler.Get(PhotonNetwork.LocalPlayer.ActorNumber).statistic.kill += (gambler.statistic.hp - DMAGE * 3 > 0) ? 0 : 1;
            _photonView.RPC("DamgeEvent", RpcTarget.All, gambler.view.OwnerActorNr);
            yield return _deley;
            _photonView.RPC("DamgeEvent", RpcTarget.All, gambler.view.OwnerActorNr);
            yield return _deley;
            _photonView.RPC("DamgeEvent", RpcTarget.All, gambler.view.OwnerActorNr); 
            yield return _deley;
        }
        yield return _deley;
        _photonView.RPC("ReturnCamera", RpcTarget.All);
        PhotonNetwork.Destroy(gameObject);
    }

    [PunRPC]
    public void EffCreate(float x, float y, float z)
    {
        GameObject eff = ObjectPoolingManager.instance.GetGo("PoisonEF");
        eff.transform.position = new Vector3(x, y, z);
        eff.transform.localScale = new Vector3(3, 3, 3);
    }

    [PunRPC]
    public void DamgeEvent(int clientId)
    {
        Gambler gambler = Gambler.Get(clientId);
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
    public void ReturnCamera()
    {
        CameraManager.instance.ChangePointOfView();
    }
}