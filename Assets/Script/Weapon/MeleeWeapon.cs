using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class MeleeWeapon : Weapon
{
    [SerializeField] protected Collider attackRange;
    [SerializeField] protected AnimationClip animationClip; // �ٰŸ� ���� ���
    void Start()
    {
        if (attackRange != null)
            attackRange.enabled = false;
    }
  
    public override void SetData()
    {

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
        yield return new WaitForSeconds(0.5f); // ���� ���� �ð�
        attackRange.enabled = false;
    }
}
