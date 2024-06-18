using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameManager;
using static UnityEngine.UI.GridLayoutGroup;

public class Bullet : MonoBehaviour
{
    public delegate bool BackInPool();
    public BackInPool backInPool;

    [SerializeField]
    private float bulletSpeed;

    [SerializeField]
    private Collider myCollider;
    [SerializeField] private float basicDamage = 5f;

    private float damage = 0;

    Vector3 dir;
    float timeCount;
    float distance = 0;
    private Vector3 startpos;
    private Vector3 endpos;

    private void OnEnable()
    {
        damage = basicDamage + basicDamage * GameManager.Instance.inGameUpgradeData1[(int)InGameUpgrade.damage] * 0.01f;
        Debug.Log(damage);
    }

    public void SetData(Vector3 firPos, Vector3 dir)
    {
        this.dir = dir;
    }

    public void SetDirection(Transform tr)
    {
        transform.position = tr.position;
        transform.rotation = tr.rotation;
        startpos = tr.position;
        endpos = tr.position + tr.forward * 10;
        //Debug.LogError("좌표 설정");
    }

    public void Terminate()
    {
        timeCount = 0;
    }

    void Update()
    {
        timeCount += Time.deltaTime;  
        transform.position = Vector3.Lerp(startpos, endpos, timeCount * 2);

        if(timeCount >= 0.5)  
        {
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
            other.gameObject.GetComponent<Enemy>().GetDamage(damage);
            Terminate();
            PoolManager.instance.bulletPool.PutInPool(this); // 총알을 풀에 다시 넣음
        }
        else if (other.gameObject.CompareTag("Player"))
        {
            other.gameObject.GetComponent<Player>().GetDamage(0.1f);
            Terminate();
            PoolManager.instance.bulletPool.PutInPool(this); // 총알을 풀에 다시 넣음
        }
        else if (other.gameObject.CompareTag("Vehicle"))
        {
            other.gameObject.GetComponent<Vehicle>().GetDamage(0.1f);
            Terminate();
            PoolManager.instance.bulletPool.PutInPool(this); // 총알을 풀에 다시 넣음
        }
    }
}
