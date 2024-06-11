//using System.Collections;
//using System.Collections.Generic;
//using Unity.Collections;
//using Unity.Jobs;
//using UnityEngine;

//[System.Serializable]
//public class BulletTest<T> where T : MonoBehaviour
//{
//    [SerializeField] T targetObject;
//    [SerializeField][Range(1, 10000)] int poolingAmount = 10000;
//    Transform containerObject;
//    Queue<T> objectPool;

//    public bool Initialize(T value = null)
//    {
//        if (value)
//        {
//            targetObject = value;
//        }

//        if (!targetObject || containerObject)
//        {
//            return false;
//        }

//        if (1 > poolingAmount)
//        {
//            poolingAmount = 1;
//        }

//        var sb = new System.Text.StringBuilder();
//        sb.Append(targetObject.name).Append(" Pool Container");

//        containerObject = new GameObject(sb.ToString()).transform;
//        objectPool = new Queue<T>();

//        MakeAndPooling();

//        return true;
//    }

//    void MakeAndPooling()
//    {
//        if (!containerObject)
//        {
//            return;
//        }

//        var job = new PoolingJob
//        {
//            targetObject = targetObject,
//            container = containerObject,
//            poolingAmount = poolingAmount,
//            objectPool = new NativeQueue<Transform>(Allocator.TempJob)
//        };

//        var jobHandle = job.Schedule(poolingAmount, 64);
//        jobHandle.Complete();

//        while (job.objectPool.Count > 0)
//        {
//            var poolObject = job.objectPool.Dequeue().GetComponent<T>();
//            objectPool.Enqueue(poolObject);
//        }

//        job.objectPool.Dispose();
//    }

//    struct PoolingJob : IJobParallelFor
//    {
//        public T targetObject;
//        public Transform container;
//        public int poolingAmount;
//        public NativeQueue<Transform> objectPool;

//        public void Execute(int index)
//        {
//            var poolObject = Object.Instantiate(targetObject, container);
//            poolObject.name = targetObject.name;
//            poolObject.gameObject.SetActive(false);
//            objectPool.Enqueue(poolObject.transform);
//        }
//    }

//    public void GetObjects(int count, out NativeArray<T> items)
//    {
//        items = new NativeArray<T>(count, Allocator.TempJob);

//        var job = new GetObjectsJob
//        {
//            containerObject = containerObject,
//            objectPool = objectPool,
//            items = items
//        };

//        var jobHandle = job.Schedule(count, 64);
//        jobHandle.Complete();
//    }

//    struct GetObjectsJob : IJobParallelFor
//    {
//        public Transform containerObject;
//        public Queue<T> objectPool;
//        public NativeArray<T> items;

//        public void Execute(int index)
//        {
//            T npc = null;

//            lock (objectPool)
//            {
//                if (objectPool.Count > 0)
//                {
//                    npc = objectPool.Dequeue();
//                }
//            }

//            if (npc != null)
//            {
//                npc.gameObject.SetActive(true);
//                items[index] = npc;
//            }
//        }
//    }

//    public bool PutInPool(T npc)
//    {
//        if (!(npc && containerObject))
//        {
//            return false;
//        }

//        npc.gameObject.SetActive(false);

//        lock (objectPool)
//        {
//            objectPool.Enqueue(npc);
//        }

//        return true;
//    }

//    public bool Destroy()
//    {
//        if (!containerObject)
//        {
//            return false;
//        }

//        MonoBehaviour.Destroy(containerObject.gameObject);

//        containerObject = null;
//        objectPool.Clear();
//        return true;
//    }

//    public void ReturnBackPool()
//    {
//        if (containerObject)
//        {
//            foreach (Transform child in containerObject)
//            {
//                if (child.gameObject.activeSelf)
//                {
//                    if (child.TryGetComponent(out T npc))
//                    {
//                        PutInPool(npc);
//                    }
//                }
//            }
//        }
//    }

//    public int ObjectCount()
//    {
//        return objectPool.Count;
//    }

//    public void SetParentToContainerObject(Transform child)
//    {
//        if (child.parent.name != containerObject.name)
//        {
//            child.transform.parent = containerObject;
//        }
//    }

//    public string GetName()
//    {
//        return targetObject.name;
//    }
//}
