using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Water_ : MonoBehaviour
{
    public float amplitude = 0.5f; // 높이를 1.0으로 증가
    public float frequency = 0.9f; // 속도를 2.0으로 증가

    private Vector3 startPosition;

    void Start()
    {
        startPosition = transform.position; // 시작 위치 저장
    }

    void Update()
    {
        Vector3 tempPos = startPosition;
        tempPos.y += Mathf.Sin(Time.fixedTime * Mathf.PI * frequency) * amplitude;

        transform.position = tempPos;
    }

}
