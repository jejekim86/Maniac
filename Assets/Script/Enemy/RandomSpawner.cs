using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomSpawner : MonoBehaviour
{
    // 오브젝트 풀 참조
    public ObjectPool<MonoBehaviour> objectPool;

    // 랜덤 배치를 위한 영역 범위
    [SerializeField] private Vector3 spawnAreaMin;
    [SerializeField] private Vector3 spawnAreaMax;

    // 생성 간격
    [SerializeField] private float spawnInterval = 2f;

    // Start는 스크립트 실행 시 첫 프레임 전에 호출됩니다.
    void Start()
    {
        // 오브젝트 풀 초기화
        objectPool.Initialize();

        // 주기적으로 오브젝트를 생성하도록 설정
        InvokeRepeating(nameof(SpawnRandomObject), 0f, spawnInterval);
    }

    // 오브젝트를 랜덤 위치에 생성하는 메서드
    void SpawnRandomObject()
    {
        if (objectPool.GetObject(out MonoBehaviour item))
        {
            // 정의된 범위 내에서 위치를 랜덤화
            Vector3 randomPosition = new Vector3(
                Random.Range(spawnAreaMin.x, spawnAreaMax.x),
                Random.Range(spawnAreaMin.y, spawnAreaMax.y),
                Random.Range(spawnAreaMin.z, spawnAreaMax.z)
            );

            // 오브젝트의 위치 설정
            item.transform.position = randomPosition;

            // 오브젝트 활성화
            item.gameObject.SetActive(true);
        }
    }

    // 생성 범위를 기즈모로 시각화
    void OnDrawGizmos()
    {
        // 기즈모 색상 설정
        Gizmos.color = Color.red;

        // spawnAreaMin과 spawnAreaMax를 사용하여 범위 박스를 그림
        Vector3 center = (spawnAreaMin + spawnAreaMax) * 0.5f;
        Vector3 size = spawnAreaMax - spawnAreaMin;
        Gizmos.DrawWireCube(center, size);
    }
}
