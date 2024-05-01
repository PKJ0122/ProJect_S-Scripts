using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiceEffect : MonoBehaviour
{
    private void OnDisable()
    {
        GameObject effect = ObjectPoolingManager.instance.GetGo("DiceEffect");
        effect.transform.position = transform.position;
    }
}
