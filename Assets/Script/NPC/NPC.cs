
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : MonoBehaviour
{
    [SerializeField] private NPCRandomSpawner spawner;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // 상점 코드
            Debug.Log("상점");

            // NPC를 다시 풀에 집어넣음
            spawner.objectPool.PutInPool(this);

            // 새로운 NPC 생성 요청
            spawner.SpawnRandomObject();
        }
    }
}