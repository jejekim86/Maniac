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

    //[SerializeField] Weapon longRangeWeapon;
    //[SerializeField] Weapon meleeWeapon;
    //[SerializeField] Text moneyText;
    //[SerializeField] Image playerimage;
    [SerializeField] private float itemMoveSpeed = 1.0f; // 아이템 이동 속도
    [SerializeField] private float itemRange = 5f; // 아이템 끌어당기는 범위
    Image rideKey_Image;
    CapsuleCollider collider;
    private Animator animator;
    PlayerInventory inventory = new PlayerInventory();
    private float walkAnimationSpeed;
    private float dashPower;
    private bool isride;
    [SerializeField] Sprite sprite;
    bool canDash;
    IEnumerator Dash()
    {
        rigidbody.AddForce(translation * dashPower, ForceMode.Impulse);
        yield return new WaitForSeconds(0.5f);
        rigidbody.velocity = Vector3.zero;
        canDash = true;
    }
    public void SetLongRangeWeapon(Weapon weapon)
    {
        // longRangeWeapon = weapon;
    }

    public void SetMeleeWeapon(Weapon weapon)
    {
        //  meleeWeapon = weapon;
    }

    public void AddMoney(int amount)
    {
        inventory.EarnMoney(amount);
        //moneyText.text = inventory.money.ToString();
    }
    Canvas canvas;
    private void Awake()
    {
        GameObject ob = new GameObject();
        rideKey_Image = ob.AddComponent<Image>();
        canvas = FindObjectOfType<Canvas>();
        rideKey_Image.transform.SetParent(canvas.transform);
    }
    private void Start()
    {
        canDash = true;
        //rigidbody = GetComponent<Rigidbody>();
        //animator = GetComponent<Animator>();
        collider = GetComponent<CapsuleCollider>();
        meshRenderer = GetComponent<MeshRenderer>();
        walkSpeed = 10;
        inventory.EarnMoney(1000);
        maxHp = 10;
        curHp = maxHp;
        //playerimage.fillAmount = maxHp;
        rideKey_Image.sprite = sprite;
        rideKey_Image.gameObject.SetActive(false);
    }
    public override void Move()
    {

        float horizontalMove = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        float speed = walkSpeed;
        float animSpeed = walkAnimationSpeed;


        translation = Vector3.forward * (vertical * Time.deltaTime);
        translation += Vector3.right * (horizontalMove * Time.deltaTime);
        translation *= speed;
        transform.Translate(translation, Space.World);


        if (Input.GetKey(KeyCode.LeftShift) && canDash)
        {
            canDash = false;
            StartCoroutine(Dash());
        }


        //animator.SetFloat("Vertical", vertical, 0.1f, Time.deltaTime);
        //animator.SetFloat("Horizontal", horizontalMove, 0.1f, Time.deltaTime);
        //animator.SetFloat("WalkSpeed", animSpeed);


        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        Physics.Raycast(ray, out hit);
        transform.LookAt(new Vector3(hit.point.x, transform.position.y, hit.point.z));


        //if (Input.GetMouseButton(1))
        //{
        //    if (meleeWeapon.Attack())
        //        animator.SetTrigger("MeleeAttack");
        //}
        //
        //if (Input.GetMouseButton(0))
        //{
        //    longRangeWeapon.Attack();
        //}
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Vehicle"))
        {
            if (Input.GetKeyDown(KeyCode.E))
                StartCoroutine(ClickButton(collision.gameObject.GetComponent<Vehicle>()));
            rideKey_Image.gameObject.SetActive(true);
            Vector3 screenPosition = Camera.main.WorldToScreenPoint(collision.transform.position);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvas.transform as RectTransform,
                screenPosition,
                canvas.worldCamera,
                out Vector2 canvasPosition
            );
            rideKey_Image.transform.localScale = collision.transform.localScale * 0.25f;
            rideKey_Image.rectTransform.anchoredPosition = canvasPosition + (Vector2.up * 3);

        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Vehicle"))
            rideKey_Image.gameObject.SetActive(false);
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
                SetColliderEnabled(false);
                isride = true;
                vehicle = item;
                transform.position = vehicle.transform.localPosition;
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
        meshRenderer.enabled = check;
    }
}
