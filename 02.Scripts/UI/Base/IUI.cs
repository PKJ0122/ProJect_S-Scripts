using System;
using System.Collections.Generic;
using UnityEngine.EventSystems;


/// <summary>
/// 캔버스단위의 기본 인터페이스
/// </summary>
public interface IUI
{
    /// <summary>
    /// Canvas 의 정렬 순서
    /// </summary>
    int sortingOrder { get; set; }

    /// <summary>
    /// 유저의 입력 상호작용 가능 여부
    /// </summary>
    bool inputActionEnable { get; set; }


    /// <summary>
    /// 유저의 입력 상호작용 가능 여부가 바뀌었을때
    /// </summary>
    event Action<bool> onInputActionEnableChanged;


    /// <summary>
    /// Canvas 활성화
    /// </summary>
    void Show();

    /// <summary>
    /// Canvas 비활성화
    /// </summary>
    void Hide();

    /// <summary>
    /// 유저와의 상호 작용
    /// </summary>
    void InputAction();

    /// <summary>
    /// 이 Canvs 내의 어떤 RaycastTarget을 감지하기 위한 기능
    /// </summary>
    /// <param name="results"> 결과 반환용 버퍼</param>
    /// <returns> 감지된 타겟이 있으면 트루 </returns>
    bool Raycast(List<RaycastResult> results);
}

