using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectManager : MonoBehaviour
{
    [SerializeField] MonoBehaviour[] effects = new MonoBehaviour[(int)EffectState.max];
    ObjectPool<MonoBehaviour>[] effectPools = new ObjectPool<MonoBehaviour>[(int)EffectState.max];

    public static EffectManager instance { get; private set; }

    private void Awake()
    {
        if (instance == null)
            instance = this;
        for (int i = 0; i < effects.Length; i++)
        {
            effectPools[i].Initialize(effects[i]);
        }
    }

    public void PlayEffect(EffectState effect, Transform transform)
    {
        if (effectPools[(int)effect].GetObject(out MonoBehaviour item))
        {
            item.transform.position = transform.position;
            item.transform.rotation = transform.rotation;
        }
    }
}
