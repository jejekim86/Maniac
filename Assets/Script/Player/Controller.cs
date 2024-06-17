using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody))]

public abstract class Controller : MonoBehaviour
{
    protected float walkSpeed;
    protected float maxHp; 
    [SerializeField] protected Image HP_image;
    public abstract void Move();

    protected new Rigidbody rigidbody;

    protected float curHp; // 현재 체력


    public virtual void AddHp(float heal)
    {
        curHp += heal;
        if (curHp > maxHp)
            curHp = maxHp;

        HP_image.fillAmount = curHp / maxHp;
    }

    public virtual void GetDamage(float damage)
    {
        if (curHp <= 0)
        {
            Debug.Log("사망함");
            // 여기서 게임 오버 결과창 표시
            transform.position = Vector3.zero;
        }
        curHp -= damage;
        HP_image.fillAmount = curHp / maxHp;
    }
}
