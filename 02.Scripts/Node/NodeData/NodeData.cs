using UnityEngine;

[CreateAssetMenu(fileName = "NodeData", menuName ="Node/NodeData")]
public class NodeData : ScriptableObject
{
    public NodeKind kind = NodeKind.CoinPlusNode;
    public GameObject renderObject;
    public string nodeName;
}