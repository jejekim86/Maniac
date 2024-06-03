using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEditor.Progress;

public class Test : TestController
{
    Vehicle vehicle;
    MeshRenderer meshRenderer;
    CapsuleCollider collider;
    Vector3 translation;


    private bool isride;
    private void Start()
    {
        isride = false;
        meshRenderer = GetComponent<MeshRenderer>();
        collider = GetComponent<CapsuleCollider>();
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
            //if (Input.GetKey(KeyCode.F))
            //{
            //    transform.SetParent(null);
            //    transform.position = vehicle.transform.position + (Vector3.right * 3);
            //    SetColliderEnabled(true);
            //    vehicle = null;
            //    isride = false;
            //}
            if (Input.GetKeyDown(KeyCode.E))
                StartCoroutine(ClickButton(null));
        }
    }

    public override void Move()
    {
        float horizontalMove = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        translation = Vector3.forward * (vertical * Time.deltaTime);
        translation += Vector3.right * (horizontalMove * Time.deltaTime);
        translation *= 10;
        transform.Translate(translation, Space.World);
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Vehicle"))
        {
            //if (Input.GetKeyDown(KeyCode.E))
            //{
            //    vehicle = collision.gameObject.GetComponent<Vehicle>();
            //    SetColliderEnabled(false);
            //    transform.SetParent(vehicle.gameObject.transform);
            //    isride = true;
            //}
            //
            if(Input.GetKeyDown(KeyCode.E))
                StartCoroutine(ClickButton(collision.gameObject.GetComponent<Vehicle>()));
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

    public void SetColliderEnabled(bool check)
    {
        collider.enabled = check;
        meshRenderer.enabled = check;
    }
}