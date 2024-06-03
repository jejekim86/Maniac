using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemMagnetic : MonoBehaviour
{
    [SerializeField] private Transform target;

    [SerializeField] private float moveSpeed = 1.0f;
    [SerializeField] private float range = 5f;

    private void Update()
    {
        // 플레이어와 현재 아이템 위치의 상대적인 거리 계산
        Vector3 relativePos = target.position - transform.position;

        // 플레이어로 향하는 각도 계산
        float angle = Mathf.Atan2(relativePos.y, relativePos.x) * Mathf.Rad2Deg;

        // 플레이어와의 거리가 일정 범위 내에 있을 때만 이동
        if (relativePos.magnitude <= range)
        {
            // 플레이어에게 부드럽게 이동
            transform.position = Vector3.Lerp(transform.position, target.position, moveSpeed * Time.deltaTime);
        }
    }
}