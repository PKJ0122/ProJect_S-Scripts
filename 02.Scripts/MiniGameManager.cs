using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniGameManager : SingletonMonoBase<MiniGameManager>
{
    public static Dictionary<MiniGameKind, MiniGameBase> minigames = new Dictionary<MiniGameKind, MiniGameBase>();

    private void OnDestroy()
    {
        minigames.Clear();
    }
}