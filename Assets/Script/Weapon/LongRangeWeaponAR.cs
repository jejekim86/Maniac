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
        // 이거 LongRangeWeapon에 Bullet을 갖고있는데 이것도 수정하면 좋을듯
        Bullet newBullet;
        PoolManager.instance.bulletPool.GetObject(out newBullet); // 이것도 살짝 애매함 
        newBullet.transform.position = fireTr.transform.position;
        newBullet.transform.rotation = fireTr.rotation;
        timeCount = 0;
        return true;
    }

    private void Update()
    {
        timeCount += Time.deltaTime;
    }

    public override void SetData()
    {

    }

}