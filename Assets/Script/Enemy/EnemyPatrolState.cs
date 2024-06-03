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
        SetNextDestination();
    }

    public void UpdateState()
    {
        if (Vector3.Distance(walkPoints[curEnemyPos].transform.position, transform.position) < walkingPointRadius)
        {
            SetNextDestination();
        }
    }

    private void SetNextDestination()
    {
        curEnemyPos = (curEnemyPos + 1) % walkPoints.Length;
        enemyAgent.SetDestination(walkPoints[curEnemyPos].transform.position);
    }

    public void ExitState()
    {
        ani.SetBool("isWalking", false);
    }
}