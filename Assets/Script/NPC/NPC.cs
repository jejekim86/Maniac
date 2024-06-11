
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
            // ���� �ڵ�
            Debug.Log("����");

            // NPC�� �ٽ� Ǯ�� �������
            spawner.objectPool.PutInPool(this);

            // ���ο� NPC ���� ��û
            spawner.SpawnRandomObject();
        }
    }
}