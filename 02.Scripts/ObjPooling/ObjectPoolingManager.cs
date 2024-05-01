using ExitGames.Client.Photon;
using Photon.Pun.Demo.Asteroids;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Pool;
using static UnityEngine.Rendering.DebugUI;

public class ObjectPoolingManager : SingletonMonoBase<ObjectPoolingManager>
{
    private string _name;

    Dictionary<string, GameObject> _poolPrefabs = new Dictionary<string, GameObject>();
    private Dictionary<string, IObjectPool<GameObject>> _ojbectPoolDic = new Dictionary<string, IObjectPool<GameObject>>();

    public void AddObjPool(string name, int count)
    {
        _name = name;
        if (_poolPrefabs.ContainsKey(name))
        {
            Debug.LogFormat("이미 등록된 오브젝트입니다.");
            return;
        }
        _poolPrefabs.Add(name, Resources.Load<GameObject>($"Pooling/{name}"));        
        IObjectPool<GameObject> pool = new ObjectPool<GameObject>(CreatePooledItem, OnTakeFromPool, OnReturnedToPool,
            OnDestroyPoolObject, true, count, count);
        _ojbectPoolDic.Add(name, pool);
        for (int i = 0; i < count; i++)
        {
            PoolAble a = CreatePooledItem().GetComponent<PoolAble>();
            a.Pool.Release(a.gameObject);
        }
    }
    private GameObject CreatePooledItem()
    {
        GameObject poolGo = Instantiate(Resources.Load<GameObject>($"Pooling/{_name}"));
        poolGo.GetComponent<PoolAble>().Pool = _ojbectPoolDic[_name];
        return poolGo;
    }

    // 대여
    private void OnTakeFromPool(GameObject poolGo)
    {
        poolGo.SetActive(true);
    }

    // 반환
    private void OnReturnedToPool(GameObject poolGo)
    {
        poolGo.SetActive(false);
    }

    // 삭제
    private void OnDestroyPoolObject(GameObject poolGo)
    {
        Destroy(poolGo);
    }
    public GameObject GetGo(string goName)
    {
        _name = goName;

        if (_poolPrefabs.ContainsKey(goName) == false)
        {
            Debug.LogFormat("{0} 오브젝트풀에 등록되지 않은 오브젝트입니다.", goName);
            return null;
        }

        return _ojbectPoolDic[goName].Get();
    }
}
