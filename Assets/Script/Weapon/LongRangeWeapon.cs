using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LongRangeWeapon : Weapon
{
    [SerializeField] protected Bullet bullet; // -->> ���ʿ� ObjectPool���� ���� �ϴ°Ŷ� �ʿ���� ���࿡ Bullet�� ������ �ٸ��� PoolManager���� �ٲ����
    [SerializeField] protected Transform fireTr;
    public override bool Attack()
    {
        return false;
    }


    public override void SetData()
    {
    }
}
