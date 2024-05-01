using ExitGames.Client.Photon.StructWrapping;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;


public class GameManager : SingletonMonoBase<GameManager>
{
    int _maxTurn = 10;
    int _turnCount = 0;

    public int turnCount
    {
        get => _turnCount;
        set
        {
            _turnCount = value;
            onTurnChage?.Invoke(value, _maxTurn);
        }
    }

    bool _allPlayerLoad;

    public int[] turnOrder { get; set; }

    public event Action<int, int> onTurnChage;

    public Transform[] strartPoint;
    public NodeInfo strartNode;
    public NodeInfo spawnNode;
  

    [field: SerializeField] public int frontalAngle { get; private set; }



    protected override void Awake()
    {
        base.Awake();

        Time.timeScale = 1.5f;

        PhotonNetwork.LocalPlayer.SetCustomProperties(new Hashtable
        {
                     { "isLoading", true }
        });

        if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("isTrunCount", out object value))
        {
            _maxTurn = (int)value;
        }
        else
        {
            _maxTurn = 10;
        }

        StartCoroutine(PlayerLoadChack());
    }

    private void Start()
    {
        Application.runInBackground = true;
    }

    IEnumerator PlayerLoadChack()
    {
        YieldInstruction delay = new WaitForSeconds(1f);
        while (!_allPlayerLoad)
        {
            yield return delay;
            _allPlayerLoad = true;

            foreach (Player player in PhotonNetwork.PlayerList)
            {
                _allPlayerLoad = (bool)player.CustomProperties["isLoading"];
            }
        }

        CharEnum charName = (CharEnum)PhotonNetwork.LocalPlayer.CustomProperties["isChar"];
        Vector3 startPointV = new Vector3(strartPoint[0].position.x, strartPoint[0].position.y, strartPoint[0].position.z);
        PhotonNetwork.Instantiate($"Character/{charName}", startPointV, Quaternion.identity);

        StartCoroutine(GameStartLogic());
    }

    IEnumerator GameStartLogic()
    {
        yield return new WaitUntil(() => turnOrder != null);
        yield return StartCoroutine(UIManager.instance.Get<UITurnDecide>().TrunUISet());
        int order = Array.IndexOf(turnOrder, PhotonNetwork.LocalPlayer.ActorNumber);
        Vector3 spawn = new Vector3(strartPoint[order].position.x,
                                    strartPoint[order].position.y,
                                    strartPoint[order].position.z);
        Gambler.Get(turnOrder[order]).transform.position = spawn;
        yield return StartCoroutine(NodeCreater.instance.StarNodeCreat(turnOrder[0]));
        yield return StartCoroutine(NodeCreater.instance.ArmoryNodeCreat(turnOrder[0]));
        CameraManager.instance.ChangePointOfView(turnOrder[0]);

        for (int i = 0; i < turnOrder.Length; i++)
        {
            Gambler.Get(turnOrder[i]).statistic.ranke = i;
        }

        if (PhotonNetwork.IsMasterClient)
            Gambler.Get(PhotonNetwork.LocalPlayer.ActorNumber).FirstTrunStrat();
    }

    public int GetNextOrder(int currentOrder)
    {
        for (int i = 0; i < turnOrder.Length - 1; i++)
        {
            if (turnOrder[i] == currentOrder)
            {
                return turnOrder[i + 1];
            }
        }

        if(_turnCount == _maxTurn)
        {
            Gambler.Get(PhotonNetwork.LocalPlayer.ActorNumber).DataRPCStart();           
        }

        return turnOrder[0];
    }

}
