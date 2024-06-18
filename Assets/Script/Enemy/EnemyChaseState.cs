using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyChaseState : MonoBehaviour, State
{
    private Animator ani;
    private NavMeshAgent enemyAgent;
    private Transform target;
    private Enemy enemy;

    public void EnterState()
    {
        if (ani == null || enemyAgent == null || enemy == null)
        {
            ani = GetComponent<Animator>();
            enemyAgent = GetComponent<NavMeshAgent>();
            enemy = GetComponent<Enemy>();
        }
                        
        ani.SetBool("isRunning", true);
    }

    public void UpdateState()
    {

        GameObject player = GameObject.FindWithTag("Player");
        GameObject vehicle = GameObject.FindWithTag("Ride");

        if (player != null)
        {
            target = player.transform; // 플레이어 설정
        }

        if (vehicle != null)
        {
            target = vehicle.transform; // 자동차 설정
        }

        if (target != null)
        {
            enemyAgent.SetDestination(target.position);
        }
    }

    public void ExitState()
    {
        ani.SetBool("isRunning", false);
    }
}
