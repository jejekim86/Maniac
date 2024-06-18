using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class Vehicle : Controller
{
    [SerializeField] WheelCollider[] wheelColliders;
    [SerializeField] float motorTorque = 2000; // 모터 토크
    [SerializeField] float braketorque = 4000; // 브레이크 토크
    [SerializeField] float maxSpeed = 20; // 최대 속도
    [SerializeField] float steeringRange = 50; // 조향 범위
    [SerializeField] float steeringRangeAtMaxSpeed = 10; // 최대 속도에서의 조향 범위
    [SerializeField] float brakeSpeed = 100;
    //float currentmotorTorque;
    //float currentBrakeTorque;
    float centreOfGravityOffset = -1f; // 무게 중심 오프셋 ? 
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

        // 차량의 전방 방향에 대한 현재 속도를 계산합니다
        // (역방향으로 이동할 때는 음수를 반환)
        float forwardSpeed = Vector3.Dot(transform.forward, rigidbody.velocity);

        // 차량이 최고 속도에 얼마나 가까운지를
        // 0에서 1 사이의 숫자로 계산합니다
        float speedFactor = Mathf.InverseLerp(0, maxSpeed, forwardSpeed);

        // 이를 사용하여 사용할 수 있는 토크를 계산합니다
        // (최고 속도에서는 토크가 0)
        float currentMotorTorque = Mathf.Lerp(motorTorque, 0, speedFactor);

        // …그리고 조향 범위를 계산합니다
        // (최고 속도에서는 차량이 더 부드럽게 조향)
        float currentSteerRange = Mathf.Lerp(steeringRange, steeringRangeAtMaxSpeed, speedFactor);

        // 사용자 입력이 차량의 속도와 같은 방향인지 확인합니다
        bool isAccelerating = Mathf.Sign(vInput) == Mathf.Sign(forwardSpeed);

        wheelColliders[0].steerAngle = wheelColliders[1].steerAngle = hInput * currentSteerRange;

        switch (forwardSpeed)
        {
            case > 0: // 전진
                wheelColliders[0].sidewaysFriction = backwheelFriction;
                wheelColliders[1].sidewaysFriction = backwheelFriction;
                wheelColliders[2].sidewaysFriction = frontwheelFriction;
                wheelColliders[3].sidewaysFriction = frontwheelFriction;
                break;
            case < 0: // 후진
                wheelColliders[0].sidewaysFriction = frontwheelFriction;
                wheelColliders[1].sidewaysFriction = frontwheelFriction;
                wheelColliders[2].sidewaysFriction = backwheelFriction;
                wheelColliders[3].sidewaysFriction = backwheelFriction;
                break;
        }
        foreach (var wheel in wheelColliders)
        {
            // "Steerable"이 활성화된 Wheel Colliders에 조향 적용
            if (isAccelerating)
            {
                // "Motorized"가 활성화된 Wheel Colliders에 토크 적용
                wheel.motorTorque = vInput * currentMotorTorque;
                wheel.brakeTorque = 0;
            }
            else
            {
                // 사용자가 반대 방향으로 가려고 할 때
                // 모든 바퀴에 브레이크 적용
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




    //private float resetThreshold = 90f; // 차량이 뒤집히는 각도 임계값
    //private float resetSpeed = 2f; // 차량을 복원하는 속도
    //private void FixedUpdate()
    //{
    //    float currentXRotation = NormalizeAngle(transform.eulerAngles.x);
    //    float currentZRotation = NormalizeAngle(transform.eulerAngles.z);
    //
    //    // X축 또는 Z축이 임계값 이상으로 기울어졌는지 확인
    //    if (Mathf.Abs(currentXRotation) > resetThreshold || Mathf.Abs(currentZRotation) > resetThreshold)
    //        StartCoroutine(ResetRotation(currentXRotation, currentZRotation));
    //}
    //private float NormalizeAngle(float angle)
    //{
    //    // 각도를 -180도에서 180도 사이로 변환
    //    if (angle > 180)
    //        angle -= 360;
    //    return angle;
    //}
    //private IEnumerator ResetRotation(float currentXRotation, float currentZRotation)
    //{
    //    Quaternion targetRotation = Quaternion.Euler(0f, transform.eulerAngles.y, 0f);
    //
    //    // 차량이 목표 회전 상태로 자연스럽게 회전
    //    while (Quaternion.Angle(transform.rotation, targetRotation) > 0.1f)
    //    {
    //        float step = resetSpeed * Time.deltaTime;
    //
    //        // X축과 Z축 회전 방향을 결정
    //        float newXRotation = Mathf.MoveTowardsAngle(transform.eulerAngles.x, 0f, step);
    //        float newZRotation = Mathf.MoveTowardsAngle(transform.eulerAngles.z, 0f, step);
    //
    //        // 회전 적용
    //        transform.eulerAngles = new Vector3(newXRotation, transform.eulerAngles.y, newZRotation);
    //
    //        yield return null;
    //    }
    //
    //    // 최종적으로 정확하게 0도로 설정
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
        float decelerationFactor = 10f; // 원하는 감속률을 조정
        float minSpeedThreshold = 0.1f; // 정지하기 전 최소 속도 임계값

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