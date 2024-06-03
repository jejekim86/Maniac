using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField]
    private float bulletSpeed;

    Vector3 dir;
    float timeCount;

    public void SetData(Vector3 firPos, Vector3 dir)
    {
        this.dir = dir;
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
        if (other.CompareTag("Enemy"))
        {
            
        }

        // 오브젝트풀로 돌아가기
    }
}
