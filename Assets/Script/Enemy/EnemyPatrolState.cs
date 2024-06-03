using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyPatrolState : MonoBehaviour, State
{
    [Header("적 위치 정보")]
    [SerializeField] private GameObject[] walkPoints;
    private int curEnemyPos = 0;
    private float walkingPointRadius = 2;

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

        ani.SetBool("isWalking", true);
    }

    public void UpdateState()
    {
        // 현재 위치랑 다음 순찰 위치 간의 거리 확인
        if (Vector3.Distance(walkPoints[curEnemyPos].transform.position, transform.position) < walkingPointRadius)
        {
            // 다음 방문 지점 랜덤으로 선택
            curEnemyPos = Random.Range(0, walkPoints.Length);

            // 배열 초과시 인덱스 초기화
            if (curEnemyPos >= walkPoints.Length)
            {
                curEnemyPos = 0;
            }
        }

        // 이동 명령
        enemyAgent.SetDestination(walkPoints[curEnemyPos].transform.position);

    }

    public void ExitState()
    {
        ani.SetBool("isWalking", false);
    }
}