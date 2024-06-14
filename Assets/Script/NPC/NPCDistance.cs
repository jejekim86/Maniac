using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NPCDistance : MonoBehaviour
{
    [SerializeField] private GameObject player;  // �÷��̾� ������Ʈ
    [SerializeField] private GameObject npc;     // NPC ������Ʈ
    [SerializeField] private Text distanceText;  // �Ÿ� �ؽ�Ʈ UI
    [SerializeField] private Image arrowImage;   // ȭ��ǥ �̹���
    [SerializeField] private float lerpSpeed = 5f; // Lerp �ӵ�
    [SerializeField] private float screenEdgeBuffer = 50f; // ��ũ�� �����ڸ� ����

    private Camera mainCamera; // ���� ī�޶� ĳ��

    void Start()
    {
        mainCamera = Camera.main;
        npc = GameObject.FindWithTag("NPC"); // �±װ� "NPC"�� ���� ������Ʈ�� ã��
    }

    void Update()
    {
        if (npc != null)
        {
            // �÷��̾�� NPC ������ �Ÿ� ���
            float distance = Vector3.Distance(player.transform.position, npc.transform.position);
            // NPC�� ȭ�� ��ġ ���
            Vector3 npcScreenPos = mainCamera.WorldToScreenPoint(npc.transform.position);

            // ��ũ�� �߽ɿ��� NPC���� ���� ���
            Vector3 screenCenter = new Vector3(Screen.width / 2, Screen.height / 2, 0);
            Vector3 direction = (npcScreenPos - screenCenter).normalized;

            // ��ũ�� �߽ɿ��� ���� �Ÿ� ������ ��ġ�� �ؽ�Ʈ ��ġ ����
            Vector3 targetPosition = screenCenter + direction * (Mathf.Min(Vector3.Distance(screenCenter, npcScreenPos), Screen.width / 2 - screenEdgeBuffer));

            // Lerp�� ����Ͽ� �ؽ�Ʈ�� ȭ��ǥ ��ġ�� �ε巴�� ������Ʈ
            distanceText.transform.position = Vector3.Lerp(distanceText.transform.position, targetPosition, Time.deltaTime * lerpSpeed);
            arrowImage.transform.position = Vector3.Lerp(arrowImage.transform.position, targetPosition, Time.deltaTime * lerpSpeed);

            // ȭ��ǥ�� NPC�� ����Ű���� ȸ��
            Vector3 toTarget = npcScreenPos - screenCenter;
            float angle = Mathf.Atan2(toTarget.y, toTarget.x) * Mathf.Rad2Deg;
            arrowImage.transform.rotation = Quaternion.Lerp(arrowImage.transform.rotation, Quaternion.Euler(0, 0, angle), Time.deltaTime * lerpSpeed);

            // �Ÿ��� �ؽ�Ʈ�� ǥ��
            distanceText.text = $"{distance:F1}\nLEVEL UP"; // F1�� �Ҽ��� ù° �ڸ����� ǥ��
        }
    }
}
