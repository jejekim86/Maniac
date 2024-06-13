using System.Collections;
using UnityEditor.Rendering.Universal;
using UnityEngine;
using UnityEngine.UI;


[RequireComponent(typeof(Rigidbody))]
public abstract class Controller : MonoBehaviour
{

    protected bool isride;
    protected float walkSpeed;
    protected float maxHp;
    [SerializeField] protected Image HP_image;
    protected MeshRenderer meshRenderer;
    protected new Rigidbody rigidbody;
    protected Vehicle vehicle;

    protected float curHp; // 현재 체력
    protected float RideTime = 3f;
    public abstract void Move();
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
            transform.position = Vector3.zero;
        }
        curHp -= damage;
        HP_image.fillAmount = curHp / maxHp;
    }
}