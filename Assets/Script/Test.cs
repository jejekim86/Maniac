using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.UI;

public class Test : Controller
{
    Vector3 translation;
    CapsuleCollider collider;
    [SerializeField] float movespeed;
    [SerializeField] Weapon longRangeWeapon;
    private void Start()
    {
        collider = GetComponent<CapsuleCollider>();
        meshRenderer = GetComponent<MeshRenderer>();
        rigidbody = GetComponent<Rigidbody>();
        //rigidbody.interpolation = RigidbodyInterpolation.Interpolate;
        isride = false;
        HP_image = null;
        vehicle = null;
    }

    public override void Move()
    {
        float horizontalMove = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        float speed = movespeed;

        translation = new Vector3(horizontalMove, 0, vertical);
        translation *= speed * Time.fixedDeltaTime;
        //translation *= speed * Time.deltaTime;
        rigidbody.MovePosition(rigidbody.position + translation);

    }


    IEnumerator ClickButton(Vehicle item = null)
    {
        switch (isride)
        {
            case true: // 차에서 내릴때
                yield return null;
                transform.SetParent(null);
                transform.position = vehicle.transform.position + (Vector3.right * 3);
                isride = false;
                vehicle = null;
                SetColliderEnabled(true);
                break;
            case false:
                if (!item) yield break; // 차에서 탑승할때
                Debug.Log("ClickButton");
                yield return new WaitForSeconds(3f);
                isride = true;
                vehicle = item;
                transform.SetParent(vehicle.gameObject.transform);
                SetColliderEnabled(false);
                break;
        }
        yield break;
    }


    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Vehicle"))
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                StartCoroutine(ClickButton(collision.gameObject.GetComponent<Vehicle>()));
            }
        }
    }



    private void FixedUpdate()
    {
        switch (vehicle == null)
        {
            case false:
                vehicle.Move();
                transform.localPosition = Vector3.zero;
                transform.localRotation = Quaternion.Euler(Vector3.zero);
                if (Input.GetKeyDown(KeyCode.E))
                    StartCoroutine(ClickButton());
                break;
            case true:
                Move();
                break;
        }
    }

    private void Update()
    {
        if (Input.GetMouseButton(0))
        {
            if (longRangeWeapon) return;
            if (longRangeWeapon.Attack())
                Debug.Log("Attack");


            Debug.Log("Input.GetMouseButton(0)");
        }
    }

    public void TestSetLongRangeWeapon(Weapon weapon)
    {
        longRangeWeapon = weapon;
    }

    public void SetColliderEnabled(bool check)
    {
        collider.isTrigger = !check;
        meshRenderer.enabled = check;
    }
}
