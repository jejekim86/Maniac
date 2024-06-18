 using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public sealed class GameManager : MonoBehaviour
{
    //[SerializeField] UIManager Uimanager;
    //[SerializeField] Datamanager Datamanager;

    [SerializeField] private Button startButton;
    [SerializeField] private Button backButton;
    [SerializeField] private Button checkButton;
    [SerializeField] private Text timerText;
    [SerializeField] private Image[] images;

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
        
        if (timerText)
        {
            StartCoroutine(StartTimer());
        }
        if (backButton)
        {
            backButton.onClick.AddListener(StartButtonOnClick);
        }
        if (backButton)
        {
            backButton.onClick.AddListener(BackButtonOnClick);
        }
        if (checkButton)
        {
            checkButton.onClick.AddListener(CheckButtonOnClick);
        }
    }

    IEnumerator StartTimer()
    {
        curTime = maxTime;
    
        while (curTime > 0)
        {
            curTime -= 1;
            minute = (int)curTime / 60;
            second = (int)curTime % 60;
            timerText.text = minute.ToString("00") + ":" + second.ToString("00");
            yield return new WaitForSeconds(1);
    
            // 10초 마다 이미지를 변경
            if ((int)curTime % 10 == 0 && Level < images.Length)
            {
                images[Level].gameObject.SetActive(true);
                Level++; // 다음 레벨로 진행
            }
    
            if (curTime <= 0)
            {
                Debug.Log("생존 성공"); // 결과 창 출력 코드로 변경
                curTime = 0;
                yield break;
            }
        }
    }

    public void StartButtonOnClick()
    {
        SceneManager.LoadScene("SkillUpgrade", LoadSceneMode.Single);
    }

    public void BackButtonOnClick()
    {
        SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
    }

    public void CheckButtonOnClick()
    {
        SceneManager.LoadScene("Game", LoadSceneMode.Single);
    }
}
