using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody))]

public class Controller : MonoBehaviour
{
    public float walkSpeed, walkAnimationSpeed, dashPower;

    public Animator animator;
    public new Rigidbody rigidbody;

    [SerializeField] Weapon longRangeWeapon;
    [SerializeField] Weapon meleeWeapon;
    [SerializeField] Text moneyText;
    [SerializeField] Image HP_image;
    [SerializeField] private float maxHp = 10f; // 최대 체력

    private int money; // 소유 돈
    private float curHp; // 현재 체력

    Vector3 translation;
    bool canDash;

    void Start()
    {
        canDash = true;
        rigidbody = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        money = 1000; // 초기 돈
        curHp = 0.1f; // 초기 체력

        //StartCoroutine(LongRangeWeapon());
    }

    void Update()
    {
        // 이동
        float horizontalMove = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        float speed = walkSpeed;
        float animSpeed = walkAnimationSpeed;


        translation = Vector3.forward * (vertical * Time.deltaTime);
        translation += Vector3.right * (horizontalMove * Time.deltaTime);
        translation *= speed;
        transform.Translate(translation, Space.World);

        // 대쉬
        if (Input.GetKey(KeyCode.LeftShift) && canDash)
        {
            canDash = false;
            StartCoroutine(Dash());
        }

        // 애니메이션
        animator.SetFloat("Vertical", vertical, 0.1f, Time.deltaTime);
        animator.SetFloat("Horizontal", horizontalMove, 0.1f, Time.deltaTime);
        animator.SetFloat("WalkSpeed", animSpeed);
        
        // 회전
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        Physics.Raycast(ray, out hit);
        transform.LookAt(new Vector3(hit.point.x, transform.position.y, hit.point.z));

        //근접 공격
        if (Input.GetMouseButton(1))
        {
            if(meleeWeapon.Attack())
                animator.SetTrigger("MeleeAttack");
        }
        //원거리 공격
        if (Input.GetMouseButton(0))
        {
            longRangeWeapon.Attack();
        }
    }

    IEnumerator Dash()
    {
        rigidbody.AddForce(translation * dashPower, ForceMode.Impulse);
        yield return new WaitForSeconds(0.5f); // 대쉬 지속 시간
        rigidbody.velocity = Vector3.zero;
        canDash = true;
    }

    public void SetLongRangeWeapon(Weapon weapon)
    {
        longRangeWeapon = weapon;
    }

    public void SetMeleeWeapon(Weapon weapon)
    {
        meleeWeapon = weapon;
    }

    public void AddMoney(int amount)
    {
        money += amount;
        moneyText.text = money.ToString();
    }

    public void AddHp(float heal)
    {
        curHp += heal;
        if (curHp > maxHp)
        {
            curHp = maxHp;

        }
            HP_image.fillAmount = curHp / maxHp;
    }
}
