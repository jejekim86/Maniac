using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCRandomSpawner : MonoBehaviour
{
    // ������Ʈ Ǯ ����
    public ObjectPool<MonoBehaviour> objectPool;

    // ���� ��ġ�� ���� ���� ����
    [SerializeField] private Vector3 spawnAreaMin;
    [SerializeField] private Vector3 spawnAreaMax;

    // �ǹ� ������Ʈ ����Ʈ
    [SerializeField] private List<GameObject> buildings;

    // ���� �ʿ� �����ϴ� NPC
    private MonoBehaviour currentNPC;

    // Start�� ��ũ��Ʈ ���� �� ù ������ ���� ȣ��˴ϴ�.
    void Start()
    {
        // ������Ʈ Ǯ �ʱ�ȭ
        objectPool.Initialize();

        // ù NPC ����
        SpawnRandomObject();
    }

    // ������Ʈ�� ���� ��ġ�� �����ϴ� �޼���
    public void SpawnRandomObject()
    {
        if (objectPool.GetObject(out MonoBehaviour item))
        {
            Vector3 randomPosition;

            // ���ǵ� ���� ������ ��ġ�� ����ȭ, ���� ������ ���ϵ��� ��
            do
            {
                randomPosition = new Vector3(
                    Random.Range(spawnAreaMin.x, spawnAreaMax.x),
                    Random.Range(spawnAreaMin.y, spawnAreaMax.y),
                    Random.Range(spawnAreaMin.z, spawnAreaMax.z)
                );
            } while (IsInsideAnyBuilding(randomPosition));

            // ������Ʈ�� ��ġ ����
            item.transform.position = randomPosition;

            // ������Ʈ Ȱ��ȭ
            item.gameObject.SetActive(true);

            // ���� NPC ���� ������Ʈ
            currentNPC = item;
        }
    }

    // �ǹ� ���� �ִ��� Ȯ���ϴ� �޼���
    bool IsInsideAnyBuilding(Vector3 position)
    {
        foreach (var building in buildings)
        {
            Collider collider = building.GetComponent<Collider>();
            if (collider != null && collider.bounds.Contains(position))
            {
                return true;
            }
        }
        return false;
    }

    // ���� ������ ������ �ð�ȭ
    void OnDrawGizmos()
    {
        // ����� ���� ����
        Gizmos.color = Color.black;

        // spawnAreaMin�� spawnAreaMax�� ����Ͽ� ���� �ڽ��� �׸�
        Vector3 spawnCenter = (spawnAreaMin + spawnAreaMax) * 0.5f;
        Vector3 spawnSize = spawnAreaMax - spawnAreaMin;
        Gizmos.DrawWireCube(spawnCenter, spawnSize);

        // ����� ���� ���� (�ǹ� ����)
        Gizmos.color = Color.green;

        // �ǹ��� �浹 ������ ������ �׸�
        foreach (var building in buildings)
        {
            Collider collider = building.GetComponent<Collider>();
            if (collider != null)
            {
                Gizmos.DrawWireCube(collider.bounds.center, collider.bounds.size);
            }
        }
    }
}
