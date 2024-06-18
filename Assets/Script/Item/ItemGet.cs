// Path: Assets/Scripts/ItemGet.cs
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameManager;

public class ItemGet : MonoBehaviour
{
    [SerializeField] private GameObject itemPrefab; // ������ ������
    [SerializeField] private GameObject itemGun;

    private int amount = 20; // ȹ�� ��差
    private float healAmount = 0.1f; // ü�� ȸ����

    public void ItemGet_Gun(GameObject target)
    {
        itemGun.SetActive(true);

        // �÷��̾��� ���� ������ ���� Ȯ��
        Transform currentWeaponTransform = target.transform.Find("Weapon");
        if (currentWeaponTransform != null)
        {
            // ���� ���Ⱑ ������ ���, ����
            Destroy(currentWeaponTransform.gameObject);
        }

        // Item �������� ���� ������Ʈ ��ġ�� ����. �⺻ ȸ�� ��(Quaternion.identity) ���
        GameObject newItem = Instantiate(itemPrefab, transform.position, Quaternion.identity);

        // ������ ������ ������Ʈ�� �÷��̾� �ڽ����� ����
        newItem.transform.SetParent(target.transform);
        newItem.name = "Weapon"; // ������ ������ �̸��� "Weapon"���� ����

        // ������ �������� ��ġ�� �÷��̾� ��ġ�� �������� (Vector3(-0.199000001,1.32799995,0.195999995))���� ����
        newItem.transform.localPosition = new Vector3(-0.199000001f, 1.32799995f, 0.195999995f);

        // ������ �������� �����̼��� Y�� �������� 90�� ȸ������ ����
        newItem.transform.localRotation = Quaternion.Euler(0, 90, 0);

        // �÷��̾� Controller�� �Ҵ�
        Player controller = target.GetComponent<Player>();
        if (controller != null)
        {
            controller.SetLongRangeWeapon(newItem.GetComponent<Weapon>());
        }
    }

    public void ItemGet_Money(GameObject target)
    {
        Player controller = target.GetComponent<Player>();
        if (controller != null)
        {
            controller.AddMoney(amount + (int)(amount * GameManager.Instance.inGameUpgradeData1[(int)InGameUpgrade.money] * 0.01f));
        }
    }

    public void ItemGet_HP(GameObject target)
    {
        // Vehicle ������Ʈ�� ���� �õ�
        Vehicle vehicle = target.GetComponent<Vehicle>();
        if (vehicle != null)
        {
            vehicle.AddHp(healAmount);
        }
        else
        {
            // Player ������Ʈ�� �õ�
            Player controller = target.GetComponent<Player>();
            if (controller != null)
            {
                controller.AddHp(healAmount);
            }
            else
            {
                Debug.LogError("ItemGet_HP: target�� Vehicle �Ǵ� Player ������Ʈ�� ����");
            }
        }
    }
}
