using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using UnityEngine;

public class Vehicle : Controller
{
    [SerializeField] WheelCollider[] wheelColliders;

    public override void AddHp(float heal) => base.AddHp(heal);
    public override void GetDamage(float damage) => base.GetDamage(damage);
    private void Start()
    {
        maxHp = 100;
    }
    public override void Move()
    {
        wheelColliders[0].steerAngle = wheelColliders[1].steerAngle = Input.GetAxis("Horizontal") * 20f;
        for (int i = 0; i < wheelColliders.Length; i++)
        {
            wheelColliders[i].motorTorque = -1 * (Input.GetAxis("Vertical") * 4000f);
            Debug.Log("motorTorque : " + wheelColliders[0].motorTorque);
        }

        if (Input.GetKeyDown(KeyCode.Space))
            ApplyBrakeTorque(50000);

        else
            ApplyBrakeTorque(0);
    }

    public void ApplyBrakeTorque(float value)
    {
        for (int i = 0; i < wheelColliders.Length; i++)
        {
            wheelColliders[i].brakeTorque = value;
            Debug.Log("BrakeTorque");
        }
    }
}