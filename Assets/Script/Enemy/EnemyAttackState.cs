using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

// 적의 공격 상태를 정의하는 클래스
public class EnemyAttackState : MonoBehaviour, State
{
    [SerializeField] private Transform target; // 공격할 목표
    [SerializeField] private GameObject weapon; // 사용할 무기

    [Header("적 공격 정보")]
    [SerializeField] private float coolTime; // 공격 쿨타임
    [SerializeField] private float damage = 5; // 공격 데미지
    [SerializeField] private float attackRange; // 공격 범위
    private bool isAttack;

    private Animator ani; // 애니메이터 컴포넌트
    private NavMeshAgent enemyAgent; // NavMeshAgent 컴포넌트
    private Enemy enemy; // Enemy 스크립트

    public void EnterState()
    {
        if (ani == null || enemyAgent == null || enemy == null)
        {
            ani = GetComponent<Animator>();
            enemyAgent = GetComponent<NavMeshAgent>();
            enemy = GetComponent<Enemy>();
        }

        if (ani != null)
        {
            ani.SetBool("isAttack", true); // 공격 애니메이션 시작
        }

        if (enemy != null)
        {
            enemy.Attack();
        }

        if (weapon != null)
        {
            weapon.SetActive(true); // 무기 활성화
        }
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
        if (enemyAgent != null)
        {
            enemyAgent.SetDestination(transform.position);
        }
    }

    private IEnumerator Attack()
    {
        if (enemy != null)
        {
            enemy.Attack();
        }
        yield return new WaitForSeconds(coolTime); // 쿨타임 동안 대기
        isAttack = false; // 공격이 끝났음을 표시
    }

    public void ExitState()
    {
        if (ani != null)
        {
            ani.SetBool("isAttack", false); // 공격 애니메이션 종료
        }

        if (weapon != null)
        {
            weapon.SetActive(false); // 무기 비활성화
        }
    }
}
