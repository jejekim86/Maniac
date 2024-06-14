using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NPCDistance : MonoBehaviour
{
    [SerializeField] private GameObject player;  // 플레이어 오브젝트
    [SerializeField] private GameObject npc;     // NPC 오브젝트
    [SerializeField] private Text distanceText;  // 거리 텍스트 UI
    [SerializeField] private Image arrowImage;   // 화살표 이미지
    [SerializeField] private float lerpSpeed = 5f; // Lerp 속도
    [SerializeField] private float screenEdgeBuffer = 50f; // 스크린 가장자리 버퍼

    private Camera mainCamera; // 메인 카메라 캐싱

    void Start()
    {
        mainCamera = Camera.main;
        npc = GameObject.FindWithTag("NPC"); // 태그가 "NPC"인 게임 오브젝트를 찾음
    }

    void Update()
    {
        if (npc != null)
        {
            // 플레이어와 NPC 사이의 거리 계산
            float distance = Vector3.Distance(player.transform.position, npc.transform.position);
            // NPC의 화면 위치 계산
            Vector3 npcScreenPos = mainCamera.WorldToScreenPoint(npc.transform.position);

            // 스크린 중심에서 NPC로의 방향 계산
            Vector3 screenCenter = new Vector3(Screen.width / 2, Screen.height / 2, 0);
            Vector3 direction = (npcScreenPos - screenCenter).normalized;

            // 스크린 중심에서 일정 거리 떨어진 위치로 텍스트 위치 설정
            Vector3 targetPosition = screenCenter + direction * (Mathf.Min(Vector3.Distance(screenCenter, npcScreenPos), Screen.width / 2 - screenEdgeBuffer));

            // Lerp를 사용하여 텍스트와 화살표 위치를 부드럽게 업데이트
            distanceText.transform.position = Vector3.Lerp(distanceText.transform.position, targetPosition, Time.deltaTime * lerpSpeed);
            arrowImage.transform.position = Vector3.Lerp(arrowImage.transform.position, targetPosition, Time.deltaTime * lerpSpeed);

            // 화살표가 NPC를 가리키도록 회전
            Vector3 toTarget = npcScreenPos - screenCenter;
            float angle = Mathf.Atan2(toTarget.y, toTarget.x) * Mathf.Rad2Deg;
            arrowImage.transform.rotation = Quaternion.Lerp(arrowImage.transform.rotation, Quaternion.Euler(0, 0, angle), Time.deltaTime * lerpSpeed);

            // 거리를 텍스트로 표시
            distanceText.text = $"{distance:F1}\nLEVEL UP"; // F1은 소수점 첫째 자리까지 표시
        }
    }
}
