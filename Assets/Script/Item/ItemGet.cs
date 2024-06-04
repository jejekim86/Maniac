using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemGet : MonoBehaviour
{
    [SerializeField] private GameObject itemPrefab; // 아이템 프리팹
    private int amount = 20; // 획득 골드량
    private int healAmount = 1; // 체력 회복량

    public void ItemGet_Gun(GameObject target)
    {        
        // Item 프리팹을 현재 오브젝트 위치에 생성. 기본 회전 값(Quaternion.identity) 사용
        GameObject newItem = Instantiate(itemPrefab, transform.position, Quaternion.identity);

        // 생성된 아이템 오브젝트를 플레이어 자식으로 설정
        newItem.transform.SetParent(gameObject.transform);

        // 생성된 아이템의 위치를 플레이어 위치를 기준으로 (Vector3(-0.199000001,1.32799995,0.195999995))으로 설정
        newItem.transform.localPosition = new Vector3(-0.199000001f, 1.32799995f, 0.195999995f);

        // 생성된 아이템의 로테이션을 Y축 기준으로 90도 회전으로 설정
        newItem.transform.localRotation = Quaternion.Euler(0, 90, 0);

        // 플레이어 Controller에 할당
        Player controller = target.GetComponent<Player>();
        if (controller != null )
        {
            controller.SetLongRangeWeapon(newItem.GetComponent<Weapon>());
        }
    }

    public void ItemGet_Money(GameObject target)
    {
        Player controller = target.GetComponent<Player>();
        if (controller != null)
        {
            controller.AddMoney(amount);
        }
    }

    public void ItemGet_HP(GameObject target)
    {
        Player controller = target.GetComponent<Player>();
        if (controller != null)
        {
            controller.AddHp(healAmount);
        }
    }
}