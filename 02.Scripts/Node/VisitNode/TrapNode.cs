using System.Collections;
using UnityEngine;

public class TrapNode : NodeBase
{
    const int damage = 10;
    YieldInstruction delay3f = new WaitForSeconds(3);
    YieldInstruction delay1f = new WaitForSeconds(1);

    public override IEnumerator Visit(int gamblerId)
    {
        GamblerStatistic gamblerStatistic = Gambler.Get(gamblerId).GetComponent<GamblerStatistic>();

        yield return delay1f;

        if (gamblerStatistic.hp <= damage)
        {
            gamblerStatistic.hp -= damage;
            yield return delay3f;
            yield break;
        }
        gamblerStatistic.hp -= damage;
    }
}
