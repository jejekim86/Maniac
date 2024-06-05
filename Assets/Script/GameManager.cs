 using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public sealed class GameManager : MonoBehaviour
{
    //[SerializeField] UIManager Uimanager;
    //[SerializeField] Datamanager Datamanager;

    //[SerializeField] private Text timerText;
    //public Image[] images;

    int Level = 0;
    float curTime;
    float maxTime;

    int minute;
    int second;

    private static GameManager instance = null;


    void Awake()
    {
        if (null == instance)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);

        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 120;
    }
    public static GameManager Instance
    {
        get
        {
            if (null == instance)
                return null;
            return instance;
        }
    }

    private void Start()
    {
        maxTime = 300f;
        //StartCoroutine(StartTimer());
    }

    //IEnumerator StartTimer()
    //{
    //    curTime = maxTime;
    //
    //    while (curTime > 0)
    //    {
    //        curTime -= 1;
    //        minute = (int)curTime / 60;
    //        second = (int)curTime % 60;
    //        timerText.text = minute.ToString("00") + ":" + second.ToString("00");
    //        yield return new WaitForSeconds(1);
    //
    //        // 10초 마다 이미지를 변경
    //        if ((int)curTime % 10 == 0 && Level < images.Length)
    //        {
    //            images[Level].gameObject.SetActive(true);
    //            Level++; // 다음 레벨로 진행
    //        }
    //
    //        if (curTime <= 0)
    //        {
    //            Debug.Log("생존 성공"); // 결과 창 출력 코드로 변경
    //            curTime = 0;
    //            yield break;
    //        }
    //    }
    //}

    private void Update()
    {
        
    }
}
