using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] RandomSpawner spawner;
    [SerializeField] ItemSpawn itemSpawn;

    [Header("�� ����")]
    [SerializeField] private EnemyPatrolState patrol;
    [SerializeField] private EnemyChaseState chase;
    [SerializeField] private EnemyAttackState attack;

    private EnemyAI enemyAI;

    [SerializeField] private Transform fireTr; // �Ѿ� �߻� ��ġ
    [SerializeField] private float reloadT = 1f; // ������ �ð�
    private float timeCount = 0f;

    [SerializeField] private float visionRadius = 10f;
    [SerializeField] private float attackRadius = 2f;
    [SerializeField] private float maxHp = 10f;
    private float curHp;
    private GameObject player;

    private bool playerInVisionRadius;
    private bool playerInAttackRadius;

    private void Awake()
    {
        enemyAI = new EnemyAI(this, patrol, chase, attack);
        enemyAI.Transition(patrol);
    }

    private void Start()
    {
        curHp = maxHp;
    }

    private void Update()
    {
        player = GameObject.FindWithTag("Player");

        // �÷��̾ �þ߿� ���Դ���
        playerInVisionRadius = Physics.CheckSphere(transform.position, visionRadius, LayerMask.GetMask("Player"));

        // �÷��̾ ���� ���� �ȿ� �ִ���
        playerInAttackRadius = Physics.CheckSphere(transform.position, attackRadius, LayerMask.GetMask("Player"));

        if (playerInAttackRadius)
        {
            UpdateState(AI.Attack);
        }
        else if (playerInVisionRadius)
        {
            UpdateState(AI.Chase);
        }
        else
        {
            UpdateState(AI.Patrol);
        }

        enemyAI.UpdateCurrentState();

        timeCount += Time.deltaTime;
    }

    public void Attack()
    {
        if (timeCount < reloadT)
        {
            return;
        }
        Bullet newBullet;
        PoolManager.instance.bulletPool.GetObject(out newBullet);
        newBullet.transform.position = fireTr.transform.position;
        newBullet.transform.rotation = fireTr.rotation;
        timeCount = 0;
    }


    public void GetDamage(float damage)
    {
        curHp -= damage;
        if (curHp <= 0)
        {
            // ���� �ٽ� Ǯ�� ���� �ֱ� ���� �������� ����
            int itemIndex = Random.Range(0, itemSpawn.itemPrefabs.Length); // ���� �ε��� ����
            MonoBehaviour item = ItemSpawn.GetItem(itemIndex); // ���õ� �ε����� �������� Ǯ���� ������
            item.transform.position = transform.position; // �������� �� ��ġ�� ����

            spawner.objectPool.PutInPool(this); // ���� �ٽ� Ǯ�� ���� ����
            Debug.Log("�� ���");
        }
    }

    private void UpdateState(AI state)
    {
        switch (state)
        {
            case AI.Patrol:
                enemyAI.Transition(patrol);
                break;
            case AI.Chase:
                enemyAI.Transition(chase);
                break;
            case AI.Attack:
                enemyAI.Transition(attack);
                break;
        }
    }
}