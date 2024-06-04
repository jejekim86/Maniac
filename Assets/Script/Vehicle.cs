using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vehicle : TestController
{
    [SerializeField] WheelCollider[] wheelColliders;

    public override void Move()
    {
        wheelColliders[0].steerAngle = wheelColliders[1].steerAngle = Input.GetAxis("Horizontal") * 20f;
        for(int i = 0; i <  wheelColliders.Length; i++)
            wheelColliders[i].motorTorque = -1 * Input.GetAxis("Vertical") * (2000f * 0.25f);
    }

}