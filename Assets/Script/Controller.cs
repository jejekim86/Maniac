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

    [SerializeField] private float itemMoveSpeed = 1.0f; // 아이템 이동 속도
    [SerializeField] private float itemRange = 5f; // 아이템 끌어당기는 범위

    private int money; // 소유 돈
    private float curHp; // 현재 체력

    Vector3 translation;

    bool canDash;
    void Start()
    {
        canDash = true;
        rigidbody = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
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

        AttractItems();
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

    private void AttractItems()
    {
        // "Item" 태그를 가진 모든 게임 오브젝트를 찾음
        GameObject[] items = GameObject.FindGameObjectsWithTag("Item");

        foreach (GameObject item in items)
        {
            // 플레이어와 아이템 사이의 상대적인 거리 계산
            Vector3 relativePos = item.transform.position - transform.position;

            // 플레이어와의 거리가 일정 범위 내에 있을 때만 이동
            if (relativePos.magnitude <= itemRange)
            {
                // 아이템을 플레이어에게 부드럽게 이동
                item.transform.position = Vector3.Lerp(item.transform.position, transform.position, itemMoveSpeed * Time.deltaTime);
            }
        }
    }
}
