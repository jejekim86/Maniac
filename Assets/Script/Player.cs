using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;

[RequireComponent(typeof(MeshRenderer))]
public class Player : Controller
{
    Vehicle vehicle;
    Vector3 translation;
    Vector3 lastMoveDirection = Vector3.forward; // ������ �̵� ������ ������ ����

    [SerializeField] Weapon longRangeWeapon;
    [SerializeField] Weapon meleeWeapon;
    [SerializeField] Text moneyText;
    [SerializeField] Image playerimage;
    [SerializeField] private float itemMoveSpeed = 1.0f; // ������ �̵� �ӵ�
    [SerializeField] private float itemRange = 5f; // ������ ������� ����

    [SerializeField] private Slider coolTime_Bag;
    [SerializeField] private Slider coolTime_Dash;
    [SerializeField] private Slider coolTime_Gun;

    CapsuleCollider collider;
    private Animator animator;
    private int money;
    private float walkAnimationSpeed;
    private float dashPower = 5000f;
    private float dashCooldown = 2f;
    private float dashDuration = 0.5f; // �뽬 ���� �ð�
    //private bool isride;
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
        // �뽬�� ���۵Ǿ����� ǥ��
        canDash = false;
        isDashing = true;
        dashTarget = transform.position + dashDirection.normalized * dashPower; // �뽬 ��ǥ ��ġ ���
        float elapsed = 0f; // ��� �ð� �ʱ�ȭ
        Vector3 startPos = transform.position;
        rigidbody.AddForce(dashDirection.normalized * dashPower, ForceMode.Impulse);
        /*
        while (elapsed < dashDuration) // �뽬 ���� �ð� ���� �ݺ�
        {
            rigidbody.MovePosition(Vector3.Lerp(startPos, dashTarget, elapsed / dashDuration)); // ���� ��ġ�� ��ǥ ��ġ�� ���� ����
            //transform.position = Vector3.Lerp(startPos, dashTarget, elapsed / dashDuration); // ���� ��ġ�� ��ǥ ��ġ�� ���� ����
            elapsed += Time.deltaTime; // ��� �ð� ������Ʈ
            yield return new WaitForFixedUpdate(); // ���� �����ӱ��� ���
        }
        */
        yield return new WaitForSeconds(0.5f);
        rigidbody.velocity = Vector3.zero;
        isDashing = false; // �뽬�� �������� ǥ��
        StartCoroutine(UpdateCooldownSlider(coolTime_Dash, dashCooldown)); // ��ٿ� �����̴� ������Ʈ ����
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
            lastMoveDirection = translation; // ������ �̵� ���� ������Ʈ
        }

        translation *= speed * Time.fixedDeltaTime;
        //translation *= speed * Time.deltaTime;
        //transform.Translate(translation, Space.World);
        rigidbody.MovePosition(rigidbody.position + translation);

        if (Input.GetKey(KeyCode.Space) && canDash && !isDashing)
        {
            coolTime_Dash.gameObject.SetActive(true);
            Vector3 dashDirection = (translation.magnitude > 0) ? translation : lastMoveDirection; // ���� ���̸� ������ �̵� �������� �뽬
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
        float elapsed = 0f; // �󸶳� ������
        while (elapsed < cooldown)
        {
            elapsed += Time.deltaTime;
            slider.value = Mathf.Lerp(slider.maxValue, 0, elapsed / cooldown);
            yield return null;
        }
        slider.value = slider.maxValue;
        slider.gameObject.SetActive(false);
        canDash = true; // �뽬 ��ٿ� ������ �뽬 ����
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Vehicle"))
        {
            if (Input.GetKeyDown(KeyCode.E))
                StartCoroutine(ClickButton(collision.gameObject.GetComponent<Vehicle>()));
        }
    }

    private void FixedUpdate()
    {
        Move();
    }

    private void Update()
    {
        switch (vehicle)
        {
            case null:
                //Move();
                break;
            default:
                vehicle.Move();
                Interact();
                break;
        }

        // ������ �������
        AttractItems();
    }

    private void AttractItems()
    {
        // "Item" �±׸� ���� ��� ���� ������Ʈ�� ã��
        GameObject[] items = GameObject.FindGameObjectsWithTag("Item");

        foreach (GameObject item in items)
        {
            // �÷��̾�� ������ ������ ������� �Ÿ� ���
            Vector3 relativePos = item.transform.position - transform.position;

            // �÷��̾���� �Ÿ��� ���� ���� ���� ���� ���� �̵�
            if (relativePos.magnitude <= itemRange)
            {
                // �������� �÷��̾�� �ε巴�� �̵�
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
