using Photon.Pun;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(PhotonView), typeof(PhotonTransformView), typeof(LineRenderer))]
public class ItemEvent_AirGun : MonoBehaviour, IItemEvent
{
    const int DMAGE = 20;
    public bool isActive
    {
        set => _isActive = value;
    }

    bool _isActive;
    WaitForSeconds _deley = new WaitForSeconds(1);
    Vector3 targetPosition;
    LayerMask _playerMask;
    PhotonView _photonView;
    LineRenderer _lineRenderer;
    AirGunMove _airgunMove;
    float _maxSkillRange = 10f;
    float _fixedYValue;
    float _radius = 1f;
    

    private void Awake()
    {
        _playerMask = 1 << LayerMask.NameToLayer("Gambler");
        _photonView = GetComponent<PhotonView>();
        _lineRenderer = GetComponent<LineRenderer>();
        _airgunMove = transform.Find("BoxingGlove").GetComponent<AirGunMove>();
        _lineRenderer.positionCount = 2;
        _lineRenderer.SetPosition(0, transform.position);
        _lineRenderer.SetPosition(1, transform.position);
        _fixedYValue = Gambler.Get(PhotonNetwork.LocalPlayer.ActorNumber).transform.position.y + 3;
    }

    private void Update()
    {
        if (!_photonView.IsMine)
        {
            return;
        }
        if (_isActive)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            
            if (Physics.Raycast(ray, out hit))
            {
                targetPosition = new Vector3(hit.point.x, _fixedYValue, hit.point.z);

                // 스킬의 고정된 길이로 설정
                Vector3 direction = (targetPosition - transform.position).normalized;
                targetPosition = transform.position + direction * _maxSkillRange;
                transform.LookAt(targetPosition);
                Gambler.Get(PhotonNetwork.LocalPlayer.ActorNumber).transform.LookAt(targetPosition - new Vector3(0, 3, 0));

                // 라인 렌더러의 끝점 설정
                _lineRenderer.SetPosition(1, targetPosition);//new Vector3(targetPosition.x, targetPosition, targetPosition.z));
            }
            if (Input.GetMouseButtonDown(0))
            {              
                UIManager.instance.Get<UIKeyInfo>().Hide();
                _isActive = false;
                ItemsManager.instance.isUse = true;
                UIManager.instance.Get<UITurnAction>().Hide();
                _airgunMove.OnMoveStart(transform.position, targetPosition);
                StartCoroutine(ItemEvent());
            }
        }
    }
    
    public IEnumerator ItemEvent()
    {
        Collider[] hitColliders = Physics.OverlapCapsule(transform.position, targetPosition, _radius, _playerMask);
        foreach (Collider hitCollider in hitColliders)
        {
            // 검출된 오브젝트 처리
            Debug.Log(hitCollider.gameObject.name + " 공기포!");
            Gambler gambler = hitCollider.GetComponent<Gambler>();
            if (gambler.view.OwnerActorNr != PhotonNetwork.LocalPlayer.ActorNumber)
            {
                Gambler.Get(PhotonNetwork.LocalPlayer.ActorNumber).statistic.dmage += gambler.statistic.isPuppet ? 0 : DMAGE;
                Gambler.Get(PhotonNetwork.LocalPlayer.ActorNumber).statistic.kill += (gambler.statistic.hp - DMAGE > 0) ? 0 : 1;
                _photonView.RPC("DamgeEvent", RpcTarget.All, gambler.view.OwnerActorNr);
            }           
            yield return _deley;
        }           
        UIManager.instance.Get<UITurnAction>().Show();
        PhotonNetwork.Destroy(gameObject);
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
}

