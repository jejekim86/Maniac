using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

// ���� ���� ���¸� �����ϴ� Ŭ����
public class EnemyAttackState : MonoBehaviour, State
{
    [SerializeField] private Transform target; // ������ ��ǥ
    [SerializeField] private GameObject weapon; // ����� ����

    [Header("�� ���� ����")]
    [SerializeField] private float coolTime; // ���� ��Ÿ��
    [SerializeField] private float damage = 5; // ���� ������
    [SerializeField] private float attackRange; // ���� ����
    private bool isAttack;

    private Animator ani; // �ִϸ����� ������Ʈ
    private NavMeshAgent enemyAgent; // NavMeshAgent ������Ʈ
    private Enemy enemy; // Enemy ��ũ��Ʈ

    public void EnterState()
    {
        if (ani == null || enemyAgent == null || enemy == null)
        {
            ani = GetComponent<Animator>();
            enemyAgent = GetComponent<NavMeshAgent>();
            enemy = GetComponent<Enemy>();
        }

        ani.SetBool("isAttack", true); // ���� �ִϸ��̼� ����
        enemy.Attack();
        weapon.SetActive(true); // ���� Ȱ��ȭ
    }

    public void UpdateState()
    {
        // ���� ���� �ƴϸ� ���� ����
        if (!isAttack)
        {
            isAttack = true;
            StartCoroutine(Attack());
        }

        // Ÿ���� �����ϸ� Ÿ���� õõ�� �ٶ󺸵��� ȸ��
        if (target != null)
        {
            Vector3 direction = (target.position - transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Lerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
        }

        // ���� �� ��ġ ����
        enemyAgent.SetDestination(transform.position);
    }

    private IEnumerator Attack()
    {
        enemy.Attack();
        yield return new WaitForSeconds(coolTime); // ��Ÿ�� ���� ���
        isAttack = false; // ������ �������� ǥ��
    }

    public void ExitState()
    {
        ani.SetBool("isAttack", false); // ���� �ִϸ��̼� ����
        weapon.SetActive(false); // ���� ��Ȱ��ȭ
    }
}
