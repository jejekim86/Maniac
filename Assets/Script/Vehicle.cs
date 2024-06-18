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
        // ���� ����
        wheelColliders[0].steerAngle = wheelColliders[1].steerAngle = Input.GetAxis("Horizontal") * 30f;

        float currentSpeed = rigidbody.velocity.magnitude;
        float inputVertical = -1 * Input.GetAxisRaw("Vertical");
        // ������ ���� �ӵ� ������ Ȯ��
        bool isMovingForward = Vector3.Dot(rigidbody.velocity, (-1 * transform.forward)) > 0;
        // ���� ���� ��ũ�� ��� ���¿� ���� �Է¿� ���� ����
        if (inputVertical < 0)
        {
            if (!isMovingForward)
            {
                
                currentmotorTorque = Mathf.Lerp(currentmotorTorque, 0f, Time.deltaTime * 5f);
            }
            else
            {
                currentmotorTorque = Mathf.Lerp(currentmotorTorque, inputVertical * 4000f, Time.deltaTime * 2f);
            }
        }
        else if (inputVertical > 0)
        {
            if (isMovingForward)
            {
                
                currentmotorTorque = Mathf.Lerp(currentmotorTorque, 0f, Time.deltaTime * 5f);
            }
            else
            {
                currentmotorTorque = Mathf.Lerp(currentmotorTorque, inputVertical * 2000f, Time.deltaTime * 2f);
            }
        }
        else
        {
            currentmotorTorque = Mathf.Lerp(currentmotorTorque, 0f, Time.deltaTime * 2f);
        }

        // ���� ��ũ ���� ����
        for (int i = 0; i < wheelColliders.Length; i++)
        {
            wheelColliders[i].motorTorque = currentmotorTorque;
        }

        // �극��ũ ��ũ ����
        if (Input.GetKey(KeyCode.Space))
        {
            currentBrakeTorque = Mathf.Lerp(currentBrakeTorque, maxBrakeTorque, brakeSpeed * Time.deltaTime);
        }
        else if (Input.GetAxisRaw("Vertical") == 0)
        {
            currentBrakeTorque = Mathf.Lerp(currentBrakeTorque, 200, brakeSpeed * Time.deltaTime);
        }
        else
        {
            currentBrakeTorque = 0;
        }

        ApplyBrakeTorque(currentBrakeTorque);

        // �Է��� ���� ��� �ڵ� ����
        if (Input.GetAxisRaw("Vertical") == 0)
        {
            DecelerateVehicle();
        }
    }
    private float resetThreshold = 90f; // ������ �������� ���� �Ӱ谪
    private float resetSpeed = 2f; // ������ �����ϴ� �ӵ�
    private void FixedUpdate()
    {
        float currentXRotation = NormalizeAngle(transform.eulerAngles.x);
        float currentZRotation = NormalizeAngle(transform.eulerAngles.z);

        // X�� �Ǵ� Z���� �Ӱ谪 �̻����� ���������� Ȯ��
        if (Mathf.Abs(currentXRotation) > resetThreshold || Mathf.Abs(currentZRotation) > resetThreshold)
            StartCoroutine(ResetRotation(currentXRotation, currentZRotation));
    }
    private float NormalizeAngle(float angle)
    {
        // ������ -180������ 180�� ���̷� ��ȯ
        if (angle > 180)
            angle -= 360;
        return angle;
    }
    private IEnumerator ResetRotation(float currentXRotation, float currentZRotation)
    {
        Quaternion targetRotation = Quaternion.Euler(0f, transform.eulerAngles.y, 0f);

        // ������ ��ǥ ȸ�� ���·� �ڿ������� ȸ��
        while (Quaternion.Angle(transform.rotation, targetRotation) > 0.1f)
        {
            float step = resetSpeed * Time.deltaTime;

            // X��� Z�� ȸ�� ������ ����
            float newXRotation = Mathf.MoveTowardsAngle(transform.eulerAngles.x, 0f, step);
            float newZRotation = Mathf.MoveTowardsAngle(transform.eulerAngles.z, 0f, step);

            // ȸ�� ����
            transform.eulerAngles = new Vector3(newXRotation, transform.eulerAngles.y, newZRotation);

            yield return null;
        }

        // ���������� ��Ȯ�ϰ� 0���� ����
        transform.rotation = targetRotation;
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

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
            collision.gameObject.GetComponent<Enemy>().GetDamage(15);
    }
}