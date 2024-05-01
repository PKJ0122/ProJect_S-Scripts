using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class LineDrawer : MonoBehaviour
{
    LineRenderer _lineRenderer;
    public int maxDistance
    {
        get => _maxDistance;
        set => _maxDistance = value;
    }

    Vector3 initialPosition;
    int _maxDistance;

    private void Awake()
    {
        _lineRenderer = GetComponent<LineRenderer>();
        initialPosition = transform.position;
    }
    private void Update()
    {
        transform.position = initialPosition;
    }

    public void DrawCircle(float radius)
    {
        _lineRenderer.positionCount = 361; // ���� �׸� �� �ʿ��� ���� ���� ����
        _lineRenderer.useWorldSpace = false; // ���� ��ǥ�� ������� �ʵ��� ����

        // ���� �����ϴ� ���� ��ġ ���
        for (int i = 0; i < 361; i++)
        {
            float angle = i * Mathf.PI / 180f;
            float x = Mathf.Cos(angle) * radius;
            float z = Mathf.Sin(angle) * radius;
            _lineRenderer.SetPosition(i, new Vector3(x, 0, z));
        }
    }
}
