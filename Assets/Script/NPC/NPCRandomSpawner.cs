using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCRandomSpawner : MonoBehaviour
{
    // 오브젝트 풀 참조
    public ObjectPool<MonoBehaviour> objectPool;

    // 랜덤 배치를 위한 영역 범위
    [SerializeField] private Vector3 spawnAreaMin;
    [SerializeField] private Vector3 spawnAreaMax;

    // 건물 오브젝트 리스트
    [SerializeField] private List<GameObject> buildings;

    // 현재 맵에 존재하는 NPC
    private MonoBehaviour currentNPC;

    // Start는 스크립트 실행 시 첫 프레임 전에 호출됩니다.
    void Start()
    {
        // 오브젝트 풀 초기화
        objectPool.Initialize();

        // 첫 NPC 생성
        SpawnRandomObject();
    }

    // 오브젝트를 랜덤 위치에 생성하는 메서드
    public void SpawnRandomObject()
    {
        if (objectPool.GetObject(out MonoBehaviour item))
        {
            Vector3 randomPosition;

            // 정의된 범위 내에서 위치를 랜덤화, 예외 영역을 피하도록 함
            do
            {
                randomPosition = new Vector3(
                    Random.Range(spawnAreaMin.x, spawnAreaMax.x),
                    Random.Range(spawnAreaMin.y, spawnAreaMax.y),
                    Random.Range(spawnAreaMin.z, spawnAreaMax.z)
                );
            } while (IsInsideAnyBuilding(randomPosition));

            // 오브젝트의 위치 설정
            item.transform.position = randomPosition;

            // 오브젝트 활성화
            item.gameObject.SetActive(true);

            // 현재 NPC 참조 업데이트
            currentNPC = item;
        }
    }

    // 건물 내에 있는지 확인하는 메서드
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

    // 생성 범위를 기즈모로 시각화
    void OnDrawGizmos()
    {
        // 기즈모 색상 설정
        Gizmos.color = Color.black;

        // spawnAreaMin과 spawnAreaMax를 사용하여 범위 박스를 그림
        Vector3 spawnCenter = (spawnAreaMin + spawnAreaMax) * 0.5f;
        Vector3 spawnSize = spawnAreaMax - spawnAreaMin;
        Gizmos.DrawWireCube(spawnCenter, spawnSize);

        // 기즈모 색상 설정 (건물 영역)
        Gizmos.color = Color.green;

        // 건물의 충돌 영역을 기즈모로 그림
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
