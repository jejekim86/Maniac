using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public sealed class GameManager : MonoBehaviour
{
    //[SerializeField] UIManager Uimanager;
    //[SerializeField] Datamanager Datamanager;
    public enum Upgrade
    {
        gameTime, moveSpeed, money, maxHealth,
        vehicleMaxSpeed, vehicleRobSpeed, damage, max
    }
    public enum InGameUpgrade
    {
        moveSpeed, money, vehicleMaxSpeed,
        vehicleRobSpeed, damage, healthRecovery, max
    }

    public float[] upgradeData1 = new float[(int)Upgrade.max];
    public float[] UpgradeData1 { get{ return upgradeData1; }}

    public float[] inGameUpgradeData1 = new float[(int)Upgrade.max];
    public float[] InGameUpgradeData1 { get { return inGameUpgradeData1; } }

    public struct UpgradeData
    {
        public float gameTime { get; private set; }
        public float moveSpeed { get; private set; }
        public int money { get; private set; }
        public float magnetDistance { get; private set; }
        public float health { get; private set; }
        public float vehicleMaxSpeed { get; private set; }
        public float vehicleRobSpeed { get; private set; }
        public float damage { get; private set; }
    }

    public struct IngameUpgradeData
    {
        public float moveSpeed { get; private set; }
        public int money { get; private set; }
        public float magnetDistance { get; private set; }
        public float vehicleMaxSpeed { get; private set; }
        public float vehicleRobSpeed { get; private set; }
        public float damage { get; private set; }
    }

    [SerializeField] private Button startButton;
    [SerializeField] private Button backButton;
    [SerializeField] private Button checkButton;
    [SerializeField] private Text timerText;
    [SerializeField] private Image[] images;
    [SerializeField] private Player player;
    public Player Player { get { return player; } }

    int Level = 0;
    float curTime;
    float maxTime;

    int minute;
    int second;

    private static GameManager instance = null;

    public UpgradeData upgradeData { get; private set; }
    public IngameUpgradeData ingameUpgradeData { get; private set; }

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
        maxTime = 300f - 300f * upgradeData.gameTime;

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

    public void SetUpgradeData(UpgradeData data)
    {
        upgradeData = data;
    }
}
