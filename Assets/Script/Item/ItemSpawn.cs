using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpawn : MonoBehaviour
{
    public static ItemSpawn Instance; // 이 클래스의 인스턴스를 저장하는 정적 변수
    [SerializeField] public GameObject[] itemPrefabs; // 생성할 아이템 프리팹 배열
    private ObjectPool<MonoBehaviour>[] itemPools; // 각 아이템 프리팹에 대한 풀을 저장하는 리스트 배열

    private void Awake()
    {
        Instance = this; // 인스턴스를 현재 객체로 설정
        itemPools = new ObjectPool<MonoBehaviour>[itemPrefabs.Length]; // 아이템 풀 리스트 배열을 초기화

        for (int i = 0; i < itemPrefabs.Length; i++) // 각 아이템 프리팹에 대해
        {
            itemPools[i] = new ObjectPool<MonoBehaviour>(); // 아이템 풀 생성
            itemPools[i].Initialize(itemPrefabs[i].GetComponent<MonoBehaviour>()); // 아이템 풀 초기화
        }
    }

    public static MonoBehaviour GetItem(int index)
    {
        if (Instance.itemPools[index].GetObject(out MonoBehaviour item)) // 아이템을 풀에서 가져옴
        {
            item.gameObject.SetActive(true); // 아이템 활성화
            return item; // 아이템 반환
        }

        return null; // 아이템을 가져오지 못한 경우 null 반환
    }

    public static void ReturnItem(MonoBehaviour item, int index)
    {
        Instance.itemPools[index].PutInPool(item); // 아이템을 풀에 반환합니다.
    }
}
