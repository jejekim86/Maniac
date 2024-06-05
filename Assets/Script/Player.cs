using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(MeshRenderer))]
public class Player : Controller
{
    Vehicle vehicle;
    Vector3 translation;
    Vector3 lastMoveDirection = Vector3.forward; // 마지막 이동 방향을 저장할 변수

    [SerializeField] Weapon longRangeWeapon;
    [SerializeField] Weapon meleeWeapon;
    [SerializeField] Text moneyText;
    [SerializeField] Image playerimage;
    [SerializeField] private float itemMoveSpeed = 1.0f; // 아이템 이동 속도
    [SerializeField] private float itemRange = 5f; // 아이템 끌어당기는 범위

    [SerializeField] private Slider coolTime_Bag;
    [SerializeField] private Slider coolTime_Dash;
    [SerializeField] private Slider coolTime_Gun;

    CapsuleCollider collider;
    private Animator animator;
    private int money;
    private float walkAnimationSpeed;
    private float dashPower = 15f;
    private float dashCooldown = 2f;
    private float dashDuration = 0.5f; // 대쉬 지속 시간
    private bool isride;
    private bool canDash;
    private bool isDashing;

    private Vector3 dashTarget;

    private void Start()
    {
        coolTime_Bag.gameObject.SetActive(false);
        coolTime_Dash.gameObject.SetActive(false);
        coolTime_Gun.gameObject.SetActive(false);

        canDash = true;
        isDashing = false;
        rigidbody = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        collider = GetComponent<CapsuleCollider>();
        meshRenderer = GetComponent<MeshRenderer>();
        walkSpeed = 10;
        money = 1000;
        maxHp = 10;
        curHp = maxHp;
        playerimage.fillAmount = maxHp;
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

    IEnumerator Dash(Vector3 dashDirection)
    {
        // 대쉬가 시작되었음을 표시
        canDash = false;
        isDashing = true;
        dashTarget = transform.position + dashDirection.normalized * dashPower; // 대쉬 목표 위치 계산
        float elapsed = 0f; // 경과 시간 초기화
        Vector3 startPos = transform.position;
        while (elapsed < dashDuration) // 대쉬 지속 시간 동안 반복
        {
            transform.position = Vector3.Lerp(startPos, dashTarget, elapsed / dashDuration); // 현재 위치를 목표 위치로 선형 보간
            elapsed += Time.deltaTime; // 경과 시간 업데이트
            yield return null; // 다음 프레임까지 대기
        }

        isDashing = false; // 대쉬가 끝났음을 표시
        StartCoroutine(UpdateCooldownSlider(coolTime_Dash, dashCooldown)); // 쿨다운 슬라이더 업데이트 시작
    }

    public override void Move()
    {
        float horizontalMove = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        float speed = walkSpeed;
        float animSpeed = walkAnimationSpeed;

        translation = new Vector3(horizontalMove, 0, vertical);
        if (translation.magnitude > 0)
        {
            lastMoveDirection = translation; // 마지막 이동 방향 업데이트
        }

        translation *= speed * Time.deltaTime;
        transform.Translate(translation, Space.World);

        if (Input.GetKey(KeyCode.Space) && canDash && !isDashing)
        {
            coolTime_Dash.gameObject.SetActive(true);
            Vector3 dashDirection = (translation.magnitude > 0) ? translation : lastMoveDirection; // 정지 중이면 마지막 이동 방향으로 대쉬
            StartCoroutine(Dash(dashDirection));
        }

        animator.SetFloat("Vertical", vertical, 0.1f, Time.deltaTime);
        animator.SetFloat("Horizontal", horizontalMove, 0.1f, Time.deltaTime);
        animator.SetFloat("WalkSpeed", animSpeed);

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            transform.LookAt(new Vector3(hit.point.x, transform.position.y, hit.point.z));
        }

        if (Input.GetMouseButton(1))
        {
            if (meleeWeapon.Attack())
            {
                animator.SetTrigger("MeleeAttack");
                StartCoroutine(UpdateCooldownSlider(coolTime_Bag, meleeWeapon.GetReloadTime()));
                coolTime_Bag.value = 1;
            }
        }

        if (Input.GetMouseButton(0))
        {
            if (longRangeWeapon == null)
            {
                return;
            }
            if (longRangeWeapon.Attack())
            {
                StartCoroutine(UpdateCooldownSlider(coolTime_Gun, longRangeWeapon.GetReloadTime()));
                coolTime_Gun.value = 1;
            }
        }
    }

    private IEnumerator UpdateCooldownSlider(Slider slider, float cooldown)
    {
        slider.gameObject.SetActive(true);
        float elapsed = 0f; // 얼마나 지났나
        while (elapsed < cooldown)
        {
            elapsed += Time.deltaTime;
            slider.value = Mathf.Lerp(slider.maxValue, 0, elapsed / cooldown);
            yield return null;
        }
        slider.value = slider.maxValue;
        slider.gameObject.SetActive(false);
        canDash = true; // 대쉬 쿨다운 끝나야 대쉬 가능
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Vehicle"))
        {
            if (Input.GetKeyDown(KeyCode.E))
                StartCoroutine(ClickButton(collision.gameObject.GetComponent<Vehicle>()));
        }
    }
    private void Update()
    {
        switch (vehicle)
        {
            case null:
                Move();
                break;
            default:
                vehicle.Move();
                Interact();
                break;
        }

        // 아이템 끌어당기기
        AttractItems();
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

    public void Interact()
    {
        if (isride)
        {
            if (Input.GetKeyDown(KeyCode.E))
                StartCoroutine(ClickButton(null));
        }
    }
    IEnumerator ClickButton(Vehicle item)
    {
        switch (isride)
        {
            case true:
                yield return null;
                transform.SetParent(null);
                transform.position = vehicle.transform.position + (Vector3.right * 3);
                SetColliderEnabled(true);
                vehicle = null;
                isride = false;
                break;
            case false:
                if (!item) yield break;
                Debug.Log("ClickButton");
                yield return new WaitForSeconds(3f);
                SetColliderEnabled(isride);
                isride = true;
                vehicle = item;
                transform.SetParent(vehicle.gameObject.transform);
                break;
        }
        yield break;
    }

    public override void AddHp(float heal)
    {
        base.AddHp(heal);
    }

    public override void GetDamage(float damage)
    {
        base.GetDamage(damage);
    }

    public void SetColliderEnabled(bool check)
    {
        GetComponent<Collider>().enabled = check;
        meshRenderer.enabled = check;
    }
}
