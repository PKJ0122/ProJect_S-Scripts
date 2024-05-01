using System.Collections;
using UnityEngine;

public abstract class NodeBase : MonoBehaviour
{
    public virtual IEnumerator Visit(int gamblerId)
    {
        yield return null;
    }
}