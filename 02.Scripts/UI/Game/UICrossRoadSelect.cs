using System;
using UnityEngine;
using UnityEngine.Device;

public class UICrossRoadSelect : UIScreensBase
{
    CrossRoadDirectionButton select;
    LayerMask _clickObjectMask;

    public event Action onCrossRoadPopupHide;


    protected override void Awake()
    {
        base.Awake();
        _clickObjectMask = 1 << LayerMask.NameToLayer("ClickObject");
    }

    public override void InputAction()
    {
        base.InputAction();

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (select != null && Input.GetMouseButtonDown(0))
        {
            select.ButtonClick();
            select.ButtonMouseOff();
            select = null;
            inputActionEnable = false;
            return;
        }
        if (Physics.Raycast(ray, out RaycastHit hit, float.PositiveInfinity, _clickObjectMask))
        {
            if (select == null)
            {
                select = hit.collider.gameObject.GetComponent<CrossRoadDirectionButton>();
                select.ButtonMouseOn();
            }
            return;
        }
        if (select != null)
        {
            select.ButtonMouseOff();
            select = null;
        }
    }

    public override void Hide()
    {
        base.Hide();
        onCrossRoadPopupHide?.Invoke();
    }
}
