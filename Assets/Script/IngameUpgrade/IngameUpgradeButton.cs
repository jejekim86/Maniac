using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class IngameUpgradeButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Text infoText;
    [SerializeField] private Text nameText;
    [SerializeField] private GameObject upgaradePanel;

    private RectTransform myRectTransform;
    private Image myImage;
    private Coroutine onMouserCoroutine;
    private int num;

    private SkillDataStruct skillData = new SkillDataStruct();

    private void Start()
    {
        myRectTransform = GetComponent<RectTransform>();
        myImage = GetComponent<Image>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        infoText.text = skillData.skillInfo;
        nameText.text = skillData.skillName;
        onMouserCoroutine = StartCoroutine(MouseOn());
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        StopCoroutine(onMouserCoroutine);
        myRectTransform.rotation = Quaternion.identity;
    }

    public void SetSkillData(SkillDataStruct skillData, Sprite sprite, int num)
    {
        this.skillData = skillData;
        this.num = num;
        myImage.sprite = sprite;
    }

    public void OnClickExit()
    {
        // ���� ����
        Time.timeScale = 1f;
        // �г� �ݱ�
        upgaradePanel.SetActive(false);
    }

    public void OnClickUpgrade()
    {
        if (skillData.skillName == "ü�� ȸ��")
            // �÷��̾� ü�� ȸ��
            GameManager.Instance.Player.AddHp(skillData.increase);
        else
            GameManager.Instance.inGameUpgradeData1[num] += skillData.increase;

        // �ؽ�Ʈ �ʱ�ȭ
        infoText.text = "";
        nameText.text = "";
        // ���� ����
        Time.timeScale = 1f;
        // �г� �ݱ�
        upgaradePanel.SetActive(false);
    }

    IEnumerator MouseOn()
    {
        float timeCount = 0;
        float angle;
        float cos;
        while (true)
        {
            cos = Mathf.Cos(timeCount * 10);
            angle = 10 * cos;
            myRectTransform.localScale = Vector3.one + Vector3.one * 0.1f * cos;
            myRectTransform.rotation = Quaternion.Euler(0, 0, angle);
            timeCount += Time.unscaledDeltaTime;
            yield return null;
        }
    }
}
