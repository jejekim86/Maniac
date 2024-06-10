using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class MeleeWeapon : Weapon
{
    [SerializeField] protected Collider attackRange;
    [SerializeField] protected AnimationClip animationClip; // �ٰŸ� ���� ���
    [SerializeField] protected GameObject stick;
    [SerializeField] private float attackDuration = 0.5f; // ���� ���� �ð�
    [SerializeField] private float damage = 15f; // ���� �����

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

        yield return new WaitForSeconds(attackDuration); // ���� ���� �ð�
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
