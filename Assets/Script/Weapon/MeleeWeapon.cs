using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class MeleeWeapon : Weapon
{
    [SerializeField] protected Collider attackRange;
    [SerializeField] protected AnimationClip animationClip; // 근거리 공격 모션
    override protected void Start()
    {
        base.Start();
        if (attackRange != null)
            attackRange.enabled = false;
    }

    override protected void Update()
    {
        base.Update();
    }
    public override void SetData()
    {
        base.SetData();
    }

    public override bool Attack()
    {
        if (timeCount >= reloadT)
        {
            StartCoroutine(MeleeRangeCheck());
            timeCount = 0;
            return true;
        }
        return false;
    }
    IEnumerator MeleeRangeCheck()
    {
        attackRange.enabled = true;
        yield return new WaitForSeconds(0.5f); // 공격 지속 시간
        attackRange.enabled = false;
    }
}
