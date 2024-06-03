using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    protected WeaponData w_data;

    protected float timeCount;
    public float reloadT = 1.0f; // 임시 값 추후에 w_data.reloadTime 으로 교체

    virtual protected void Start()
    {
        timeCount = 100;
    }

    virtual protected void Update()
    {
        timeCount += Time.deltaTime;
    }

    virtual public void SetData()
    {
        //TODO DBDATA가져와야함
    }

    virtual public bool Attack()
    {
        return false;
    }
}