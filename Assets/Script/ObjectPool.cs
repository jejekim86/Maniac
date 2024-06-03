using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ObjectPool<T> where T : MonoBehaviour
{
    [SerializeField] T targetObject;
    
    [SerializeField][Range(1, 10000)] int poolingAmount = 1;
    Transform containerObject;

    Queue<T> objectPool;

    public bool Initialize(T value = null)
    {
        if (value)
        {
            targetObject = value;
        }

        if (!targetObject || containerObject)
        {
            return false;
        }

        if (1 > poolingAmount)
        {
            poolingAmount = 1;
        }

        System.Text.StringBuilder sb = new System.Text.StringBuilder();

        sb.Append(targetObject.name);

        sb.Append("을 담고 있는 Pool Container");

        containerObject = new GameObject(sb.ToString()).transform;

        objectPool = new Queue<T>();

        MakeAndPooling();

        return true;
    }

    bool MakeAndPooling()
    {
        if (!containerObject)
        {
            return false;
        }

        T poolObject;

        for (int i = 0; poolingAmount > i; i++)
        {
            poolObject = MonoBehaviour.Instantiate(targetObject, containerObject);
            poolObject.name = targetObject.name;
            poolObject.gameObject.SetActive(false);
            objectPool.Enqueue(poolObject);
        }
        return true;
    }

    /// <summary> item 하나를 Pool에서 꺼내 활성화 시킨다. </summary>
    public bool GetObject(out T item)
    {
        item = null;

        if (!containerObject)
        {
            return false;
        }

        if (0 >= objectPool.Count)
        {
            if (!MakeAndPooling()) return false;
        }

        item = objectPool.Dequeue();
        item.gameObject.SetActive(true);
        return true;
    }

    /// <summary> item을 비활성화 시키고 Pool에 넣는다. </summary>
    public bool PutInPool(T item)
    {
        if (!(item && containerObject))
        {
            return false;
        }

        item.gameObject.SetActive(false);
        objectPool.Enqueue(item);
        return true;
    }

    public bool Destroy()
    {
        if (!containerObject)
        {
            return false;
        }

        MonoBehaviour.Destroy(containerObject.gameObject);

        containerObject = null;
        objectPool.Clear();
        objectPool = null;
        return true;
    }

    public void ReturnBackPool()
    {
        if (containerObject)
        {
            foreach (Transform child in containerObject)
            {
                if (child.gameObject.activeSelf)
                {
                    if (child.TryGetComponent(out T item))
                    {
                        PutInPool(item);
                    }
                }
            }
        }
    }

    public int ObjectCount()
    {
        return objectPool.Count;
    }

    public void SetParentTocontainerObject(Transform child)
    {
        if (child.parent.name != containerObject.name)
        {
            child.transform.parent = containerObject;
        }
    }

    public string Getname()
    {
        return targetObject.name;
    }
}
