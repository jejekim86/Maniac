using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LongRangeWeaponAR : LongRangeWeapon
{
    public override bool Attack()
    {
        if (timeCount < reloadT)
        {
            return false;
        }
        Bullet newBullet;
        PoolManager.instance.bulletPool.GetObject(out newBullet);
        newBullet.transform.position = fireTr.transform.position;
        newBullet.transform.rotation = fireTr.rotation;
        timeCount = 0;
        return true;
    }

    public override void SetData()
    {
        base.SetData();
    }

    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        base.Update();
    }
}