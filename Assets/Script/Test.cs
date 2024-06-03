using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Test : TestController
{
    Vehicle vehicle;
    MeshRenderer meshRenderer;
    CapsuleCollider collider;
    Vector3 translation;

    float buttonpushtime = 0;
    public bool isride { get; private set; }
    private void Start()
    {
        isride = false;
        meshRenderer = GetComponent<MeshRenderer>();
        collider = GetComponent<CapsuleCollider>();
        buttonpushtime = 0;
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
            if (Input.GetKey(KeyCode.F))
            {
                transform.SetParent(null);
                transform.position = vehicle.transform.position + (Vector3.right * 3);
                meshRenderer.enabled = true;
                collider.enabled = true;
                vehicle = null;
                isride = false;
            }
        }
    }

    public override void Move()
    {
        float horizontalMove = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        float speed = 10;

        translation = Vector3.forward * (vertical * Time.deltaTime);
        translation += Vector3.right * (horizontalMove * Time.deltaTime);
        translation *= speed;
        transform.Translate(translation, Space.World);
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Vehicle"))
        {
            if (Input.GetKey(KeyCode.E))
            {
                vehicle = collision.gameObject.GetComponent<Vehicle>();
                meshRenderer.enabled = false;
                collider.enabled = false;
                transform.SetParent(vehicle.gameObject.transform);
                isride = true;
            }
        }
    }
}