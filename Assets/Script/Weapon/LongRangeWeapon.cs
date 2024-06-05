using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LongRangeWeapon : Weapon
{
    [SerializeField] protected Bullet bullet; // -->> 애초에 ObjectPool에서 생성 하는거라 필요없음 만약에 Bullet의 종류가 다르면 PoolManager에서 바꿔야함
    [SerializeField] protected Transform fireTr;
    public override bool Attack()
    {
        return false;
    }


    public override void SetData()
    {
    }
}
