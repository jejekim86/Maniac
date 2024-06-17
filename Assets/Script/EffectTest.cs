using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class EffectTest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Invoke("Test", 2f);
    }

    public void Test()
    {
        PoolManager.instance.PlayEffect(EffectState.Test, transform);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
