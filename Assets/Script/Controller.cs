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

    public virtual void Dead()
    {
        Debug.Log("사망함");
        transform.position = Vector3.zero;
    }

    public virtual void GetDamage(float damage)
    {
        if (curHp <= 0)
        {
            Dead();
        }
        curHp -= damage;
        HP_image.fillAmount = curHp / maxHp;
    }
}
