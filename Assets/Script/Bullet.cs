using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Bullet;
using static UnityEngine.UI.GridLayoutGroup;

public class Bullet : MonoBehaviour
{
    public delegate bool BackInPool();
    public BackInPool backInPool;

    [SerializeField]
    private float bulletSpeed;

    Vector3 dir;
    float timeCount;

    public GameObject owner; // 총알을 발사한 객체

    public void SetData(Vector3 firPos, Vector3 dir)
    {
        this.dir = dir;
    }

    void Update()
    {
        if(timeCount >= 1)  
        {
            PoolManager.instance.bulletPool.PutInPool(this);
            //backInPool.Invoke();
            timeCount = 0;
        }
        transform.position = Vector3.Lerp(transform.position, transform.position + transform.forward * 10, timeCount);
        timeCount += Time.deltaTime;  
    }

    private void OnTriggerEnter(Collider other)
    {
        // 최상위 부모 오브젝트 가져오기
        Transform parent = other.transform;
        while (parent.parent != null)
        {
            parent = parent.parent;
        }

        // 자기가 쏜 총알 무시
        if (parent.gameObject == owner || other.gameObject == owner)
        {
            return;
        }

        if (other.CompareTag("Enemy"))
        {
            other.gameObject.GetComponent<Enemy>().GetDamage(5f);
            //PoolManager.instance.bulletPool.PutInPool(this); // 총알을 풀에 다시 넣음
        }
        else if (other.CompareTag("Player"))
        {
            other.gameObject.GetComponent<Player>().GetDamage(0.1f);
            //PoolManager.instance.bulletPool.PutInPool(this); // 총알을 풀에 다시 넣음
        }
    }
}
