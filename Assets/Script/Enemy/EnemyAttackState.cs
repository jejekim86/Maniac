using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAttackState : MonoBehaviour, State
{
    [SerializeField] private Transform target;
    [SerializeField] private GameObject weapon;

    [Header("적 공격 정보")]
    [SerializeField] private float coolTime;
    [SerializeField] private float damage = 5;
    [SerializeField] private float attackRange;
    private bool isAttack;

    private Animator ani;
    private NavMeshAgent enemyAgent;
    private Enemy enemy;

    public void EnterState()
    {
        if (ani == null || enemyAgent == null || enemy == null)
        {
            ani = GetComponent<Animator>();
            enemyAgent = GetComponent<NavMeshAgent>();
            enemy = GetComponent<Enemy>();
        }

        ani.SetBool("isAttack", true);
        EnemyAttack enemyAttack = GetComponent<EnemyAttack>();
        enemyAttack.Attack();
        weapon.SetActive(true);

    }

    public void UpdateState()
    {
        if (!isAttack)
        {
            isAttack = true;

            // 공격시 플레이어 바라보기
            transform.LookAt(target);

            // 공격시 위치 고정
            enemyAgent.SetDestination(transform.position);

            StartCoroutine(Attack());
        }
    }

    private IEnumerator Attack()
    {
        yield return new WaitForSeconds(coolTime);
        isAttack = false;
    }

    public void ExitState()
    {
        ani.SetBool("isAttack", false);
        weapon.SetActive(false);
    }
}