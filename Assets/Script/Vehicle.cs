using System;
using System.Collections;
using System.Collections.Generic;
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
            wheelColliders[i].motorTorque = -1 * (Input.GetAxis("Vertical") * 10000f);
            Debug.Log("motorTorque : " + wheelColliders[i].motorTorque);
        }

    }
}