using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : TestController
{
    Vehicle vehicle;
    [SerializeField] Vehicle testvehicle;
    MeshRenderer meshRenderer;
    Vector3 translation;
    public bool isride { get; private set; }
    private void Start()
    {
        isride = false;
        meshRenderer = GetComponent<MeshRenderer>();
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
                break;
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


    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Vehicle"))
        {
            vehicle = collision.gameObject.GetComponent<Vehicle>();
            meshRenderer.enabled = false;
            transform.SetParent(vehicle.gameObject.transform);
        }
    }
}