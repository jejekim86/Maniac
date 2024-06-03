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
        // 공격 중이 아니면 공격 시작
        if (!isAttack)
        {
            isAttack = true;
            StartCoroutine(Attack());
        }

        // 타겟이 존재하면 타겟을 천천히 바라보도록 회전
        if (target != null)
        {
            Vector3 direction = (target.position - transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Lerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
        }

        // 공격 시 위치 고정
        enemyAgent.SetDestination(transform.position);
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