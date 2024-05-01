using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectObject : PoolAble
{
    private void OnParticleSystemStopped()
    {
        ReleaseObject();
    }
}