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

    [SerializeField] Weapon longRangeWeapon;
    //[SerializeField] Weapon meleeWeapon;
    //[SerializeField] Text moneyText;
    //[SerializeField] Image playerimage;
    CapsuleCollider collider;
    private Animator animator;
    //private int money;
    private float walkAnimationSpeed;
    private float dashPower;
    private bool isride;

    bool canDash;
    IEnumerator Dash()
    {
        float lerpT = 0;
        Vector3 startTransform = transform.position;
        Vector3 endTransform = transform.position + (transform.forward * 5);

        Transform startTransform2 = transform;

        while (lerpT <= 0.5f)
        {
            lerpT += Time.deltaTime * 2;

            transform.position = Vector3.Lerp(startTransform, endTransform, lerpT);
            //rigidbody.AddForce(translation * dashPower, ForceMode.Impulse);
            yield return null;
        }
        Debug.Log("대쉬 종료");
        canDash = true;
    }
    public void SetLongRangeWeapon(Weapon weapon)
    {
        longRangeWeapon = weapon;
    }

    public void SetMeleeWeapon(Weapon weapon)
    {
        //meleeWeapon = weapon;
    }

    public void AddMoney(int amount)
    {
        //money += amount;
        //moneyText.text = money.ToString();
    }

    private void Start()
    {
        canDash = true;
        rigidbody = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        //money = 1000; 
        collider = GetComponent<CapsuleCollider>();
        meshRenderer = GetComponent<MeshRenderer>();
        walkSpeed = 10;
        maxHp = 10;
        //playerimage.fillAmount = maxHp;
        curHp = maxHp;
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


        if (Input.GetKey(KeyCode.Space) && canDash)
        {
            canDash = false;
            StartCoroutine(Dash());
        }


        animator.SetFloat("Vertical", vertical, 0.1f, Time.deltaTime);
        animator.SetFloat("Horizontal", horizontalMove, 0.1f, Time.deltaTime);
        animator.SetFloat("WalkSpeed", animSpeed);


        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        Physics.Raycast(ray, out hit);
        transform.LookAt(new Vector3(hit.point.x, transform.position.y, hit.point.z));


        //if (Input.GetMouseButton(1))
        //{
        //    if (meleeWeapon.Attack())
        //        animator.SetTrigger("MeleeAttack");
        //}

        if (Input.GetMouseButton(0))
        {
            longRangeWeapon.Attack();
        }
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
