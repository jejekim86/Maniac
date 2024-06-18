using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    [SerializeField] RandomSpawner spawner;
    [SerializeField] ItemSpawn itemSpawn;

    [Header("적 상태")]
    [SerializeField] private EnemyPatrolState patrol;
    [SerializeField] private EnemyChaseState chase;
    [SerializeField] private EnemyAttackState attack;

    private EnemyAI enemyAI;

    [SerializeField] private Transform fireTr; // 총알 발사 위치
    [SerializeField] private float reloadT = 1f; // 재장전 시간
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

        // 플레이어가 시야에 들어왔는지
        playerInVisionRadius = Physics.CheckSphere(transform.position, visionRadius, LayerMask.GetMask("Player"));
        // 자동차가 시야에 들어왔는지
        vehicleInVisionRadius = Physics.CheckSphere(transform.position, visionRadius, LayerMask.GetMask("Vehicle"));

        // 플레이어가 공격 범위 안에 있는지
        playerInAttackRadius = Physics.CheckSphere(transform.position, attackRadius, LayerMask.GetMask("Player"));

        // 자동차가 공격 범위 안에 있는지
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

        if (fireTr != null) // fireTr이 null인지 확인
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
            spawner.objectPool.PutInPool(this); // 적을 다시 풀에 집어 넣음
            Debug.Log("적 사망");
        }
    }

    private void SpawnItem()
    {
        int itemIndex = Random.Range(0, itemSpawn.itemPrefabs.Length); // 랜덤 인덱스 선택
        MonoBehaviour item = ItemSpawn.GetItem(itemIndex); // 선택된 인덱스의 아이템을 풀에서 가져옴
        item.transform.position = transform.position; // 아이템을 적 위치에 생성
    }
}