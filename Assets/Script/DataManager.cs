using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Xml;

public class DataManager : MonoBehaviour
{
    public static DataManager instance = null;


    void Awake()
    {
        if (null == instance)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);
    }
    public static DataManager Instance
    {
        get
        {
            if (null == instance)
                return null;
            return instance;
        }
    }
    void Awake()
    {
        instance = this;
    }
}
