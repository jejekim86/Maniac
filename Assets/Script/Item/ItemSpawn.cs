using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpawn : MonoBehaviour
{
    public static ItemSpawn Instance; // �� Ŭ������ �ν��Ͻ��� �����ϴ� ���� ����
    [SerializeField] public GameObject[] itemPrefabs; // ������ ������ ������ �迭
    private ObjectPool<MonoBehaviour>[] itemPools; // �� ������ �����տ� ���� Ǯ�� �����ϴ� ����Ʈ �迭

    private void Awake()
    {
        Instance = this; // �ν��Ͻ��� ���� ��ü�� ����
        itemPools = new ObjectPool<MonoBehaviour>[itemPrefabs.Length]; // ������ Ǯ ����Ʈ �迭�� �ʱ�ȭ

        for (int i = 0; i < itemPrefabs.Length; i++) // �� ������ �����տ� ����
        {
            itemPools[i] = new ObjectPool<MonoBehaviour>(); // ������ Ǯ ����
            itemPools[i].Initialize(itemPrefabs[i].GetComponent<MonoBehaviour>()); // ������ Ǯ �ʱ�ȭ
        }
    }

    public static MonoBehaviour GetItem(int index)
    {
        if (Instance.itemPools[index].GetObject(out MonoBehaviour item)) // �������� Ǯ���� ������
        {
            item.gameObject.SetActive(true); // ������ Ȱ��ȭ
            return item; // ������ ��ȯ
        }

        return null; // �������� �������� ���� ��� null ��ȯ
    }

    public static void ReturnItem(MonoBehaviour item, int index)
    {
        Instance.itemPools[index].PutInPool(item); // �������� Ǯ�� ��ȯ�մϴ�.
    }
}
