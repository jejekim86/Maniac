using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    protected WeaponData w_data;
    [SerializeField] protected float reloadT;
    protected float timeCount;

    abstract public void SetData();

    abstract public bool Attack();
}