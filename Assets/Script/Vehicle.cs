using UnityEngine;

public class Vehicle : Controller
{
    [SerializeField] WheelCollider[] wheelColliders;
    [SerializeField] float maxSpeed = 50f;
    [SerializeField] float maxBrakeTorque = 50000f;
    [SerializeField] float brakeSpeed = 100;
    float currentmotorTorque;
    float currentBrakeTorque;


    public override void AddHp(float heal) => base.AddHp(heal);
    public override void GetDamage(float damage) => base.GetDamage(damage);
    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
    }
    private void Start()
    {
        maxHp = 10;
        curHp = maxHp;
        HP_image.fillAmount = maxHp;
        currentBrakeTorque = 0;
    }


    public void SetHp_imageActive(bool check) => HP_image.gameObject.SetActive(check);

    public override void Move()
    {
        wheelColliders[0].steerAngle = wheelColliders[1].steerAngle = Input.GetAxis("Horizontal") * 10f;

        float currentspeed = rigidbody.velocity.magnitude;
        currentmotorTorque = (Input.GetAxisRaw("Vertical") * 4000f);
        // max torque
        if (currentspeed >= maxSpeed)
            currentmotorTorque = 0;
        // Motor torque control
        for (int i = 0; i < wheelColliders.Length; i++)
        {
            wheelColliders[i].motorTorque = -1 * currentmotorTorque;
            Debug.Log("motorTorque : " + wheelColliders[0].motorTorque);
        }

        if (Input.GetAxisRaw("Vertical") != 0)
            currentBrakeTorque = Mathf.Lerp(currentBrakeTorque, maxBrakeTorque, brakeSpeed * Time.deltaTime);
        else
            currentBrakeTorque = 200;
        // Apply brake torque if space key is pressed
        if (Input.GetKey(KeyCode.Space))
            currentBrakeTorque = maxBrakeTorque;
        // Apply deceleration torque if no input is given
        else
            currentBrakeTorque = 0;

        ApplyBrakeTorque(currentBrakeTorque);
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