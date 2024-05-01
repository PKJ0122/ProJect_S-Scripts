using System;
using UnityEngine;

public class CrossRoadDirectionButton : MonoBehaviour
{
    [SerializeField] int _crossRoadDirectionButton;

    public static CrossRoadDirectionButton leftButton;
    public static CrossRoadDirectionButton rightButton;

    public event Action<int> onButtonClick;

    MeshRenderer meshRenderer;
    [SerializeField] Material _nomalState;
    [SerializeField] Material _selectState;
    Vector3 usuallyScale;


    private void Awake()
    {
        if (leftButton == null && _crossRoadDirectionButton == 0)
            leftButton = this;

        if (rightButton == null && _crossRoadDirectionButton == 1)
            rightButton = this;


        meshRenderer = GetComponent<MeshRenderer>();
        usuallyScale = transform.localScale;
        transform.localScale *= 0.7f;
    }

    private void OnDestroy()
    {
        leftButton = null;
        rightButton = null;
    }

    public void ButtonMouseOn()
    {
        meshRenderer.material = _selectState;
        transform.localScale = usuallyScale;
    }

    public void ButtonMouseOff()
    {
        meshRenderer.material = _nomalState;
        transform.localScale *= 0.7f;
    }

    public void ButtonClick()
    {
        onButtonClick?.Invoke(_crossRoadDirectionButton);
    }
}