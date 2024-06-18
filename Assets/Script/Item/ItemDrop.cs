using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDrop : MonoBehaviour
{
    [SerializeField] private GameObject itemPrefab; // æ∆¿Ã≈€ «¡∏Æ∆’
    [SerializeField] private string itemType;
    [SerializeField] private float rotationSpeed;

    float y;

    void Update()
    {
        if (CompareTag("Item"))
        {
            y += Time.deltaTime * rotationSpeed;
            itemPrefab.transform.rotation = Quaternion.Euler(0, y, 0);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log($"{itemType} »πµÊ");
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

            Destroy(gameObject);
        }
    }
}