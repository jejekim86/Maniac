// Path: Assets/Scripts/ItemDrop.cs
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDrop : MonoBehaviour
{
    [SerializeField] private GameObject itemPrefab; // 아이템 프리팹
    [SerializeField] private string itemType; // 아이템 종류
    [SerializeField] private float rotationSpeed; // 회전 속도

    float y;

    void Update()
    {
        // "Item" 태그가 붙어 있는 경우 아이템을 회전시킴
        if (CompareTag("Item"))
        {
            y += Time.deltaTime * rotationSpeed;
            itemPrefab.transform.rotation = Quaternion.Euler(0, y, 0);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"충돌 발생: {other.gameObject.name}와 {itemType} 아이템"); // 충돌 로그 추가

        // 플레이어와 충돌하면 아이템을 획득
        if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log($"{itemType} 획득");

            ItemGet itemGet = other.GetComponent<ItemGet>();
            if (itemGet != null)
            {
                if (itemType == "Gun")
                {
                    itemGet.ItemGet_Gun(other.gameObject);
                }
                else if (itemType == "Money")
                {
                    itemGet.ItemGet_Money(other.gameObject);
                }
                else if (itemType == "HP")
                {
                    itemGet.ItemGet_HP(other.gameObject);
                }
            }
            else
            {
                Debug.LogError("Player 객체에 ItemGet 컴포넌트가 없음");
            }

            Destroy(gameObject);
        }
        // 차량과 충돌하면 아이템을 획득
        else if (other.gameObject.CompareTag("Ride"))
        {
            Debug.Log($"{itemType} 획득");

            ItemGet itemGet = other.GetComponent<ItemGet>();
            if (itemGet != null)
            {
                if (itemType == "Gun")
                {
                    itemGet.ItemGet_Gun(other.gameObject);
                }
                else if (itemType == "Money")
                {
                    itemGet.ItemGet_Money(other.gameObject);
                }
                else if (itemType == "HP")
                {
                    itemGet.ItemGet_HP(other.gameObject);
                }
            }
            else
            {
                Debug.LogError("Vehicle 객체에 ItemGet 컴포넌트가 없음");
            }

            Destroy(gameObject);
        }
    }
}
