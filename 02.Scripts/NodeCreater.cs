using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(PhotonView))]
public class NodeCreater : SingletonMonoBase<NodeCreater>
{
    Dictionary<NodeKind, System.Type> _nodeBases = new Dictionary<NodeKind, System.Type>();
    public static Dictionary<int,NodeInfo> _nodes = new Dictionary<int, NodeInfo>();
    public static NodeInfo Get(int id) => _nodes[id];

    List<int> creating;

    public int nodeLength { get => _nodePath.Length; }
    public int nodeKindLength { get => _nodeBases.Count; }
    public int starNodeIndex { get => _starNodeIndex; set => _starNodeIndex = value; }
    public int armoryNodeIndex { get => _armoryNodeIndex; set => _armoryNodeIndex = value; }
    public GameObject starGameObject { get => _starGameObject; }

    PhotonView _view;
    [SerializeField] NodeInfo[] _nodePath;
    int _starNodeIndex = -1;
    int _armoryNodeIndex = -1;

    List<int> _destroyList = new List<int>();

    int[] _percent = new int[] { 40, 5, 7, 3, 5, 3, 7, 7, 5, 7, 3, 5, 5 };
    int _percentAmount = 0;

    GameObject _starGameObject;

    protected override void Awake()
    {
        base.Awake();
        SetDictionary();

        if (!PhotonNetwork.IsMasterClient)
            return;

        creating = new List<int>();

        for (int i = 0; i < _percent.Length; i++)
        {
            _percentAmount += _percent[i];
        }

        for (int i = 0; i < _nodePath.Length; i++)
        {
            creating.Add(NodeSet());
        }

        _view = GetComponent<PhotonView>();

        int[] creatingArray = creating.ToArray();
        _view.RPC("SetCreatingNodeList", RpcTarget.Others, creatingArray);
    }

    

    int NodeSet()
    {
        int randomNode = Random.Range(0, _percentAmount+1);
        int amount = 0;

        for (int i = 0; i < _percent.Length; i++)
        {
            if (amount <= randomNode && randomNode < amount+_percent[i])
            {
                return i;
            }
            amount += _percent[i];
        }

        return 0;
    }

    private void Start()
    {
        StartCoroutine(NodeCreate());
        if (PhotonNetwork.IsMasterClient)
        {
            int starIndex = Random.Range(0, _nodePath.Length);
            int armoryIndex = starIndex;

            while (armoryIndex == starIndex)
            {
                armoryIndex = Random.Range(0, _nodePath.Length);
            }

            int[] ints = new int[2];
            ints[0] = starIndex;
            ints[1] = armoryIndex;

            _view.RPC("CreatSpecialNode", RpcTarget.All, ints);
        }
    }

    public void OnDestroy()
    {
        _nodes.Clear();
    }

    YieldInstruction delay = new WaitForSeconds(2f);

    public IEnumerator StarNodeCreat(int gamblerId)
    {
        Destroy(_nodePath[_starNodeIndex].GetComponent<NodeBase>());
        _nodePath[_starNodeIndex].AddComponent<StarNode>();
        _nodePath[_starNodeIndex].nodeData = NodeDataRepository.instance.Get(NodeKind.StarNode);
        CameraManager.instance.ChangePointOfView(_nodePath[_starNodeIndex].transform);
        Transform[] gos = _nodePath[_starNodeIndex].transform.GetComponentsInChildren<Transform>();
        for (int i = 1; i < gos.Length; i++)
        {
            gos[i].gameObject.SetActive(false);
        }
        GameObject go = Instantiate(NodeDataRepository.instance.Get(NodeKind.StarNode).renderObject
                , _nodePath[_starNodeIndex].transform.position, _nodePath[_starNodeIndex].transform.rotation);
        go.transform.SetParent(_nodePath[_starNodeIndex].transform);
        float t = 0;
        _starGameObject = ObjectPoolingManager.instance.GetGo("Star");
        _starGameObject.transform.localScale = new Vector3(2, 2, 2);
        Vector3 starnode = _nodePath[_starNodeIndex].transform.position;
        while (t <= 3f)
        {
            t += Time.deltaTime;
            _starGameObject.transform.position = Vector3.Lerp(starnode + new Vector3(0, 15, 0),
                                                              starnode + new Vector3(0, 7, 0),
                                                              t / 3f);
            yield return null;
        }
        yield return delay;
        CameraManager.instance.ChangePointOfView(gamblerId);
    }

    public IEnumerator NextStarNodeCreat(int nodeIndex , NodeBase nodeBase, int nodeKind , int gamblerId)
    {
        _starGameObject.transform.localScale = new Vector3(2, 2, 2);
        _starGameObject.GetComponent<PoolAble>().ReleaseObject();
        _starGameObject = null;
        _destroyList.Add(_starNodeIndex);
        nodeBase.gameObject.AddComponent(_nodeBases[(NodeKind)nodeKind]);
        nodeBase.GetComponent<NodeInfo>().nodeData = NodeDataRepository.instance.Get((NodeKind)nodeKind);
        Transform[] gos = nodeBase.gameObject.transform.GetComponentsInChildren<Transform>();
        for (int i = 1; i < gos.Length; i++)
        {
            gos[i].gameObject.SetActive(false);
        }
        GameObject go = Instantiate(NodeDataRepository.instance.Get((NodeKind)nodeKind).renderObject
                , nodeBase.transform.position, nodeBase.transform.rotation);
        go.transform.SetParent(nodeBase.gameObject.transform);
        _starNodeIndex = nodeIndex;
        yield return StartCoroutine(StarNodeCreat(gamblerId));
    }

    public IEnumerator ArmoryNodeCreat(int gamblerId)
    {
        Destroy(_nodePath[_armoryNodeIndex].GetComponent<NodeBase>());
        _nodePath[_armoryNodeIndex].AddComponent<ArmoryNode>();
        _nodePath[_armoryNodeIndex].nodeData = NodeDataRepository.instance.Get(NodeKind.ArmoryNode);
        CameraManager.instance.ChangePointOfView(_nodePath[_armoryNodeIndex].transform);
        Transform[] gos = _nodePath[_armoryNodeIndex].transform.GetComponentsInChildren<Transform>();
        for (int i = 1; i < gos.Length; i++)
        {
            gos[i].gameObject.SetActive(false);
        }
        GameObject go = Instantiate(NodeDataRepository.instance.Get(NodeKind.ArmoryNode).renderObject
                , _nodePath[_armoryNodeIndex].transform.position, _nodePath[_armoryNodeIndex].transform.rotation);
        go.transform.SetParent(_nodePath[_armoryNodeIndex].transform);
        GameObject armoryEF = ObjectPoolingManager.instance.GetGo("ArmoryEF");
        armoryEF.transform.position = _nodePath[_armoryNodeIndex].transform.position;
        yield return delay;
        CameraManager.instance.ChangePointOfView(gamblerId);
    }

    public IEnumerator NextArmoryNodeCreat(int nodeIndex, NodeBase nodeBase, int nodeKind, int gamblerId)
    {
        _destroyList.Add(_armoryNodeIndex);
        nodeBase.gameObject.AddComponent(_nodeBases[(NodeKind)nodeKind]);
        nodeBase.GetComponent<NodeInfo>().nodeData = NodeDataRepository.instance.Get((NodeKind)nodeKind);
        Transform[] gos = nodeBase.gameObject.transform.GetComponentsInChildren<Transform>();
        for (int i = 1; i < gos.Length; i++)
        {
            gos[i].gameObject.SetActive(false);
        }
        GameObject go = Instantiate(NodeDataRepository.instance.Get((NodeKind)nodeKind).renderObject
                , nodeBase.transform.position, nodeBase.transform.rotation);
        go.transform.SetParent(nodeBase.gameObject.transform);
        _armoryNodeIndex = nodeIndex;
        yield return StartCoroutine(ArmoryNodeCreat(gamblerId));
    }

    IEnumerator NodeCreate()
    {
        yield return new WaitUntil(() => creating != null);

        for (int i = 0; i < creating.Count; i++)
        {
            if (_nodePath[i].fixedNode)
                continue;

            NodeKind nodeKind = (NodeKind)creating[i];
            _nodePath[i].nodeData = NodeDataRepository.instance.Get(nodeKind);
            _nodePath[i].gameObject.AddComponent(_nodeBases[nodeKind]);
            GameObject go = Instantiate(NodeDataRepository.instance.Get(nodeKind).renderObject
                , _nodePath[i].transform.position, _nodePath[i].transform.rotation);
            go.transform.SetParent(_nodePath[i].transform);
        }
    }

    [PunRPC]
    void SetCreatingNodeList(int[] creatingArray)
    {
        creating = creatingArray.ToList();
    }

    [PunRPC]
    void CreatSpecialNode(int[] indexs)
    {
        _starNodeIndex = indexs[0];
        _armoryNodeIndex = indexs[1];
    }

    void SetDictionary()
    {
        Assembly currentAssembly = Assembly.GetExecutingAssembly();

        IEnumerable<Type> nodeTypes = currentAssembly.GetTypes().Where(t 
                                                  => t.IsSubclassOf(typeof(NodeBase)) && !t.IsAbstract);

        foreach (Type type in nodeTypes)
        {
            NodeKind nodeKind;
            bool parseResult = Enum.TryParse(type.Name, out nodeKind);

            if (parseResult)
            {
                _nodeBases.Add(nodeKind, type);
            }
        }
    }

    public void NodeDistroy()
    {
        if (_destroyList.Count == 0)
            return;

        foreach (int i in _destroyList)
        {
            if (_nodePath[i].TryGetComponent(out StarNode starNode))
            {
                Destroy(starNode);
                continue;
            }
            if (_nodePath[i].TryGetComponent(out ArmoryNode armoryNode))
            {
                Destroy(armoryNode);
                continue;
            }
        }

        _destroyList.Clear();
    }

    
}