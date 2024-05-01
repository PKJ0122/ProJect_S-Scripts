using UnityEngine;

public class BuoyantRotator : MonoBehaviour
{
    public float buoyancyForce = 10f; // �η��� ũ��
    public float rotationSpeed = 50f; // ȸ�� �ӵ�
    public float floatAmplitude = 0.5f; // �յ� ���ٴϴ� ������ ����
    public float floatFrequency = 1f; // �յ� ���ٴϴ� �ӵ�

    private Rigidbody rb;
    private Vector3 startPos;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        startPos = transform.position; // �ʱ� ��ġ ����
    }

    void FixedUpdate()
    {
        // �η� ����
        rb.AddForce(Vector3.up * buoyancyForce);

        // �¿� ���� ȸ��
        transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);

        // �յ� ���ٴϴ� ȿ�� ����
        Vector3 tempPos = startPos;
        tempPos.y += Mathf.Sin(Time.fixedTime * Mathf.PI * floatFrequency) * floatAmplitude;

        transform.position = new Vector3(transform.position.x, tempPos.y, transform.position.z);
    }
}
