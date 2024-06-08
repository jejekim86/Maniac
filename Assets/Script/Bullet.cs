using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.UI.GridLayoutGroup;

public class Bullet : MonoBehaviour
{
    public delegate bool BackInPool();
    public BackInPool backInPool;

    [SerializeField]
    private float bulletSpeed;

    [SerializeField]
    private Collider myCollider;

    Vector3 dir;
    float timeCount;
    float distance = 0;
    private Vector3 startpos;
    private Vector3 endpos;
    public void SetData(Vector3 firPos, Vector3 dir)
    {
        this.dir = dir;
    }

    public void SetDirection(Transform tr)
    {
        transform.position = tr.position;
        transform.rotation = tr.rotation;
        Terminate();
    }

    public void Terminate()
    {
        startpos = transform.position;
        endpos = transform.position + transform.forward * 3;
    }

    void Update()
    {
        timeCount += Time.deltaTime;  
        transform.position = Vector3.Lerp(startpos, endpos, timeCount);

        if(timeCount >= 1)  
        {
            timeCount = 0;
            Terminate();
            PoolManager.instance.bulletPool.PutInPool(this);
        }
    }
    

    public void SetCollider(bool value)
    {
        myCollider.enabled = value;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            Debug.Log("총알 히트");
            other.gameObject.GetComponent<Enemy>().GetDamage(5f);
            PoolManager.instance.bulletPool.PutInPool(this); // 총알을 풀에 다시 넣음
        }
        else if (other.gameObject.CompareTag("Player"))
        {
            other.gameObject.GetComponent<Player>().GetDamage(0.1f);
            PoolManager.instance.bulletPool.PutInPool(this); // 총알을 풀에 다시 넣음
        }
    }
}
