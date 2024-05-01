using Photon.Pun;
using System.Collections;

public interface ICoinRouletteEvent
{
    string eventName { get; }

    IEnumerator CoinEvent(int gamblerId , PhotonView view);
}