using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestFire : PoolAble
{
    float timer;
    private void Update()
    {
        if(timer < 2)
        {
            timer += Time.deltaTime;
        }
        else
        {
            timer = 0;
            ReleaseObject();
        }
    }
}
