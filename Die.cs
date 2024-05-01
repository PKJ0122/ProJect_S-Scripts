using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Die : StateMachineBehaviour
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        GameObject deatheffect = ObjectPoolingManager.instance.GetGo("Death");
        deatheffect.transform.position = animator.gameObject.transform.position + new Vector3(0,2,0);
    }
}
