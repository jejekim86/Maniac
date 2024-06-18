// Path: Assets/Scripts/ItemGet.cs
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameManager;

public class ItemGet : MonoBehaviour
{
    [SerializeField] private GameObject itemPrefab; // 아이템 프리팹
    [SerializeField] private GameObject itemGun;

    private int amount = 20; // 획득 골드량
    private float healAmount = 0.1f; // 체력 회복량

    public void ItemGet_Gun(GameObject target)
    {
        itemGun.SetActive(true);

        // 플레이어의 현재 장착된 무기 확인
        Transform currentWeaponTransform = target.transform.Find("Weapon");
        if (currentWeaponTransform != null)
        {
            // 현재 무기가 장착된 경우, 제거
            Destroy(currentWeaponTransform.gameObject);
        }

        // Item 프리팹을 현재 오브젝트 위치에 생성. 기본 회전 값(Quaternion.identity) 사용
        GameObject newItem = Instantiate(itemPrefab, transform.position, Quaternion.identity);

        // 생성된 아이템 오브젝트를 플레이어 자식으로 설정
        newItem.transform.SetParent(target.transform);
        newItem.name = "Weapon"; // 장착된 무기의 이름을 "Weapon"으로 설정

        // 생성된 아이템의 위치를 플레이어 위치를 기준으로 (Vector3(-0.199000001,1.32799995,0.195999995))으로 설정
        newItem.transform.localPosition = new Vector3(-0.199000001f, 1.32799995f, 0.195999995f);

        // 생성된 아이템의 로테이션을 Y축 기준으로 90도 회전으로 설정
        newItem.transform.localRotation = Quaternion.Euler(0, 90, 0);

        // 플레이어 Controller에 할당
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
        // Vehicle 컴포넌트를 먼저 시도
        Vehicle vehicle = target.GetComponent<Vehicle>();
        if (vehicle != null)
        {
            vehicle.AddHp(healAmount);
        }
        else
        {
            // Player 컴포넌트를 시도
            Player controller = target.GetComponent<Player>();
            if (controller != null)
            {
                controller.AddHp(healAmount);
            }
            else
            {
                Debug.LogError("ItemGet_HP: target에 Vehicle 또는 Player 컴포넌트가 없음");
            }
        }
    }
}
