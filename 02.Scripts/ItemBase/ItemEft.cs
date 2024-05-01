using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemEft : PoolAble
{
    float _timer;

    private void OnEnable()
    {
        _timer = 0f;
    }

    private void Update()
    {
        if(_timer < 4f)
        {
            _timer += Time.deltaTime;
        }
        else
        {
            _timer = 4f;
            ReleaseObject();
        }
    }
}
