using System.ComponentModel;
using System.Diagnostics.Eventing.Reader;
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

    public void SetTag(string tagname)
    {
        foreach (string definedTag in UnityEditorInternal.InternalEditorUtility.tags)
        {
            if (definedTag == tagname)
            {
                gameObject.tag = tagname;
                return;
            }
        }
    }

    public void SetHp_imageActive(bool check) => HP_image.gameObject.SetActive(check);

    public override void Move()
    {
        wheelColliders[0].steerAngle = wheelColliders[1].steerAngle = Input.GetAxis("Horizontal") * 30f;

        float currentspeed = rigidbody.velocity.magnitude;
        currentmotorTorque = (Input.GetAxisRaw("Vertical") * 4000f);
        // max torque
        if (currentspeed >= maxSpeed)
            currentmotorTorque = 0;
        // Motor torque control
        for (int i = 0; i < wheelColliders.Length; i++)
        {
            wheelColliders[i].motorTorque = -1 * currentmotorTorque;
            //Debug.Log("motorTorque : " + wheelColliders[0].motorTorque);
        }

        if (currentBrakeTorque != 0)
            currentBrakeTorque = Mathf.Lerp(currentBrakeTorque, maxBrakeTorque, brakeSpeed * Time.deltaTime);
        else
            currentBrakeTorque = 200;
        // Apply brake torque if space key is pressed
        // �����̽� Ű�� ������ ��� �극��ũ ��ũ ����
        if (Input.GetKey(KeyCode.Space))
            currentBrakeTorque = maxBrakeTorque;
        else
            currentBrakeTorque = 0;

        ApplyBrakeTorque(currentBrakeTorque);

        // �Է��� ���� ��� �ڵ� ����
        if (Input.GetAxisRaw("Vertical") == 0)
            DecelerateVehicle();
    }

    public void ApplyBrakeTorque(float value)
    {
        for (int i = 0; i < wheelColliders.Length; i++)
            wheelColliders[i].brakeTorque = value;
    }

    private void DecelerateVehicle()
    {
        float decelerationFactor = 10f; // ���ϴ� ���ӷ��� ����
        float minSpeedThreshold = 0.1f; // �����ϱ� �� �ּ� �ӵ� �Ӱ谪

        Vector3 velocity = rigidbody.velocity;
        if (velocity.magnitude > minSpeedThreshold)
        {
            velocity -= velocity.normalized * decelerationFactor * Time.deltaTime;
            rigidbody.velocity = velocity;
        }
        else
        {
            rigidbody.velocity = Vector3.zero;
        }
    }
}