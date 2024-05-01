using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : SingletonMonoBase<CameraManager>
{
    CinemachineVirtualCamera _camera;
    Transform _beForeTransform;


    protected override void Awake()
    {
        base.Awake();
        _camera = GetComponent<CinemachineVirtualCamera>();
    }

    public void ChangePointOfView(int gamblerId)
    {
        _camera.Follow = Gambler.Get(gamblerId).transform;
        _beForeTransform = Gambler.Get(gamblerId).transform;
    }

    public void ChangePointOfView(Transform transform)
    {
        _camera.Follow = transform;
    }

    public void ChangePointOfView()
    {
        _camera.Follow = _beForeTransform;
    }
}
