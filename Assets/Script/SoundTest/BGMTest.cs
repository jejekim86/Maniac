using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGMTest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        SoundManager.Instance.PlayBGM(BGM.menuBGM);
        Invoke("play1", 10f);
    }

    void play1()
    {
        SoundManager.Instance.PlayBGM(BGM.inGameDefault);
        Invoke("play2", 10f);
    }

    void play2()
    {
        SoundManager.Instance.PlayBGM(BGM.inGameChase);

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
