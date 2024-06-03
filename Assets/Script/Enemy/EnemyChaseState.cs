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

        target = GameObject.FindWithTag("Player").transform;
        ani.SetBool("isRunning", true);
    }

    public void UpdateState()
    {
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