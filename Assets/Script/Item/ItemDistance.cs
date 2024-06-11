using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemDistance : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private Text distanceText;

    void Update()
    {
        // ���� �ִ� ��� �������� �±׷� ã��
        GameObject[] items = GameObject.FindGameObjectsWithTag("Item");

        if (items.Length == 0)
        {
            return;
        }

        // ���� ����� ������ ã��
        GameObject closestItem = null;
        float closestDistance = Mathf.Infinity;

        foreach (GameObject item in items)
        {
            float distance = Vector3.Distance(player.transform.position, item.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestItem = item;
            }
        }

        // �Ÿ��� ������ �̸��� �ؽ�Ʈ�� ǥ��
        if (closestItem != null)
        {
            distanceText.text = $"{closestItem.name} \n {closestDistance:F1}";
        }
    }
}
