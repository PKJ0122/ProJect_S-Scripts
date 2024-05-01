using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealNode : NodeBase
{
    YieldInstruction delay = new WaitForSeconds(1);

    public override IEnumerator Visit(int gamblerId)
    {
        yield return delay;
        Gambler.Get(gamblerId).statistic.hp += 10;
    }
}
