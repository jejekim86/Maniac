using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*public class Test : TestController
{
    MeshRenderer meshRenderer;
    CapsuleCollider collider;
    Vector3 translation;


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

    

    public override void Move()
    {
        float horizontalMove = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        translation = Vector3.forward * (vertical * Time.deltaTime);
        translation += Vector3.right * (horizontalMove * Time.deltaTime);
        translation *= 10;
        transform.Translate(translation, Space.World);
    }

   

}*/