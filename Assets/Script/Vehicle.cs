using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vehicle : TestController
{
    Vector3 translation;


    public override void Move()
    {
        // TODO : �ӽ� ���̴ϱ� ���߿� Car Object�� ���� �����ؾ��� 
        float horizontalMove = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        float speed = 10;

        translation = Vector3.forward * (vertical * Time.deltaTime);
        translation += Vector3.right * (horizontalMove * Time.deltaTime);
        translation *= speed;
        transform.Translate(translation, Space.World);
    }

}