// Path: Assets/Scripts/ItemDrop.cs
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDrop : MonoBehaviour
{
    [SerializeField] private GameObject itemPrefab; // ������ ������
    [SerializeField] private string itemType; // ������ ����
    [SerializeField] private float rotationSpeed; // ȸ�� �ӵ�

    float y;

    void Update()
    {
        // "Item" �±װ� �پ� �ִ� ��� �������� ȸ����Ŵ
        if (CompareTag("Item"))
        {
            y += Time.deltaTime * rotationSpeed;
            itemPrefab.transform.rotation = Quaternion.Euler(0, y, 0);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"�浹 �߻�: {other.gameObject.name}�� {itemType} ������"); // �浹 �α� �߰�

        // �÷��̾�� �浹�ϸ� �������� ȹ��
        if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log($"{itemType} ȹ��");

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
                Debug.LogError("Player ��ü�� ItemGet ������Ʈ�� ����");
            }

            Destroy(gameObject);
        }
        // ������ �浹�ϸ� �������� ȹ��
        else if (other.gameObject.CompareTag("Ride"))
        {
            Debug.Log($"{itemType} ȹ��");

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
                Debug.LogError("Vehicle ��ü�� ItemGet ������Ʈ�� ����");
            }

            Destroy(gameObject);
        }
    }
}
