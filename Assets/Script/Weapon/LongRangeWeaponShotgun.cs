using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.UI;

public class LongRangeWeaponShotgun : LongRangeWeapon
{
    [SerializeField] private int numberBulletsFire;
    [SerializeField] private Slider coolTime_Slider;

    private void Start()
    {
        coolTime_Slider.gameObject.SetActive(false);
        coolTime_Slider.maxValue = reloadT;
        coolTime_Slider.value = reloadT;
    }

    public override void SetData()
    {
        // 무기 데이터 설정 로직 (필요 시 추가)
    }

    public override bool Attack()
    {
        if (timeCount >= reloadT)
        {
            StartCoroutine(FireBullet());
            StartCoroutine(UpdateCooldownSlider());
            timeCount = 0;
            return true;
        }
        return false;
    }

    private IEnumerator FireBullet()
    {
        Bullet newBullet;
        PoolManager.instance.bulletPool.GetObject(out newBullet);
        newBullet.transform.position = fireTr.position;
        newBullet.transform.rotation = fireTr.rotation;
        yield return null;
    }

    private IEnumerator UpdateCooldownSlider()
    {
        coolTime_Slider.gameObject.SetActive(true);
        float elapsed = 0f;

        while (elapsed < reloadT)
        {
            elapsed += Time.deltaTime;
            coolTime_Slider.value = Mathf.Lerp(0, coolTime_Slider.maxValue, elapsed / reloadT);
            yield return null;
        }

        coolTime_Slider.value = coolTime_Slider.maxValue;
        coolTime_Slider.gameObject.SetActive(false);
    }


    /*public override bool Attack()
    {
        if (timeCount < reloadT)
        {
            return false;
        }
        
        *//*
        Quaternion startAngle = fireTr.rotation * Quaternion.Euler(0, -15, 0);
        Quaternion endAngle = fireTr.rotation * Quaternion.Euler(0, 15, 0);

        ShootBulletJob shootBulletJob;

        var rotation = new NativeArray<Quaternion>(numberBulletsFire, Allocator.TempJob);
        shootBulletJob.startAngle = startAngle;
        shootBulletJob.endAngle = endAngle;
        shootBulletJob.numberBulletsFire = numberBulletsFire;
        shootBulletJob.rot = rotation;

        JobHandle jobHandle = shootBulletJob.Schedule(numberBulletsFire, numberBulletsFire / 3);

        jobHandle.Complete();

        Bullet[] bullets;
        PoolManager.instance.bulletPool.GetObjects(out bullets, numberBulletsFire);
        for (int i = 0; i < bullets.Length; i++)
        {
            bullets[i].backInPool += () => PoolManager.instance.bulletPool.PutInPool(bullets[i]);
            bullets[i].transform.position = fireTr.position;
            bullets[i].transform.rotation = rotation[i];
        }
        *//*
        StartCoroutine(CoShootBullet());
        timeCount = 0;
        return true;
    }*/

    struct ShootBulletJob : IJobParallelFor
    {
        public Quaternion startAngle;
        public Quaternion endAngle;
        public int numberBulletsFire;

        public NativeArray<Quaternion> rot;
        public void Execute(int index)
        {
            Quaternion rotation = Quaternion.Lerp(startAngle, endAngle, (float)1 / numberBulletsFire * index);
            rot[index] = rotation;
        }
    }
    
    IEnumerator CoShootBullet()
    {
        Quaternion startAngle = fireTr.rotation * Quaternion.Euler(0, -15, 0);
        Quaternion endAngle = fireTr.rotation * Quaternion.Euler(0, 15, 0);

        ShootBulletJob shootBulletJob;

        var rotation = new NativeArray<Quaternion>(numberBulletsFire, Allocator.TempJob);
        shootBulletJob.startAngle = startAngle;
        shootBulletJob.endAngle = endAngle;
        shootBulletJob.numberBulletsFire = numberBulletsFire;
        shootBulletJob.rot = rotation;

        JobHandle jobHandle = shootBulletJob.Schedule(numberBulletsFire, numberBulletsFire / 3);

        jobHandle.Complete();

        Bullet[] bullets;
        PoolManager.instance.bulletPool.GetObjects(out bullets, numberBulletsFire);
        for (int i = 0; i < bullets.Length; i++)
        {
            bullets[i].backInPool += () => PoolManager.instance.bulletPool.PutInPool(bullets[i]);
            bullets[i].transform.position = fireTr.position;
            bullets[i].transform.rotation = rotation[i];
            if (i % 1000 == 0)
            {
                yield return null;
            }
        }
    }
}