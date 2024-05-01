using UnityEngine;

public class BuoyantRotator : MonoBehaviour
{
    public float buoyancyForce = 10f; // 부력의 크기
    public float rotationSpeed = 50f; // 회전 속도
    public float floatAmplitude = 0.5f; // 둥둥 떠다니는 높이의 범위
    public float floatFrequency = 1f; // 둥둥 떠다니는 속도

    private Rigidbody rb;
    private Vector3 startPos;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        startPos = transform.position; // 초기 위치 저장
    }

    void FixedUpdate()
    {
        // 부력 적용
        rb.AddForce(Vector3.up * buoyancyForce);

        // 좌우 무한 회전
        transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);

        // 둥둥 떠다니는 효과 적용
        Vector3 tempPos = startPos;
        tempPos.y += Mathf.Sin(Time.fixedTime * Mathf.PI * floatFrequency) * floatAmplitude;

        transform.position = new Vector3(transform.position.x, tempPos.y, transform.position.z);
    }
}
