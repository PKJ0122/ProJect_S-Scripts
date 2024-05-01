using Photon.Pun;
using System.Collections;
using Unity.Burst.CompilerServices;
using UnityEngine;
using UnityEngine.Profiling;

[RequireComponent(typeof(PhotonView), typeof(PhotonTransformView))]
public class ItemEvent_Kamui : MonoBehaviour, IItemEvent
{
    public bool isActive
    {
        set => _isActive = value;
    }

    bool _isActive;
    float x;
    float z;
    float _radius = 2f;
    WaitForSeconds _deley = new WaitForSeconds(1);
    Transform _transform;
    Vector3 _position;
    LayerMask _playerMask;
    PhotonView _photonView;


    private void Awake()
    {
        _transform = GetComponent<Transform>();
        _playerMask = 1 << LayerMask.NameToLayer("Gambler");
        _photonView = GetComponent<PhotonView>();
        _position = _transform.position;
        CameraManager.instance.ChangePointOfView(transform);
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
            _transform.Translate(7 * Time.deltaTime * new Vector3(x, 0, z));
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
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, _radius, _playerMask);
        foreach (Collider hitCollider in hitColliders)
        {
            // 검출된 오브젝트 처리
            Debug.Log(hitCollider.gameObject.name + " 카무이!");
            Gambler gambler = hitCollider.GetComponent<Gambler>();
            yield return _deley;
            _photonView.RPC("Kamui", RpcTarget.All, new object[] { gambler.view.OwnerActorNr ,
                Gambler.Get(PhotonNetwork.LocalPlayer.ActorNumber).currentNode.nodeId });
            yield return _deley;
        }
        _photonView.RPC("CameraReturn", RpcTarget.All);

        PhotonNetwork.Destroy(gameObject);
        yield break;
    }

    [PunRPC]
    public void Kamui(int clientId, int nodeId)
    {
        Gambler gambler = Gambler.Get(clientId);
        if (gambler.statistic.isPuppet)
        {
            gambler.statistic.isPuppet = false;
        }
        else
        {
            gambler.currentNode = NodeCreater.Get(nodeId);
            gambler.transform.position = NodeCreater.Get(nodeId).transform.position;
        }
        CameraManager.instance.ChangePointOfView();
    }

    [PunRPC]
    public void CameraReturn()
    {
        CameraManager.instance.ChangePointOfView();
    }
}

