using Photon.Pun;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(PhotonView), typeof(PhotonTransformView))]
public class ItemEvent_Rocket : MonoBehaviour, IItemEvent
{
    const int DMAGE = 10;
    public bool isActive
    {
        set => _isActive = value;
    }

    bool _isActive;
    float _radius = 3f;
    LayerMask _playerMask;
    PhotonView _photonView;
    int _arrival;
   
    private void Awake()
    {
        _photonView = GetComponent<PhotonView>();
        transform.localScale = new Vector3(_radius, _radius, _radius);
        _playerMask = 1 << LayerMask.NameToLayer("Gambler");
        _arrival = 10;
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
        Gambler ga = Gambler.Get(PhotonNetwork.LocalPlayer.ActorNumber);
        for (int i = 0; i < _arrival; i++)
        {
            yield return StartCoroutine(MoveEvent());

            Collider[] hitColliders = Physics.OverlapSphere(transform.position, _radius, _playerMask);
            foreach (var hitCollider in hitColliders)
            {
                if (hitCollider.gameObject == ga.gameObject)
                {
                    continue;
                }
                Gambler gambler = hitCollider.GetComponent<Gambler>();
                Gambler.Get(PhotonNetwork.LocalPlayer.ActorNumber).statistic.dmage += gambler.statistic.isPuppet ? 0 : DMAGE;
                Gambler.Get(PhotonNetwork.LocalPlayer.ActorNumber).statistic.kill += (gambler.statistic.hp - DMAGE > 0) ? 0 : 1;
                _photonView.RPC("DamgeEvent", RpcTarget.All, gambler.view.OwnerActorNr);
            }
        }
        
        UIManager.instance.Get<UITurnAction>().Show();
        PhotonNetwork.Destroy(gameObject);
    }

    WaitForEndOfFrame _waifFrame = new WaitForEndOfFrame();

    public IEnumerator MoveEvent()
    {
        Gambler ga = Gambler.Get(PhotonNetwork.LocalPlayer.ActorNumber);
        NodeInfo node = ga.currentNode.GetComponent<NodeInfo>();
        ga.currentNode = node.nextNode;
        transform.position = ga.transform.position;
        ga.transform.LookAt(ga.currentNode.transform.position + new Vector3(0, 0.5f, 0));
        transform.LookAt(ga.currentNode.transform.position + new Vector3(0, 0.5f, 0));
        float t = 0;
        while (t <= 0.3f)
        {
            t += Time.deltaTime;

            // 선형 보간을 사용하여 현재 위치를 계산합니다.
            ga.transform.position = Vector3.Lerp(ga.transform.position, ga.currentNode.transform.position, t/0.3f);
            transform.position = ga.transform.position;
            
            // 도착하면 이동을 멈춥니다.
            yield return null;
        }
        NodeBase nodeBase = ga.currentNode.GetComponent<NodeBase>();
        if (nodeBase as CrossroadsNode)
        {
            IPassby crossroadsNode = (IPassby)nodeBase;
            yield return StartCoroutine(crossroadsNode.PassBy(PhotonNetwork.LocalPlayer.ActorNumber));
        }
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

