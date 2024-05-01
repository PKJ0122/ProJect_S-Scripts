using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightOption : MonoBehaviour
{
    private Light targetLight; // ������ ����Ʈ
    public float maxIntensity = 5.0f; // �ִ� intensity ��
    public float duration = 10.0f; // ������ �ִ�ġ���� �ø��� �� �ɸ��� �ð�(��)

    private void Awake()
    {
        targetLight = GetComponent<Light>();
    }

    private void OnEnable()
    {
        StartCoroutine(IncreaseLightIntensityOverTime(duration));
    }

    private IEnumerator IncreaseLightIntensityOverTime(float duration)
    {
        float startIntensity = targetLight.intensity; // ���� ����
        float time = 0;

        while (time < duration)
        {
            targetLight.intensity = Mathf.Lerp(startIntensity, maxIntensity, time / duration);
            time += Time.deltaTime;
            yield return null; // ���� �����ӱ��� ���
        }

        // ���������� �ִ� ������ Ȯ���� ����
        targetLight.intensity = maxIntensity;
    }
}
