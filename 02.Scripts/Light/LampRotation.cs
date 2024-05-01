using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class LampLotation : MonoBehaviour
{
    Transform _rampTransform;
    Quaternion _rampRotation;
    public float startRotation = -20;
    public float endRotation = -60f;
    public float duration = 4f; // 한 방향으로의 전환에 걸리는 시간

    private void Awake()
    {
        _rampTransform = transform.Find("Lamp Light").GetComponent<Transform>();
        _rampRotation = _rampTransform.rotation;
    }

    private void Start()
    {
        StartCoroutine(RotateObject());
    }

    IEnumerator RotateObject()
    {
        while (true)
        {
            // A에서 B로 회전
            yield return RotateFromTo(startRotation, endRotation, duration);
            // B에서 A로 회전
            yield return RotateFromTo(endRotation, startRotation, duration);
        }
    }

    IEnumerator RotateFromTo(float startAngle, float endAngle, float duration)
    {
        Vector3 _rampEulerAngles = _rampRotation.eulerAngles;

        float rampangle_y = _rampEulerAngles.y;
        float rampangle_z = _rampEulerAngles.z;

        float timeElapsed = 0f;
        while (timeElapsed < duration)
        {
            float currentAngle = Mathf.Lerp(startAngle, endAngle, timeElapsed / duration);
            _rampTransform.rotation = Quaternion.Euler(currentAngle, rampangle_y, rampangle_z);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        _rampTransform.rotation = Quaternion.Euler(endAngle, rampangle_y, rampangle_z);

        yield return new WaitForSeconds(2f);
    }

}
