using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;

public enum EffectState
{
    Test, max
}

public class PoolManager : MonoBehaviour
{
    [SerializeField] Effect[] effects;
    ObjectPool<Effect>[] effectPools = new ObjectPool<Effect>[10];

    static public PoolManager instance { get; private set; }
    public ObjectPool<Bullet> bulletPool;
    private void Awake()
    {
        if(instance == null)
            instance = this;
    }

    void Start()
    {
        bulletPool.Initialize();
        for(int i = 0; i < effects.Length; i++)
        {
            effectPools[i] = new ObjectPool<Effect>();
            effectPools[i].Initialize(effects[i]);
        }
    }
        
    public void PlayEffect(EffectState _effect, Transform transform)
    {
        if (effectPools[(int)_effect].GetObject(out Effect effect))
        {
            effect.SetEffect(transform, effectPools[(int)_effect].PutInPool);
        }
    }

}
