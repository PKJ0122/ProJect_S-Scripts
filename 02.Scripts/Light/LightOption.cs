using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightOption : MonoBehaviour
{
    private Light targetLight; // 조절할 라이트
    public float maxIntensity = 5.0f; // 최대 intensity 값
    public float duration = 10.0f; // 강도를 최대치까지 올리는 데 걸리는 시간(초)

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
        float startIntensity = targetLight.intensity; // 시작 강도
        float time = 0;

        while (time < duration)
        {
            targetLight.intensity = Mathf.Lerp(startIntensity, maxIntensity, time / duration);
            time += Time.deltaTime;
            yield return null; // 다음 프레임까지 대기
        }

        // 마지막으로 최대 강도를 확실히 설정
        targetLight.intensity = maxIntensity;
    }
}
