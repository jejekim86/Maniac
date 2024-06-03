using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.Mathematics;
using UnityEngine;

public class Vehicle : TestController
{
    Vector3 translation;
    [SerializeField] WheelCollider[] wheelColliders;
    [SerializeField] int hp = 10;

    public override void Move()
    {
        wheelColliders[0].steerAngle = wheelColliders[1].steerAngle = Input.GetAxis("Horizontal") * 20f;
        for(int i = 0; i <  wheelColliders.Length; i++)
            wheelColliders[i].motorTorque = -1 * Input.GetAxis("Vertical") * (2000f * 0.25f);
    }
}