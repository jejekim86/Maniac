using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody))]

public abstract class Controller : MonoBehaviour
{
    protected float walkSpeed;
    protected int maxHp; 
    public abstract void Move();
    [SerializeField] protected Image HP_image;
    protected MeshRenderer meshRenderer;

    protected new Rigidbody rigidbody;

    protected int curHp; // 현재 체력

    public virtual void AddHp(int heal)
    {
        curHp += heal;
        if (curHp > maxHp)
            curHp = maxHp;

        HP_image.fillAmount = curHp / maxHp;
    }
}
