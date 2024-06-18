using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

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
    private GameObject vehicle;

    private bool playerInVisionRadius;
    private bool playerInAttackRadius;
    private bool vehicleInVisionRadius;
    private bool vehicleInAttackRadius;

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
        vehicle = GameObject.FindWithTag("Ride");

        // �÷��̾ �þ߿� ���Դ���
        playerInVisionRadius = Physics.CheckSphere(transform.position, visionRadius, LayerMask.GetMask("Player"));
        // �ڵ����� �þ߿� ���Դ���
        vehicleInVisionRadius = Physics.CheckSphere(transform.position, visionRadius, LayerMask.GetMask("Vehicle"));

        // �÷��̾ ���� ���� �ȿ� �ִ���
        playerInAttackRadius = Physics.CheckSphere(transform.position, attackRadius, LayerMask.GetMask("Player"));

        // �ڵ����� ���� ���� �ȿ� �ִ���
        vehicleInAttackRadius = Physics.CheckSphere(transform.position, attackRadius, LayerMask.GetMask("Vehicle"));

        HandleStateTransitions();

        enemyAI.UpdateCurrentState();

        timeCount += Time.deltaTime;
    }

    private void HandleStateTransitions()
    {
        if (playerInAttackRadius || vehicleInAttackRadius)
        {
            enemyAI.Transition(attack);
        }
        else if (playerInVisionRadius || vehicleInVisionRadius)
        {
            enemyAI.Transition(chase);
        }
        else
        {
            enemyAI.Transition(patrol);
        }
    }

    public void Attack()
    {
        if (timeCount < reloadT)
        {
            return;
        }

        if (fireTr != null) // fireTr�� null���� Ȯ��
        {
            Bullet newBullet;
            PoolManager.instance.bulletPool.GetObject(out newBullet);
            newBullet.SetDirection(fireTr);
        }
        timeCount = 0;
    }

    public void GetDamage(float damage)
    {
        curHp -= damage;
        if (curHp <= 0)
        {
            SpawnItem();
            spawner.objectPool.PutInPool(this); // ���� �ٽ� Ǯ�� ���� ����
            Debug.Log("�� ���");
        }
    }

    private void SpawnItem()
    {
        int itemIndex = Random.Range(0, itemSpawn.itemPrefabs.Length); // ���� �ε��� ����
        MonoBehaviour item = ItemSpawn.GetItem(itemIndex); // ���õ� �ε����� �������� Ǯ���� ������
        item.transform.position = transform.position; // �������� �� ��ġ�� ����
    }
}