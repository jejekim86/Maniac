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
        // 씬에 있는 모든 아이템을 태그로 찾기
        GameObject[] items = GameObject.FindGameObjectsWithTag("Item");

        if (items.Length == 0)
        {
            return;
        }

        // 가장 가까운 아이템 찾기
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

        // 거리와 아이템 이름을 텍스트로 표시
        if (closestItem != null)
        {
            distanceText.text = $"{closestItem.name} \n {closestDistance:F1}";
        }
    }
}
