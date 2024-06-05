using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class MeleeWeapon : Weapon
{
    [SerializeField] protected Collider attackRange;
    [SerializeField] protected AnimationClip animationClip; // 근거리 공격 모션
    [SerializeField] protected GameObject stick;
    [SerializeField] private float attackDuration = 0.5f; // 공격 지속 시간
    [SerializeField] private float damage = 15f; // 공격 대미지

    void Start()
    {
        stick.SetActive(false);

        if (attackRange != null)
            attackRange.enabled = false;
    }

    private void Update()
    {
        timeCount += Time.deltaTime;
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
        stick.SetActive(true);

        yield return new WaitForSeconds(attackDuration); // 공격 지속 시간
        attackRange.enabled = false;
        stick.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            other.GetComponent<Enemy>().GetDamage(damage);
        }
    }
}
