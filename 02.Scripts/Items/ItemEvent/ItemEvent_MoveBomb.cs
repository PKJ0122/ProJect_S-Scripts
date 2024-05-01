using Photon.Pun;
using System;
using System.Collections;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(PhotonView), typeof(PhotonTransformView))]
public class ItemEvent_MoveBomb : MonoBehaviour, IItemEvent
{
    const int DMAGE = 15;
    public bool isActive
    {
        set => _isActive = value;
    }
    bool _isActive;
    float _timer;
    float x;
    float z;
    float _radius = 3f;
    Transform _transform;
    LayerMask _playerMask;
    PhotonView _photonView;
    WaitForSeconds deley = new WaitForSeconds(1f);
    TMP_Text _timerText;


    private void Awake()
    {
        _transform = GetComponent<Transform>();
        _playerMask = 1 << LayerMask.NameToLayer("Gambler");
        _photonView = GetComponent<PhotonView>();
        _timerText = transform.Find("Canvas/Text (TMP)").GetComponent<TMP_Text>();
        CameraManager.instance.ChangePointOfView(transform);     
    }

    private void OnEnable()
    {
        _timer = 10f;
        _timerText.text = "10";
    }


    private void Update()
    {
        if (_timer > 0)
        {
            _timer -= Time.deltaTime;
            _timerText.text = $"{_timer:0}";
        }
        else
        {
            _timerText.text = $"0";
            _timer = 0f;           
        }
        if (!_photonView.IsMine)
        {
            return;
        }
        if (_isActive)
        {
            x = Input.GetAxis("Horizontal");
            z = Input.GetAxis("Vertical");

            if (_timer > 0)
            {
                _transform.Translate(7 * Time.deltaTime * new Vector3(x, 0, z));
            }
            else
            {
                UIManager.instance.Get<UIKeyInfo>().Hide();
                _timer = 0;
                _isActive = false;
                ItemsManager.instance.isUse = true;
                UIManager.instance.Get<UITurnAction>().Hide();
                StartCoroutine(ItemEvent());
            }
        }
    }

    public IEnumerator ItemEvent()
    {
        Debug.Log("이벤트 실행");
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, _radius, _playerMask);
        _photonView.RPC("EftCreat", RpcTarget.All);
        foreach (var hitCollider in hitColliders)
        {
            Gambler gambler = hitCollider.GetComponent<Gambler>();
            Gambler.Get(PhotonNetwork.LocalPlayer.ActorNumber).statistic.dmage += gambler.statistic.isPuppet ? 0 : DMAGE;
            Gambler.Get(PhotonNetwork.LocalPlayer.ActorNumber).statistic.kill += (gambler.statistic.hp - DMAGE > 0) ? 0 : 1;
            _photonView.RPC("DamgeEvent", RpcTarget.All, gambler.view.OwnerActorNr);
            yield return deley;           
        }
        _photonView.RPC("ReturnCamare", RpcTarget.All);
        UIManager.instance.Get<UITurnAction>().Show();
        PhotonNetwork.Destroy(gameObject);
    }

    [PunRPC]
    public void EftCreat()
    {
        GameObject effect = ObjectPoolingManager.instance.GetGo("BoomEF");
        effect.transform.position = transform.position + new Vector3(0, 1f, 0);
        SoundManager.instance.SEFPlay("Bomb");
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
    public void ReturnCamare()
    {
        CameraManager.instance.ChangePointOfView();
    }
}

