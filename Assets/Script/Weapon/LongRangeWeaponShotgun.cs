using System.Collections;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

public class LongRangeWeaponShotgun : LongRangeWeapon
{
    [SerializeField] private int numberBulletsFire;

    NativeArray<Vector3> bulletsTransform;
    NativeArray<Vector3> bulletsTargetTransform;

    bool finishShoot = false;

    public override void SetData()
    {
        base.SetData();
    }

    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        base.Update();
    }
    Bullet newBullet;
    public override bool Attack()
    {
        if (timeCount < reloadT)
        {
            return false;
        }

        /*
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
        */
        StartCoroutine(CoShootBullet());
        
        timeCount = 0;
        return true;
    }

    struct ShootBulletJob : IJobParallelFor
    {
        public Quaternion startAngle;
        public Quaternion endAngle;
        public int numberBulletsFire;

        public NativeArray<Quaternion> rot;
        public void Execute(int index)
        {
            //Quaternion rotation = Quaternion.Lerp(startAngle, endAngle, (float)1 / numberBulletsFire * index);
            //rot[index] = rotation;
            rot[index] = Quaternion.Lerp(startAngle, endAngle, (float)1 / numberBulletsFire * index);
        }
    }

    Bullet[] bullets;

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

        bulletsTransform = new NativeArray<Vector3>(numberBulletsFire, Allocator.TempJob);
        bulletsTargetTransform = new NativeArray<Vector3>(numberBulletsFire, Allocator.TempJob);

        PoolManager.instance.bulletPool.GetObjects(out bullets, numberBulletsFire);
        for (int i = 0; i < numberBulletsFire; i++)
        {
            //bullets[i].backInPool += () => PoolManager.instance.bulletPool.PutInPool(bullets[i]);
            bullets[i].transform.position = fireTr.position;
            bullets[i].transform.rotation = rotation[i];

            bulletsTransform[i] = bullets[i].transform.position;
            bulletsTargetTransform[i] = bullets[i].transform.position + bullets[i].transform.forward * 10;

            if (i % 1000 == 0)
            {
                yield return null;
            }
        }
        StartCoroutine(CoMoveBullet());
    }


    IEnumerator CoMoveBullet()
    {
        float StartTime = Time.time;

        NativeArray<Vector3> lerpTransform = new NativeArray<Vector3>(numberBulletsFire, Allocator.TempJob);
        MoveBulletJob moveBulletJob;

        moveBulletJob.bulletsTransform = bulletsTransform;
        moveBulletJob.bulletsTargetTransform = bulletsTargetTransform;
        moveBulletJob.lerpTransform = lerpTransform;

        while (Time.time - StartTime <= 1.0f)
        {
            moveBulletJob.lerpT = Time.time - StartTime;

            JobHandle jobHandle = moveBulletJob.Schedule(numberBulletsFire, numberBulletsFire / 3);
            jobHandle.Complete();

            for (int i = 0; i < numberBulletsFire; i++)
            {
                bullets[i].transform.position = lerpTransform[i];
            }

            yield return null;
        }
        for(int i = 0; i < numberBulletsFire; i++)
        {
            PoolManager.instance.bulletPool.PutInPool(bullets[i]);
        }
        finishShoot = false;
    }

    struct MoveBulletJob : IJobParallelFor
    {
        public float lerpT;
        public NativeArray<Vector3> bulletsTransform;
        public NativeArray<Vector3> bulletsTargetTransform;

        public NativeArray<Vector3> lerpTransform;
        public void Execute(int index)
        {
            lerpTransform[index] = Vector3.Lerp(bulletsTransform[index], bulletsTargetTransform[index], lerpT);
        }
    }

}