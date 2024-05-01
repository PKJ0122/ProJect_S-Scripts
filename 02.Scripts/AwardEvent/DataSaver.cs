using System;
using System.Collections.Generic;

public class DataSaver : SingletonMonoBase<DataSaver>
{
    public Dictionary<int, GamblerData> gamblers = new Dictionary<int, GamblerData>();

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(gameObject);
    }
}

