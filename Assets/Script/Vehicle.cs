using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vehicle : TestController
{
    Vector3 translation;
    [SerializeField] WheelCollider[] wheelColliders;
    [SerializeField] int hp = 10;
    [Range(0, 5)] public float brakepoint;

    public override void Move()
    {
        wheelColliders[0].steerAngle = wheelColliders[1].steerAngle = Input.GetAxis("Horizontal") * 20f;
        for(int i = 0; i <  wheelColliders.Length; i++)
            wheelColliders[i].motorTorque = -1 * Input.GetAxis("Vertical") * (2000f * 0.25f);

        //if (Input.GetKeyDown(KeyCode.Space))
        //{
        //    for (int i = 0; i < wheelColliders.Length; i++)
        //        wheelColliders[i].brakeTorque = brakepoint;
        //}

    }

}