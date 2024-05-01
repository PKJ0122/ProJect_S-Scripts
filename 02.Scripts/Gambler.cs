using ExitGames.Client.Photon.StructWrapping;
using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Profiling;
using static UnityEngine.UI.GridLayoutGroup;
using Random = UnityEngine.Random;

[RequireComponent(typeof(PhotonView), typeof(PhotonTransformView), typeof(NavMeshAgent))]
public class Gambler : MonoBehaviour
{
    public static Dictionary<int, Gambler> spawned = new Dictionary<int, Gambler>();
    public static Gambler Get(int clientID) => spawned[clientID];
    public GamblerStatistic statistic { get; private set; }
    public PhotonView view { get => _view; }
    public NodeInfo currentNode { get => _currentNode; set => _currentNode = value; }
    public NavMeshAgent navMeshAgent { get => _agent; }

    public Dictionary<int, GamblerData> datas = new Dictionary<int, GamblerData>();

    public int diceEa
    {
        get => _diceEa;
        set => _diceEa = value;
    }

    public int diceNum
    {
        get => _diceNum;
        set
        {
            _diceNum = value;
            onDiceNumChange?.Invoke(value);
        }
    }

    public int choseNum { get; set; } = -1;

    public Coroutine moveLogic { get; set; }

    NavMeshAgent _agent;
    PhotonView _view;
    NodeInfo _currentNode;
    Animator _animator;
    bool _diceButtonClick;
    bool _diceKeyDown;
    int _diceEa = 0;
    int _diceNum = 0;
    GameObject _dice;
    GameObject _puppet;
    int _beforeNodeId;

    public event Action<int> onDiceNumChange;


    private void Awake()
    {
        statistic = GetComponent<GamblerStatistic>();
        _view = GetComponent<PhotonView>();
        _agent = GetComponent<NavMeshAgent>();
        _agent.enabled = false;
        _animator = GetComponent<Animator>();
        _currentNode = GameManager.instance.strartNode;
        spawned.Add(_view.OwnerActorNr, this);
        _dice = transform.Find("Dice").gameObject;
        _puppet = transform.Find("Puppet").gameObject;

        if (_view.IsMine)
        {
            UIManager.instance.Get<UITurnAction>().onButtonClick += () =>
            {
                _diceButtonClick = true;
            };
            #region 풀링 등록
            ObjectPoolingManager.instance.AddObjPool("Blood", 4);
            ObjectPoolingManager.instance.AddObjPool("Heal", 2);
            ObjectPoolingManager.instance.AddObjPool("Death", 4);
            ObjectPoolingManager.instance.AddObjPool("Coin", 3);
            ObjectPoolingManager.instance.AddObjPool("DiceEffect", 2);
            ObjectPoolingManager.instance.AddObjPool("BoomEF", 2); 
            ObjectPoolingManager.instance.AddObjPool("PoisonEF", 2);
            ObjectPoolingManager.instance.AddObjPool("LockBombEft", 2);
            ObjectPoolingManager.instance.AddObjPool("SnipingEft", 2);
            ObjectPoolingManager.instance.AddObjPool("Beggar", 1);
            ObjectPoolingManager.instance.AddObjPool("MagicEft", 2);
            ObjectPoolingManager.instance.AddObjPool("Drool", 1);
            ObjectPoolingManager.instance.AddObjPool("Devil", 1);
            ObjectPoolingManager.instance.AddObjPool("Star", 2);
            ObjectPoolingManager.instance.AddObjPool("ArmoryEF", 2);
            ObjectPoolingManager.instance.AddObjPool("CoinEF", 2);
            for (int i = 0; i < System.Enum.GetNames(typeof(ItemCode)).Length; i++)
            {
                ObjectPoolingManager.instance.AddObjPool($"ItemModle_{(ItemCode)i}", 2);
            }                    
            #endregion
        }
    }

    private void Start()
    {
        if (PhotonNetwork.IsMasterClient && _view.IsMine)
        {
            StartCoroutine(TrunSet());
        }
        statistic.onPuttetChanged += value =>
        {
            OnPuttetable(value);
        };
    }

    private void Update()
    {
        if (_view.IsMine)
        {
            if (Input.GetKeyDown(KeyCode.Space))
                _diceKeyDown = true;
        }

        Vector3 relVelocity = new Vector3(0, 0, Vector3.Dot(_agent.transform.forward, _agent.velocity));
        float speed = Mathf.Lerp(0, 1, relVelocity.z / 5);

        _animator.SetFloat("Velocity", speed);
    }

    public void SetAnimationNo(GamblerAnimation animation)
    {
        _animator.SetInteger("State", (int)animation);
    }

    public void SetAnimation(GamblerAnimation animation)
    {
        Front();
        _animator.SetInteger("State", (int)animation);
    }

    IEnumerator TrunSet()
    {
        yield return new WaitUntil(() => spawned.Count == PhotonNetwork.PlayerList.Length);
        #region 순서정하는 로직

        Dictionary<int, int> playerNumbers = new Dictionary<int, int>();
        List<int> _trunOrder = new List<int>();

        for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
        {
            playerNumbers.Add(PhotonNetwork.PlayerList[i].ActorNumber, Random.Range(0, 101));
        }

        List<KeyValuePair<int, int>> sortedPlayers = playerNumbers.ToList();
        sortedPlayers.Sort((pair1, pair2) => pair2.Value.CompareTo(pair1.Value));

        _trunOrder.Clear();

        foreach (var pair in sortedPlayers)
        {
            _trunOrder.Add(pair.Key);
        }

        int[] _trunOrderArray = _trunOrder.ToArray();

        _view.RPC("SettrunOrder", RpcTarget.All, _trunOrderArray);
        #endregion
    }

    public void FirstTrunStrat()
    {
        _view.RPC("StartTurn", RpcTarget.AllBuffered, GameManager.instance.GetNextOrder(
                                              GameManager.instance.turnOrder[GameManager.instance.turnOrder.Length - 1]));
    }

    [PunRPC]
    public void SettrunOrder(int[] _trunOrder)
    {
        Debug.Log("턴셋 알피씨 울림");
        GameManager.instance.turnOrder = _trunOrder;
    }

    [PunRPC]
    public void StartTurn(int clientID)
    {
        if (clientID == GameManager.instance.turnOrder[0])
            GameManager.instance.turnCount++;

        if (UIManager.instance.Get<UIOtherTurnIndicate>().inputActionEnable)
        {
            UIManager.instance.Get<UIOtherTurnIndicate>().Hide();
        }

        if (PhotonNetwork.LocalPlayer.ActorNumber == clientID)
        {
            moveLogic = StartCoroutine(Get(clientID).MyTrun());
        }
        else
        {
            UIManager.instance.Get<UIOtherTurnIndicate>().Show(spawned[clientID].view.Owner.NickName);
        }

        CameraManager.instance.ChangePointOfView(clientID);
        UIManager.instance.Get<UITrunStartWords>().Show(spawned[clientID].view.Owner.NickName);
    }

    YieldInstruction _delay = new WaitForSeconds(2f);
    YieldInstruction deathDelay = new WaitForSeconds(1.5f);
    YieldInstruction diceDelay = new WaitForSeconds(0.7f);

    public IEnumerator MyTrun()
    {
        ItemsManager.instance.isUse = false;
        Front();
        UIManager.instance.Get<UITurnAction>().Show();
        _agent.enabled = true;
        _diceButtonClick = false;
        _diceEa++;
        yield return new WaitUntil(() => _diceButtonClick);

        for (int i = 0; i < _diceEa; i++)
        {
            _diceKeyDown = false;
            _view.RPC("DiceSetOnOff", RpcTarget.All, true);
            UIManager.instance.Get<UIDiceKey>().Show();
            yield return new WaitUntil(() => _diceKeyDown);
            _animator.SetInteger("State", 1);
            yield return diceDelay;
            UIManager.instance.Get<UIDiceKey>().Hide();
            diceNum += choseNum < 0 ? Random.Range(1, 13) : choseNum;
            _view.RPC("DiceSetOnOff", RpcTarget.All, false);
            _view.RPC("DiceNumSynchronization", RpcTarget.All,diceNum );
            yield return diceDelay;
        }

        _diceEa = 0;
        choseNum = -1;

        NodeBase nodeEvent = null;

        while (diceNum > 0)
        {
            Move();
            yield return new WaitUntil(() => Vector3.Distance(new Vector3(_currentNode.transform.position.x,
                                                                          _currentNode.transform.position.y,
                                                                          _currentNode.transform.position.z),
                                                                          transform.position) <= 1f);
            _view.RPC("DiceNumSynchronization", RpcTarget.All, --diceNum );
            nodeEvent = _currentNode.GetComponent<NodeBase>();

            if (nodeEvent is IPassby)
            {
                _view.RPC("NodeName", RpcTarget.All, _currentNode.nodeId);
                IPassby passbyNode = (IPassby)nodeEvent;
                yield return StartCoroutine(passbyNode.PassBy(_view.OwnerActorNr));
            }
        }

        _view.RPC("NodeName", RpcTarget.All, _currentNode.nodeId);
        yield return StartCoroutine(nodeEvent.Visit(_view.OwnerActorNr));
        yield return _delay;

        _agent.enabled = false;

        _view.RPC("DestroyNode", RpcTarget.All);
        _view.RPC("StartTurn", RpcTarget.All, GameManager.instance.GetNextOrder(PhotonNetwork.LocalPlayer.ActorNumber));
    }

    [PunRPC]
    private void DestroyNode()
    {
        NodeCreater.instance.NodeDistroy();
    }

    [PunRPC]
    private void DiceSetOnOff(bool on)
    {
        _dice.SetActive(on);

        if (!on)
            UIManager.instance.Get<UIDiceValue>().Show();
    }

    [PunRPC]
    private void NodeName(int nodeId)
    {
        if (_beforeNodeId == nodeId)
            return;

        _beforeNodeId = nodeId;
        UIManager.instance.Get<UINodeName>().Show(NodeCreater.Get(nodeId).nodeData.nodeName);
    }

    [PunRPC]
    private void DiceNumSynchronization(int value)
    {
        diceNum = value;
    }

    private void Move()
    {
        Transform nextNode = _currentNode.nextNode.transform;
        _currentNode = nextNode.GetComponent<NodeInfo>();
        _agent.SetDestination(new Vector3(nextNode.position.x, nextNode.position.y, nextNode.position.z));
    }

    public void Front()
    {
        if (_agent.enabled)
        {
            _agent.enabled = false;
            transform.rotation = new Quaternion(transform.rotation.x,
                                                GameManager.instance.frontalAngle,
                                                transform.rotation.z,
                                                transform.rotation.w);
            _agent.enabled = true;
            return;
        }
        transform.rotation = new Quaternion(transform.rotation.x,
                                                GameManager.instance.frontalAngle,
                                                transform.rotation.z,
                                                transform.rotation.w);
    }

    public IEnumerator GamblerDie()
    {
        _animator.SetInteger("State", 2);
        yield return deathDelay;
        _agent.enabled = false;
        _currentNode = GameManager.instance.spawnNode;
        transform.position = new Vector3(GameManager.instance.spawnNode.transform.position.x,
                                         GameManager.instance.spawnNode.transform.position.y,
                                         GameManager.instance.spawnNode.transform.position.z);
        Front();
        statistic.hp = 30;
        _agent.enabled = true;
    }

    public void OnPuttetable(bool check)
    {
        _puppet.SetActive(check);
    }

    public void ItemGetting(ItemCode itemCode)
    {
        ItemsManager.instance.Get(itemCode).Count++;      
    }

    public void ItemDelet(ItemCode itemCode)
    {
        ItemsManager.instance.Get(itemCode).Count--;
    }

    public void DataRPCStart()
    {
        _view.RPC("DataSave", RpcTarget.All);
        _view.RPC("AwardScene", RpcTarget.MasterClient);
    }

    [PunRPC]
    public void AwardScene()
    {
        PhotonNetwork.LoadLevel("Map-Award");
    }

    [PunRPC]
    public void DataSave()
    {
        for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
        {
            int id = PhotonNetwork.PlayerList[i].ActorNumber;
            Gambler gambler = Get(id);
            datas.Add(id, new GamblerData(gambler.statistic.star, gambler.statistic.coin, gambler.statistic.dmage, gambler.statistic.kill, id));
        }
        DataSaver.instance.gamblers = datas;
    }

    private void OnDestroy()
    {
        spawned.Clear();
    }
}
