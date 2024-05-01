using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PhotonView))]
public class NodeInfo : MonoBehaviour
{
    public NodeKind _kind
    {
        get => _kind;
        set
        {
            _kind = value;
        }
    }

    public NodeData nodeData;
    [field : SerializeField] public bool fixedNode { get; private set; }
    [field: SerializeField] public int nodeId { get; set; }

    public NodeInfo nextNode { get; set; }
    [field: SerializeField] public List<NodeInfo> nextNodes { get; set; }

    private void Awake()
    {
        nextNode = nextNodes[0];

        if (nodeId == -1)
            return;

        if (!NodeCreater._nodes.TryAdd(nodeId, this))
        {
            Debug.LogError($"이미 등록된 노드id 입니다 {name},{nodeId}");
        }
    }
}