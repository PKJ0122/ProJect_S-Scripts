using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtMap : PoolAble
{
    float _x;
    float _z;


    private void Update()
    {
        _x = Input.GetAxisRaw("Horizontal");
        _z = Input.GetAxisRaw("Vertical");

        transform.Translate(Time.deltaTime * 13 * new Vector3(_x, 0, _z));
    }

}
