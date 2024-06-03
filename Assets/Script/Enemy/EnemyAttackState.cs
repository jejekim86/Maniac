using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using UnityEngine.AI;

// 적의 공격 상태를 정의하는 클래스
public class EnemyAttackState : MonoBehaviour, State
{
    // 공격할 목표
    [SerializeField] private Transform target;
    // 사용할 무기
    [SerializeField] private GameObject weapon;

    // 적 공격 정보 헤더
    [Header("적 공격 정보")]
    // 공격 쿨타임
    [SerializeField] private float coolTime;
    // 공격 데미지
    [SerializeField] private float damage = 5;
    // 공격 범위
    [SerializeField] private float attackRange;
    // 공격 중인지 여부
    private bool isAttack;

    // 애니메이터 컴포넌트
    private Animator ani;
    // NavMeshAgent 컴포넌트
    private NavMeshAgent enemyAgent;
    // Enemy 스크립트
    private Enemy enemy;

    // 상태 진입 시 호출되는 메서드
    public void EnterState()
    {
        // 애니메이터, NavMeshAgent, Enemy 컴포넌트 초기화
        if (ani == null || enemyAgent == null || enemy == null)
        {
            ani = GetComponent<Animator>();
            enemyAgent = GetComponent<NavMeshAgent>();
            enemy = GetComponent<Enemy>();
        }

        // 공격 애니메이션 시작
        ani.SetBool("isAttack", true);

        // EnemyAttack 컴포넌트를 가져와서 공격 실행
        EnemyAttack enemyAttack = GetComponent<EnemyAttack>();
        enemyAttack.Attack();

        // 무기를 활성화
        weapon.SetActive(true);
    }

    // 상태 업데이트 시 호출되는 메서드
    public void UpdateState()
    {
        // 공격 중이 아니면 공격 시작
        if (!isAttack)
        {
            isAttack = true;

            // 공격 시 플레이어를 바라보게 함
            transform.LookAt(target);

            // 공격 시 위치 고정
            enemyAgent.SetDestination(transform.position);

            // 공격 코루틴 시작
            StartCoroutine(Attack());
        }
    }

    // 공격을 수행하는 코루틴
    private IEnumerator Attack()
    {
        // 쿨타임 동안 대기
        yield return new WaitForSeconds(coolTime);
        // 공격이 끝났음을 표시
        isAttack = false;
    }

    // 상태 종료 시 호출되는 메서드
    public void ExitState()
    {
        // 공격 애니메이션 종료
        ani.SetBool("isAttack", false);
        // 무기를 비활성화
        weapon.SetActive(false);
    }
}