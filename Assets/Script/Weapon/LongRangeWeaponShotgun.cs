using System.Collections;
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
        // ���� ������ ���� ���� (�ʿ� �� �߰�)
    }

    NativeArray<Vector3> bulletsTransform;
    NativeArray<Vector3> bulletsTargetTransform;
    NativeArray<Quaternion> rotation;

    public override void SetData()
    {
        base.SetData();
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
            rot[index] = Quaternion.Lerp(startAngle, endAngle, (float)1 / numberBulletsFire * index);
        }
    }

    Bullet[] bullets;

    IEnumerator CoShootBullet()
    {
        Quaternion startAngle = fireTr.rotation * Quaternion.Euler(0, -15, 0);
        Quaternion endAngle = fireTr.rotation * Quaternion.Euler(0, 15, 0);

        rotation = new NativeArray<Quaternion>(numberBulletsFire, Allocator.TempJob);

        ShootBulletJob shootBulletJob;
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

        bool onCollider = false;

        while (Time.time - StartTime <= 1.0f)
        {
            moveBulletJob.lerpT = Time.time - StartTime;

            JobHandle jobHandle = moveBulletJob.Schedule(numberBulletsFire, numberBulletsFire / 3);
            jobHandle.Complete();

            for (int i = 0; i < numberBulletsFire; i++)
            {
                bullets[i].transform.position = lerpTransform[i];
            }

            if (!onCollider)
            {
                for(int i = 0; i < numberBulletsFire; i++)
                {
                    bullets[i].SetCollider(true);
                }
            }

            BulletRenderer.Instance.RenderBulletsAtPositions(lerpTransform, rotation);

            yield return null;
        }
        for(int i = 0; i < numberBulletsFire; i++)
        {
            PoolManager.instance.bulletPool.PutInPool(bullets[i]);
        }
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