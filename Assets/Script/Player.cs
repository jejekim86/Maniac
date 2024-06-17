using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.Burst.CompilerServices;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;

public class Player : Controller
{
    Vehicle vehicle;
    Vector3 translation;
    Vector3 lastMoveDirection = Vector3.forward;

    [SerializeField] Weapon longRangeWeapon;
    [SerializeField] Weapon meleeWeapon;
    [SerializeField] Text moneyText;
    [SerializeField] private float itemMoveSpeed = 1.0f;
    [SerializeField] private float itemRange = 5f;
    [SerializeField] private GameObject RenderObject;

    [Header("Slider")]
    [SerializeField] private Slider coolTime_Bag;
    [SerializeField] private Slider coolTime_Dash;
    [SerializeField] private Slider coolTime_Gun;
    [SerializeField] private Slider coolTime_Ride;
    CapsuleCollider collider;
    private Animator animator;
    private int money;
    private float walkAnimationSpeed;
    private float dashPower = 50f;
    private float dashCooldown = 2f;
    private float dashDuration = 0.5f;
    private bool isride;
    private bool canDash;
    private bool isDashing;
    private bool isclicked;
    private Vector3 dashTarget;

    public override void AddHp(float heal) => base.AddHp(heal);
    public override void GetDamage(float damage) => base.GetDamage(damage);
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
        walkSpeed = 10;
        money = 1000;
        maxHp = 10;
        curHp = maxHp;
        HP_image.fillAmount = maxHp;
        isclicked = false;
    }

    public void SetLongRangeWeapon(Weapon weapon) => longRangeWeapon = weapon;

    public void SetMeleeWeapon(Weapon weapon) => meleeWeapon = weapon;

    public void AddMoney(int amount)
    {
        money += amount;
        moneyText.text = money.ToString();
    }

    IEnumerator Dash(Vector3 dashDirection)
    {
        canDash = false;
        isDashing = true;
        dashTarget = transform.position + dashDirection.normalized * dashPower;
        Vector3 startPos = transform.position;
        rigidbody.AddForce(dashDirection.normalized * dashPower, ForceMode.Impulse);
        /*
        while (elapsed < dashDuration) 
        {
            rigidbody.MovePosition(Vector3.Lerp(startPos, dashTarget, elapsed / dashDuration)); // ���� ��ġ�� ��ǥ ��ġ�� ���� ����
            //transform.position = Vector3.Lerp(startPos, dashTarget, elapsed / dashDuration); // ���� ��ġ�� ��ǥ ��ġ�� ���� ����
            elapsed += Time.deltaTime; 
            yield return new WaitForFixedUpdate(); 
        }
        */
        yield return new WaitForSeconds(0.5f);
        rigidbody.velocity = Vector3.zero;
        isDashing = false;
        StartCoroutine(UpdateCooldownSlider(coolTime_Dash, dashCooldown));
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
            lastMoveDirection = translation;
        }

        translation *= speed * Time.fixedDeltaTime;
        //translation *= speed * Time.deltaTime;
        //transform.Translate(translation, Space.World);
        rigidbody.MovePosition(rigidbody.position + translation);

        if (Input.GetKey(KeyCode.Space) && canDash && !isDashing)
        {
            coolTime_Dash.gameObject.SetActive(true);
            Vector3 dashDirection = (translation.magnitude > 0) ? translation : lastMoveDirection;
            StartCoroutine(Dash(dashDirection));
        }

        animator.SetFloat("Vertical", vertical, 0.1f, Time.deltaTime);
        animator.SetFloat("Horizontal", horizontalMove, 0.1f, Time.deltaTime);
        animator.SetFloat("WalkSpeed", animSpeed);

        //Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        //RaycastHit hit;
        //if (Physics.Raycast(ray, out hit))
        //    transform.LookAt(new Vector3(hit.point.x, transform.position.y, hit.point.z));
        //
        //if (Input.GetMouseButton(0))
        //{
        //    if (longRangeWeapon == null) return;
        //    if (longRangeWeapon.Attack())
        //    {
        //        StartCoroutine(UpdateCooldownSlider(coolTime_Gun, longRangeWeapon.GetReloadTime()));
        //        coolTime_Gun.value = 1;
        //    }
        //}
        //if (Input.GetMouseButton(1))
        //{
        //    if (meleeWeapon.Attack())
        //    {
        //        animator.SetTrigger("MeleeAttack");
        //        StartCoroutine(UpdateCooldownSlider(coolTime_Bag, meleeWeapon.GetReloadTime()));
        //        coolTime_Bag.value = 1;
        //    }
        //}

    }

    public void Attack()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
            transform.LookAt(new Vector3(hit.point.x, transform.position.y, hit.point.z));

        if (Input.GetMouseButton(0))
        {
            if (longRangeWeapon == null) return;

            if (longRangeWeapon.Attack())
            {
                StartCoroutine(UpdateCooldownSlider(coolTime_Gun, longRangeWeapon.GetReloadTime()));
                coolTime_Gun.value = 1;
            }
        }
        if (vehicle) return;

        if (Input.GetMouseButton(1))
        {
            if (meleeWeapon.Attack())
            {
                animator.SetTrigger("MeleeAttack");
                StartCoroutine(UpdateCooldownSlider(coolTime_Bag, meleeWeapon.GetReloadTime()));
                coolTime_Bag.value = 1;
            }
        }
    }

    private IEnumerator UpdateCooldownSlider(Slider slider, float cooldown)
    {
        slider.gameObject.SetActive(true);
        float elapsed = 0f;
        while (elapsed < cooldown)
        {
            elapsed += Time.deltaTime;
            slider.value = Mathf.Lerp(slider.maxValue, 0, elapsed / cooldown);
            yield return null;
        }
        slider.value = slider.maxValue;
        slider.gameObject.SetActive(false);
        canDash = true;
    }


    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Vehicle"))
            Interact(collision.gameObject.GetComponent<Vehicle>());
    }

    private void OnCollisionEnter(Collision collision)
    {
        rigidbody.constraints = RigidbodyConstraints.FreezeAll;
    }

    private void OnCollisionExit(Collision collision)
    {
        rigidbody.constraints = RigidbodyConstraints.None;
    }



    private void FixedUpdate()
    {
        switch (vehicle)
        {
            case null:
                Move();
                rigidbody.constraints = RigidbodyConstraints.None;
                break;
            default:
                vehicle.Move();
                if (Input.GetKeyDown(KeyCode.E))
                    StartCoroutine(ClickButton());
                break;
        }
    }

    private void Update()
    {
        AttractItems();
        Attack();
    }

    private void AttractItems()
    {

        GameObject[] items = GameObject.FindGameObjectsWithTag("Item");

        foreach (GameObject item in items)
        {
            Vector3 relativePos = item.transform.position - transform.position;


            if (relativePos.magnitude <= itemRange)
                item.transform.position = Vector3.Lerp(item.transform.position, transform.position, itemMoveSpeed * Time.deltaTime);
        }
    }

    public void Interact(Vehicle item)
    {
        switch (isride)
        {
            case true:
                if (Input.GetKeyDown(KeyCode.E))
                    StartCoroutine(ClickButton(null));
                break;
            default:
                if (Input.GetKeyDown(KeyCode.E))
                    StartCoroutine(IncreaseSlider(item));
                break;
        }
    }
    IEnumerator IncreaseSlider(Vehicle item)
    {
        float duration = 3.0f; // 슬라이더가 채워지는 시간
        float startTime = Time.time; // 시작 시간
        coolTime_Ride.gameObject.SetActive(true);
        while (Time.time - startTime < duration)
        {
            coolTime_Ride.transform.position = Camera.main.WorldToScreenPoint(item.transform.position);
            coolTime_Ride.value += (coolTime_Ride.maxValue / duration) * Time.deltaTime;
            coolTime_Ride.value = Mathf.Clamp(coolTime_Ride.value, 0, coolTime_Ride.maxValue);
            yield return null; // 다음 프레임까지 대기
        }

        coolTime_Ride.value = coolTime_Ride.maxValue;
        StartCoroutine(ClickButton(item));
        coolTime_Ride.value = 0;
        coolTime_Ride.gameObject.SetActive(false);
    }
    IEnumerator ClickButton(Vehicle item = null)
    {
        switch (vehicle)
        {
            case null:
                if (!item) yield break; // 차에서 탑승할때
                Debug.Log("ClickButton");
                vehicle = item;
                vehicle.SetHp_imageActive(true);
                transform.SetParent(vehicle.gameObject.transform);
                transform.localPosition = Vector3.zero;
                transform.localRotation = Quaternion.Euler(Vector3.zero);
                rigidbody.constraints = RigidbodyConstraints.FreezeAll;
                SetColliderEnabled(false);
                break;
            default: // 차에서 내릴때
                transform.SetParent(null);
                transform.position = vehicle.transform.position + (Vector3.right * 3);
                rigidbody.constraints = RigidbodyConstraints.None;
                vehicle.SetHp_imageActive(false);
                vehicle = null;
                SetColliderEnabled(true);
                break;
        }
        yield break;
    }
    public void SetColliderEnabled(bool check)
    {
        collider.enabled = check;
        RenderObject.gameObject.SetActive(check);
    }
}
