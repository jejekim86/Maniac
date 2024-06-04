using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
public class PoolManager : MonoBehaviour
{
    static public PoolManager instance { get; private set; }

    public ObjectPool<Bullet> bulletPool;
    Thread threed1;
    Thread threed2;
    private void Awake()
    {
        if(instance == null)
            instance = this;
    }

    void Start()
    {
        bulletPool.Initialize();
    }

    
}
