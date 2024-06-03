using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttack : LongRangeWeaponAR
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
}
