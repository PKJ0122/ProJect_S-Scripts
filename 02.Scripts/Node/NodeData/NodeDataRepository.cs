using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeDataRepository : SingletonMonoBase<NodeDataRepository>
{
    Dictionary<NodeKind,NodeData> _nodeDatas = new Dictionary<NodeKind,NodeData>();

    protected override void Awake()
    {
        base.Awake();
        NodeDatas nodeDatas = Resources.Load<NodeDatas>("NodeDatas");
        foreach (NodeData nodeData in nodeDatas.nodeDatas)
        {
            _nodeDatas.Add(nodeData.kind, nodeData);
        }
    }

    public NodeData Get(NodeKind nodeKind)
    {
        return _nodeDatas[nodeKind];
    }
}
