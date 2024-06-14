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

    List<SkillDataStruct> skillDatas;

    private void OnEnable()
    {
        //StartCoroutine(ShowPanel());
    }

    private void OnDisable()
    {
        for (int i = 0; i < buttons.Length; i++)
        {
            buttons[i].enabled = false;
        }
        myRectTransform.localScale = Vector3.zero;
    }

    IEnumerator ShowPanel()
    {
        float timeCount = 0;
        Vector3 endScale = Vector3.one;

        while (timeCount < 1f)
        {
            myRectTransform.localScale = Vector3.Lerp(myRectTransform.localScale, endScale, timeCount);
            timeCount += Time.deltaTime;
            yield return null;
        }

        StartCoroutine(ShowButton());
    }

    IngameUpgradeButton button;
    
    IEnumerator ShowButton()
    {
        List<SkillDataStruct> skillDatasClone = skillDatas;
        int i = 0;
        buttons[i].enabled = true;
        
        for (i = 1; i < 4; i++)
        {
            int num = Random.Range(0, skillDatasClone.Count);
            buttons[i].enabled = true;
            button = buttons[i].GetComponent<IngameUpgradeButton>();
            button.SetSkillData(skillDatasClone[num]);
            skillDatasClone.RemoveAt(num);
            yield return new WaitForSeconds(0.25f);
        }
    }

}
