/// <summary>
/// 게임 종료시의 겜블러의 데이터 구조체
/// </summary>
public struct GamblerData
{
    /// <summary>
    /// 게임 종료시 겜블러의 데이터 구조체
    /// </summary>
    /// <param name="star">갬블러의 별</param>
    /// <param name="coin">갬블러의 코인</param>
    /// <param name="dmage">갬블러의 딜량</param>
    /// <param name="kill">갬블러의 킬수</param>
    public GamblerData(int star, int coin, int dmage, int kill, int id = 0) 
    {
        _star = star;
        _coin = coin;
        _dmage = dmage;
        _kill = kill;
        _id = id;
    }

    int _star;
    int _coin;
    int _dmage;
    int _kill;
    int _id;

    public int Star { get => _star; set => _star = value; }
    public int Coin { get => _coin; set => _coin = value; }
    public int Dmage { get => _dmage; set => _dmage = value; }
    public int Kill { get => _kill; set => _kill = value; }
    public int Id { get => _id; set => _id = value; }
}