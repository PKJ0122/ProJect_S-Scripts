using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemModlePooling : PoolAble
{
    float timer = 0;

    private void OnEnable()
    {
        timer = 0;
    }

    private void Update()
    {
        if(timer < 2)
        {
            timer += Time.deltaTime;
        }
        else
        {
            ReleaseObject();
        }
    }
}
