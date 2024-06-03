using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum AI
{
    Patrol,
    Chase,
    Attack
}

public class EnemyAI
{
    public State CurState { get; private set; }

    private Enemy controller;
    private EnemyPatrolState patrol;
    private EnemyChaseState chase;
    private EnemyAttackState attack;

    public EnemyAI(Enemy _controller, EnemyPatrolState _patrol, EnemyChaseState _chase, EnemyAttackState _attack)
    {
        controller = _controller;
        patrol = _patrol;
        chase = _chase;
        attack = _attack;
    }

    public void Transition(State state)
    {
        if (CurState != null)
        {
            CurState.ExitState();
        }
        CurState = state;
        CurState.EnterState();
    }

    public void UpdateCurrentState()
    {
        if (CurState != null)
        {
            CurState.UpdateState();
        }
    }
}