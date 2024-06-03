using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LongRangeWeapon : Weapon
{
    [SerializeField] protected Bullet bullet;
    [SerializeField] protected Transform fireTr;
    public override bool Attack()
    {
        return base.Attack();
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
