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
        // �̰� LongRangeWeapon�� Bullet�� �����ִµ� �̰͵� �����ϸ� ������
        Bullet newBullet;
        PoolManager.instance.bulletPool.GetObject(out newBullet); // �̰͵� ��¦ �ָ��� 
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