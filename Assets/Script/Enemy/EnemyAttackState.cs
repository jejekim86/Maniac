using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using UnityEngine.AI;

// ���� ���� ���¸� �����ϴ� Ŭ����
public class EnemyAttackState : MonoBehaviour, State
{
    // ������ ��ǥ
    [SerializeField] private Transform target;
    // ����� ����
    [SerializeField] private GameObject weapon;

    // �� ���� ���� ���
    [Header("�� ���� ����")]
    // ���� ��Ÿ��
    [SerializeField] private float coolTime;
    // ���� ������
    [SerializeField] private float damage = 5;
    // ���� ����
    [SerializeField] private float attackRange;
    // ���� ������ ����
    private bool isAttack;

    // �ִϸ����� ������Ʈ
    private Animator ani;
    // NavMeshAgent ������Ʈ
    private NavMeshAgent enemyAgent;
    // Enemy ��ũ��Ʈ
    private Enemy enemy;

    // ���� ���� �� ȣ��Ǵ� �޼���
    public void EnterState()
    {
        // �ִϸ�����, NavMeshAgent, Enemy ������Ʈ �ʱ�ȭ
        if (ani == null || enemyAgent == null || enemy == null)
        {
            ani = GetComponent<Animator>();
            enemyAgent = GetComponent<NavMeshAgent>();
            enemy = GetComponent<Enemy>();
        }

        // ���� �ִϸ��̼� ����
        ani.SetBool("isAttack", true);

        // EnemyAttack ������Ʈ�� �����ͼ� ���� ����
        EnemyAttack enemyAttack = GetComponent<EnemyAttack>();
        enemyAttack.Attack();

        // ���⸦ Ȱ��ȭ
        weapon.SetActive(true);
    }

    // ���� ������Ʈ �� ȣ��Ǵ� �޼���
    public void UpdateState()
    {
        // ���� ���� �ƴϸ� ���� ����
        if (!isAttack)
        {
            isAttack = true;

            // ���� �� �÷��̾ �ٶ󺸰� ��
            transform.LookAt(target);

            // ���� �� ��ġ ����
            enemyAgent.SetDestination(transform.position);

            // ���� �ڷ�ƾ ����
            StartCoroutine(Attack());
        }
    }

    // ������ �����ϴ� �ڷ�ƾ
    private IEnumerator Attack()
    {
        // ��Ÿ�� ���� ���
        yield return new WaitForSeconds(coolTime);
        // ������ �������� ǥ��
        isAttack = false;
    }

    // ���� ���� �� ȣ��Ǵ� �޼���
    public void ExitState()
    {
        // ���� �ִϸ��̼� ����
        ani.SetBool("isAttack", false);
        // ���⸦ ��Ȱ��ȭ
        weapon.SetActive(false);
    }
}