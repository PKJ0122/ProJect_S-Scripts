using Photon.Pun;
using System.Net;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(PhotonView), typeof(PhotonTransformView))]
public class AirGunMove : MonoBehaviour
{
    bool _isAct;
    float speed = 20; 
    float journeyLength;
    float startTime;

    Vector3 _start;
    Vector3 _end;

    // Update is called once per frame
    void Update()
    {
        if (_isAct)
        {
            float distCovered = (Time.time - startTime) * speed;
            float fracJourney = distCovered / journeyLength;

            // 선형 보간을 사용하여 현재 위치를 계산합니다.
            transform.position = Vector3.Lerp(_start, _end, fracJourney);

            // 도착하면 이동을 멈춥니다.
            if (fracJourney >= 1f)
            {
                enabled = false;
                Debug.Log("도착!");
            }
        }
    }

    public void OnMoveStart(Vector3 start, Vector3 end) 
    {
        SoundManager.instance.SEFPlay("Punch");
        _start = start;
        _end = end;
        journeyLength = Vector3.Distance(start, end);
        startTime = Time.time;
        _isAct = true;
    }
}
