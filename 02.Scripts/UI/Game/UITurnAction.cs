using Photon.Pun;
using System;
using UnityEngine.UI;

public class UITurnAction : UIScreensBase
{
    Button _dice;
    Button _item;
    Button _map;
    LookAtMap _lookatMap;
    bool _isMap;
    public event Action onButtonClick;


    protected override void Awake()
    {
        base.Awake();
        _dice = transform.Find("Button - Dice").GetComponent<Button>();
        _item = transform.Find("Button - Item").GetComponent<Button>();
        _map = transform.Find("Button - Map").GetComponent<Button>();
        ObjectPoolingManager.instance.AddObjPool("LookAtMap", 1);

        _dice.onClick.AddListener(() =>
        {
            onButtonClick?.Invoke();
            base.Hide();
        });

        ItemsManager.instance.onItemUse += value =>
        {
            _item.interactable = !value;
            if (!_dice.interactable)
                _dice.interactable = true;
            if (!_map.interactable)
                _map.interactable = true;
            Show();
        };

        _item.onClick.AddListener(() =>
        {
            UIInventoryPopup a = UIManager.instance.Get<UIInventoryPopup>();
            a.Show();
            _item.interactable = false;
            _dice.interactable = false;
            _map.interactable = false;
            Hide();
        });

        _map.onClick.AddListener(() =>
        {
            _isMap = !_isMap;
            if (!_isMap)
            {
                _lookatMap.ReleaseObject();
                CameraManager.instance.ChangePointOfView(Gambler.Get(PhotonNetwork.LocalPlayer.ActorNumber).transform);
                _item.interactable = !ItemsManager.instance.isUse;
                _dice.interactable = true;
            }
            else
            {
                _lookatMap = ObjectPoolingManager.instance.GetGo("LookAtMap").GetComponent<LookAtMap>();
                _lookatMap.transform.position = Gambler.Get(PhotonNetwork.LocalPlayer.ActorNumber).transform.position;
                CameraManager.instance.ChangePointOfView(_lookatMap.transform);
                _item.interactable = false;
                _dice.interactable = false;
            }
        });
    }
    private void Start()
    {
        UIManager.instance.Get<UIInventoryPopup>().isShowEvent += value =>
        {
            _item.interactable = !ItemsManager.instance.isUse;
            _dice.interactable = !value;
            _map.interactable = !value;
        };
    }
}