using Photon.Pun;
using System;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class GamblerStatistic : MonoBehaviour, IPunObservable
{
    const int MAX_HP = 30;
    const int START_COIN = 30;

    int _star = 0;
    int _coin = START_COIN;
    int _hp = MAX_HP;
    int _ranke = 0;
    int _dmage = 0;
    int _kill = 0;
    int _miniGameScore = 0;
    bool _isPuppet;

    #region Event
    public bool isPuppet
    {
        get => _isPuppet;
        set 
        { 
            _isPuppet = value;
            onPuttetChanged?.Invoke(value);
        }
    }

    public int star
    {
        get => _star;
        set
        {
            if (_star == value)
                return;

            _star = value;
            onStarChanged?.Invoke(value);
        }
    }
    public int coin
    {
        get => _coin;
        set
        {
            if (_coin == value)
                return;

            _coin = value;
            onCoinChanged?.Invoke(value);
        }
    }
    public int hp
    {
        get => _hp;
        set
        {
            if (value == MAX_HP || _hp == value)
            {
                _hp = value;
                onHpChanged?.Invoke(_hp);
                return;
            }

            int temp = _hp;
            _hp = (int)MathF.Min(value, MAX_HP);
            onHpChanged?.Invoke(_hp);

            if (_hp < temp)
            {
                if (_hp <= 0)
                {
                    StartCoroutine(GetComponent<Gambler>().GamblerDie());
                }
                else
                {
                    GameObject effect = ObjectPoolingManager.instance.GetGo("Blood");
                    effect.transform.position = transform.position + new Vector3(0, 2, 0);
                }
            }
            else if (_hp > temp)
            {
                SoundManager.instance.SEFPlay("Heal");
                GameObject effect = ObjectPoolingManager.instance.GetGo("Heal");
                effect.transform.position = transform.position + new Vector3(0, 2, 0);
            }
        }
    }
    public int ranke
    {
        get => _ranke;
        set
        {
            _ranke = value;
            onRankeChanged?.Invoke(value + 1);
        }
    }
    public int dmage
    {
        get => _dmage;
        set => _dmage = value;
    }
    public int kill
    {
        get => _kill;
        set => _kill = value;
    }
    public int MiniGameScore 
    { 
        get => _miniGameScore;
        set
        {
            _miniGameScore = value;
            onMiniGameScoreChanged?.Invoke(_miniGameScore);
        }
    }

    public event Action<int> onStarChanged;
    public event Action<int> onCoinChanged;
    public event Action<int> onHpChanged;
    public event Action<int> onRankeChanged;
    public event Action<int> onMiniGameScoreChanged;
    public event Action<bool> onPuttetChanged;
    #endregion

   
    public void ClearMiniGameEvent()
    {
        onMiniGameScoreChanged = null;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(star);
            stream.SendNext(coin);
            stream.SendNext(hp);
            stream.SendNext(dmage);
            stream.SendNext(kill);
            stream.SendNext(MiniGameScore);
        }
        else
        {
            star = (int)stream.ReceiveNext();
            coin = (int)stream.ReceiveNext();
            hp = (int)stream.ReceiveNext();
            dmage = (int)stream.ReceiveNext();
            kill = (int)stream.ReceiveNext();
            MiniGameScore = (int)stream.ReceiveNext();
        }
    }
}