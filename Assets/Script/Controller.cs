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
    public abstract void Move();
    protected MeshRenderer meshRenderer;

    protected new Rigidbody rigidbody;

    protected float curHp; // 현재 체력
    protected float RideTime = 3f;
    public virtual void AddHp(float heal)
    {
        curHp += heal;
        if (curHp > maxHp)
            curHp = maxHp;

        HP_image.fillAmount = curHp / maxHp;
    }
    protected virtual IEnumerator PushRideButton(Controller item)
    {
        switch (item is Vehicle)
        {
            case true: // 차에서 내릴때
                yield return null;
                transform.SetParent(null);
                transform.position = item.transform.position + (Vector3.right * 3);
                break;
            case false: // 차에서 탑승할때
                Debug.Log("ClickButton");
                yield return new WaitForSeconds(RideTime);
                item.gameObject.SetActive(false);
                break;
        }
        yield break;
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