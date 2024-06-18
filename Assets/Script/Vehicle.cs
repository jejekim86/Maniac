using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class Vehicle : Controller
{
    [SerializeField] WheelCollider[] wheelColliders;
    [SerializeField] float motorTorque = 2000; // ���� ��ũ
    [SerializeField] float braketorque = 4000; // �극��ũ ��ũ
    [SerializeField] float maxSpeed = 20; // �ִ� �ӵ�
    [SerializeField] float steeringRange = 50; // ���� ����
    [SerializeField] float steeringRangeAtMaxSpeed = 10; // �ִ� �ӵ������� ���� ����
    [SerializeField] float brakeSpeed = 100;
    //float currentmotorTorque;
    //float currentBrakeTorque;
    float centreOfGravityOffset = -1f; // ���� �߽� ������ ? 
    WheelFrictionCurve frontwheelFriction;
    WheelFrictionCurve backwheelFriction;


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
        //HP_image.fillAmount = maxHp;
        //currentBrakeTorque = 0;
        rigidbody.centerOfMass += Vector3.up * centreOfGravityOffset;
        frontwheelFriction = wheelColliders[0].sidewaysFriction;
        backwheelFriction = wheelColliders[wheelColliders.Length - 1].sidewaysFriction;
    }

    public void SetHp_imageActive(bool check) => HP_image.gameObject.SetActive(check);

    public override void Move()
    {
        float vInput = -1 * Input.GetAxis("Vertical");
        float hInput = Input.GetAxis("Horizontal");

        // ������ ���� ���⿡ ���� ���� �ӵ��� ����մϴ�
        // (���������� �̵��� ���� ������ ��ȯ)
        float forwardSpeed = Vector3.Dot(transform.forward, rigidbody.velocity);

        // ������ �ְ� �ӵ��� �󸶳� ���������
        // 0���� 1 ������ ���ڷ� ����մϴ�
        float speedFactor = Mathf.InverseLerp(0, maxSpeed, forwardSpeed);

        // �̸� ����Ͽ� ����� �� �ִ� ��ũ�� ����մϴ�
        // (�ְ� �ӵ������� ��ũ�� 0)
        float currentMotorTorque = Mathf.Lerp(motorTorque, 0, speedFactor);

        // ���׸��� ���� ������ ����մϴ�
        // (�ְ� �ӵ������� ������ �� �ε巴�� ����)
        float currentSteerRange = Mathf.Lerp(steeringRange, steeringRangeAtMaxSpeed, speedFactor);

        // ����� �Է��� ������ �ӵ��� ���� �������� Ȯ���մϴ�
        bool isAccelerating = Mathf.Sign(vInput) == Mathf.Sign(forwardSpeed);

        wheelColliders[0].steerAngle = wheelColliders[1].steerAngle = hInput * currentSteerRange;

        switch (forwardSpeed)
        {
            case > 0: // ����
                wheelColliders[0].sidewaysFriction = backwheelFriction;
                wheelColliders[1].sidewaysFriction = backwheelFriction;
                wheelColliders[2].sidewaysFriction = frontwheelFriction;
                wheelColliders[3].sidewaysFriction = frontwheelFriction;
                break;
            case < 0: // ����
                wheelColliders[0].sidewaysFriction = frontwheelFriction;
                wheelColliders[1].sidewaysFriction = frontwheelFriction;
                wheelColliders[2].sidewaysFriction = backwheelFriction;
                wheelColliders[3].sidewaysFriction = backwheelFriction;
                break;
        }
        foreach (var wheel in wheelColliders)
        {
            // "Steerable"�� Ȱ��ȭ�� Wheel Colliders�� ���� ����
            if (isAccelerating)
            {
                // "Motorized"�� Ȱ��ȭ�� Wheel Colliders�� ��ũ ����
                wheel.motorTorque = vInput * currentMotorTorque;
                wheel.brakeTorque = 0;
            }
            else
            {
                // ����ڰ� �ݴ� �������� ������ �� ��
                // ��� ������ �극��ũ ����
                wheel.brakeTorque = Mathf.Abs(vInput) * braketorque;
                wheel.motorTorque = 0;
            }
            if (Input.GetKeyDown(KeyCode.E))
            {
                wheel.brakeTorque = Mathf.Abs(vInput) * braketorque;
                wheel.motorTorque = 0;
            }
        }
       if (vInput == 0)
            DecelerateVehicle();
    }




    //private float resetThreshold = 90f; // ������ �������� ���� �Ӱ谪
    //private float resetSpeed = 2f; // ������ �����ϴ� �ӵ�
    //private void FixedUpdate()
    //{
    //    float currentXRotation = NormalizeAngle(transform.eulerAngles.x);
    //    float currentZRotation = NormalizeAngle(transform.eulerAngles.z);
    //
    //    // X�� �Ǵ� Z���� �Ӱ谪 �̻����� ���������� Ȯ��
    //    if (Mathf.Abs(currentXRotation) > resetThreshold || Mathf.Abs(currentZRotation) > resetThreshold)
    //        StartCoroutine(ResetRotation(currentXRotation, currentZRotation));
    //}
    //private float NormalizeAngle(float angle)
    //{
    //    // ������ -180������ 180�� ���̷� ��ȯ
    //    if (angle > 180)
    //        angle -= 360;
    //    return angle;
    //}
    //private IEnumerator ResetRotation(float currentXRotation, float currentZRotation)
    //{
    //    Quaternion targetRotation = Quaternion.Euler(0f, transform.eulerAngles.y, 0f);
    //
    //    // ������ ��ǥ ȸ�� ���·� �ڿ������� ȸ��
    //    while (Quaternion.Angle(transform.rotation, targetRotation) > 0.1f)
    //    {
    //        float step = resetSpeed * Time.deltaTime;
    //
    //        // X��� Z�� ȸ�� ������ ����
    //        float newXRotation = Mathf.MoveTowardsAngle(transform.eulerAngles.x, 0f, step);
    //        float newZRotation = Mathf.MoveTowardsAngle(transform.eulerAngles.z, 0f, step);
    //
    //        // ȸ�� ����
    //        transform.eulerAngles = new Vector3(newXRotation, transform.eulerAngles.y, newZRotation);
    //
    //        yield return null;
    //    }
    //
    //    // ���������� ��Ȯ�ϰ� 0���� ����
    //    transform.rotation = targetRotation;
    //}
    //
    //public void ApplyBrakeTorque(float value)
    //{
    //    for (int i = 0; i < wheelColliders.Length; i++)
    //        wheelColliders[i].brakeTorque = value;
    //}
    //
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

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
            collision.gameObject.GetComponent<Enemy>().GetDamage(15);
    }
}