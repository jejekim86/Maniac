using System.Collections;
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
        // 스페이스 키가 눌러진 경우 브레이크 토크 적용
        if (Input.GetKey(KeyCode.Space))
            currentBrakeTorque = maxBrakeTorque;
        else
            currentBrakeTorque = 0;

        ApplyBrakeTorque(currentBrakeTorque);

        // 입력이 없는 경우 자동 감속
        if (Input.GetAxisRaw("Vertical") == 0)
            DecelerateVehicle();


    }
    private float resetThreshold = 90f; // 차량이 뒤집히는 각도 임계값
    private float resetSpeed = 2f; // 차량을 복원하는 속도
    private void FixedUpdate()
    {
        float currentXRotation = NormalizeAngle(transform.eulerAngles.x);
        float currentZRotation = NormalizeAngle(transform.eulerAngles.z);

        // X축 또는 Z축이 임계값 이상으로 기울어졌는지 확인
        if (Mathf.Abs(currentXRotation) > resetThreshold || Mathf.Abs(currentZRotation) > resetThreshold)
            StartCoroutine(ResetRotation(currentXRotation, currentZRotation));
    }
    private float NormalizeAngle(float angle)
    {
        // 각도를 -180도에서 180도 사이로 변환
        if (angle > 180)
            angle -= 360;
        return angle;
    }
    private IEnumerator ResetRotation(float currentXRotation, float currentZRotation)
    {
        Quaternion targetRotation = Quaternion.Euler(0f, transform.eulerAngles.y, 0f);

        // 차량이 목표 회전 상태로 자연스럽게 회전
        while (Quaternion.Angle(transform.rotation, targetRotation) > 0.1f)
        {
            float step = resetSpeed * Time.deltaTime;

            // X축과 Z축 회전 방향을 결정
            float newXRotation = Mathf.MoveTowardsAngle(transform.eulerAngles.x, 0f, step);
            float newZRotation = Mathf.MoveTowardsAngle(transform.eulerAngles.z, 0f, step);

            // 회전 적용
            transform.eulerAngles = new Vector3(newXRotation, transform.eulerAngles.y, newZRotation);

            yield return null;
        }

        // 최종적으로 정확하게 0도로 설정
        transform.rotation = targetRotation;
    }

    public void ApplyBrakeTorque(float value)
    {
        for (int i = 0; i < wheelColliders.Length; i++)
            wheelColliders[i].brakeTorque = value;
    }

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