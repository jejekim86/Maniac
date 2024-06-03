using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vehicle : TestController
{
    Vector3 translation;


    public override void Move()
    {
        // TODO : 임시 용이니깐 나중에 Car Object에 따라서 수정해야함 
        float horizontalMove = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        float speed = 10;

        translation = Vector3.forward * (vertical * Time.deltaTime);
        translation += Vector3.right * (horizontalMove * Time.deltaTime);
        translation *= speed;
        transform.Translate(translation, Space.World);
    }

}