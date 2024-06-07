using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    protected WeaponData w_data;
    protected float reloadT = 1f;
    protected float timeCount = 0;

    public float GetReloadTime()
    {
        return reloadT;
    }

    abstract public void SetData();

    abstract public bool Attack();
}