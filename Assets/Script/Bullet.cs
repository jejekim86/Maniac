using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField]
    private float bulletSpeed;

    Vector3 dir;
    float timeCount;

    public GameObject owner; // �Ѿ��� �߻��� ��ü

    public void SetData(Vector3 firPos, Vector3 dir, GameObject owner)
    {
        transform.position = firPos; // �Ѿ��� ��ġ�� �߻� ��ġ�� �ʱ�ȭ
        this.dir = dir.normalized; // ���� ���͸� ����ȭ
        this.dir = dir;
        this.owner = owner;
    }

    void Update()
    {
        if(timeCount >= 1)  
        {
            PoolManager.instance.bulletPool.PutInPool(this);
            timeCount = 0;
        }
        transform.position = Vector3.Lerp(transform.position, transform.position + transform.forward * 10, timeCount);
        timeCount += Time.deltaTime;  
    }

    private void OnTriggerEnter(Collider other)
    {
        // �ֻ��� �θ� ������Ʈ ��������
        Transform parent = other.transform;
        while (parent.parent != null)
        {
            parent = parent.parent;
        }

        // �ڱⰡ �� �Ѿ� ����
        if (parent.gameObject == owner || other.gameObject == owner)
        {
            return;
        }

        if (other.CompareTag("Enemy"))
        {
            other.gameObject.GetComponent<Enemy>().GetDamage(5f);
            //PoolManager.instance.bulletPool.PutInPool(this); // �Ѿ��� Ǯ�� �ٽ� ����
        }
        else if (other.CompareTag("Player"))
        {
            other.gameObject.GetComponent<Player>().GetDamage(0.1f);
            //PoolManager.instance.bulletPool.PutInPool(this); // �Ѿ��� Ǯ�� �ٽ� ����
        }
    }
}
