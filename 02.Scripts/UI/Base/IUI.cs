using System;
using System.Collections.Generic;
using UnityEngine.EventSystems;


/// <summary>
/// ĵ���������� �⺻ �������̽�
/// </summary>
public interface IUI
{
    /// <summary>
    /// Canvas �� ���� ����
    /// </summary>
    int sortingOrder { get; set; }

    /// <summary>
    /// ������ �Է� ��ȣ�ۿ� ���� ����
    /// </summary>
    bool inputActionEnable { get; set; }


    /// <summary>
    /// ������ �Է� ��ȣ�ۿ� ���� ���ΰ� �ٲ������
    /// </summary>
    event Action<bool> onInputActionEnableChanged;


    /// <summary>
    /// Canvas Ȱ��ȭ
    /// </summary>
    void Show();

    /// <summary>
    /// Canvas ��Ȱ��ȭ
    /// </summary>
    void Hide();

    /// <summary>
    /// �������� ��ȣ �ۿ�
    /// </summary>
    void InputAction();

    /// <summary>
    /// �� Canvs ���� � RaycastTarget�� �����ϱ� ���� ���
    /// </summary>
    /// <param name="results"> ��� ��ȯ�� ����</param>
    /// <returns> ������ Ÿ���� ������ Ʈ�� </returns>
    bool Raycast(List<RaycastResult> results);
}

