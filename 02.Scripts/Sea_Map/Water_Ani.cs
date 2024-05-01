using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Water_ : MonoBehaviour
{
    public float amplitude = 0.5f; // ���̸� 1.0���� ����
    public float frequency = 0.9f; // �ӵ��� 2.0���� ����

    private Vector3 startPosition;

    void Start()
    {
        startPosition = transform.position; // ���� ��ġ ����
    }

    void Update()
    {
        Vector3 tempPos = startPosition;
        tempPos.y += Mathf.Sin(Time.fixedTime * Mathf.PI * frequency) * amplitude;

        transform.position = tempPos;
    }

}
