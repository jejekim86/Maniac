using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UpgradePanelManager : MonoBehaviour
{
    [SerializeField] private RectTransform myRectTransform;
    [SerializeField] private Button buttonPrefab;
    [SerializeField] private Transform buttonsParent;

    [SerializeField] private Image[] buttons = new Image[4];
    [SerializeField] private GameObject[] upgradeButtons;
    [SerializeField] private Sprite[] upgradeButtonSprites;

    List<SkillDataStruct> skillDatas = new List<SkillDataStruct>();

    private void Start()
    {
        SetData();
    }

    private void OnEnable()
    {
        StartCoroutine(ShowPanel());
    }

    private void OnDisable()
    {
        for (int i = 0; i < buttons.Length; i++)
        {
            buttons[i].enabled = false;
        }
        myRectTransform.localScale = Vector3.zero;
    }

    void SetData()
    {
        skillDatas = DBManagerTest.instance.GetIngameSkillData();
    }

    IEnumerator ShowPanel()
    {
        float timeCount = 0;
        Vector3 endScale = Vector3.one;

        while (timeCount < 1f)
        {
            myRectTransform.localScale = Vector3.Lerp(myRectTransform.localScale, endScale, timeCount);
            timeCount += Time.unscaledDeltaTime;
            yield return null;
        }

        StartCoroutine(ShowButton());
    }

    IngameUpgradeButton button;
    
    IEnumerator ShowButton()
    {
        int i = 0;
        buttons[i].enabled = true;

        HashSet<int> uniqueNumbers = new HashSet<int>();
        List<int> randomNums; 
        while (uniqueNumbers.Count < 3)
        {
            int num = Random.Range(0, skillDatas.Count);
            uniqueNumbers.Add(num); // 중복된 숫자는 자동으로 걸러짐
        }
        randomNums = new List<int>(uniqueNumbers);
        for (i = 1; i < 4; i++)
        {
            buttons[i].enabled = true;
            button = buttons[i].GetComponent<IngameUpgradeButton>();
            button.SetSkillData(skillDatas[randomNums[i - 1]], upgradeButtonSprites[randomNums[i - 1]], randomNums[i - 1]);
            yield return new WaitForSecondsRealtime(0.25f);
        }
    }

}
