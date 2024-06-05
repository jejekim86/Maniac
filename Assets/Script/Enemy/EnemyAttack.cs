using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttack : LongRangeWeaponAR
{
    // 이거 Weapon Class에 상속 받을 필요 X 이거 그냥 EnemyClass에 Attack 함수 구현해서 사용 
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