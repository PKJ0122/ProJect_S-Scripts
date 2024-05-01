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

            // ���� ������ ����Ͽ� ���� ��ġ�� ����մϴ�.
            transform.position = Vector3.Lerp(_start, _end, fracJourney);

            // �����ϸ� �̵��� ����ϴ�.
            if (fracJourney >= 1f)
            {
                enabled = false;
                Debug.Log("����!");
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
