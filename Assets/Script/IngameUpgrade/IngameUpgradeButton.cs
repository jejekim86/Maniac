using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class IngameUpgradeButton : MonoBehaviour, IPointerEnterHandler
{
    [SerializeField] private Text infotext;
    [SerializeField] private GameObject upgaradePanel;

    private SkillDataStruct skillData = new SkillDataStruct();

    public void OnPointerEnter(PointerEventData eventData)
    {
        infotext.text = skillData.skillInfo;
    }

    public void SetSkillData(SkillDataStruct skillData)
    {
        this.skillData = skillData;
    }

    public void OnClickExit()
    {
        // 게임 실행
        Time.timeScale = 1f;
        // 패널 닫기
        upgaradePanel.SetActive(false);
    }

    public void OnClickUpgrade()
    {
        // 업그레이드 로직



        // 게임 실행
        Time.timeScale = 1f;
        // 패널 닫기
        upgaradePanel.SetActive(false);
    }
}
