using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SkillManager : MonoBehaviour
{
    [SerializeField] private DBManager dbManager = new DBManager();
    [SerializeField] private GameObject skillTextPrefab;
    [SerializeField] private GameObject skillButtonPrefab;
    [SerializeField] private Transform skillTextContainer;
    [SerializeField] private Transform skillButtonContainer;

    private List<SkillDataStruct> skills;
    private Dictionary<string, GameObject> skillInfoPanels = new Dictionary<string, GameObject>();

    void Start()
    {
        LoadSkills();
    }

    void LoadSkills()
    {
        skills = dbManager.GetSkillData();

        for (int i = 0; i < skills.Count; i++)
        {
            var skill = skills[i];

            GameObject skillText = Instantiate(skillTextPrefab, skillTextContainer);
            skillText.SetActive(false); // �ʱ⿡�� ��Ȱ��ȭ

            // �ؽ�Ʈ ��� ����
            skillText.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = skill.skillName;
            skillText.transform.GetChild(0).GetChild(1).GetComponent<Text>().text = skill.skillInfo;
            skillText.transform.GetChild(0).GetChild(2).GetComponent<Text>().text = skill.increase.ToString();

            // ��ų ���� �г��� ��ųʸ��� ����
            skillInfoPanels[skill.skillName] = skillText;

            GameObject skillButton = Instantiate(skillButtonPrefab, skillButtonContainer);

            // ��ư ��� ���� �� ��ų �̸� ����
            Button upgradeButton = skillButton.transform.GetChild(0).GetComponent<Button>();
            string skillName = skill.skillName; // �ݵ�� ���� ���� ��� -> ��...?
            upgradeButton.onClick.AddListener(() => UpgradeSkill(skillName));

            // �̹��� ����
            Image skillImage = skillButton.transform.GetChild(0).GetChild(0).GetComponent<Image>();
            string imagePath = $"Skills/Skill_{i + 1}"; // �̹��� �̸� Skill_1, Skill_2, ... ����

            Sprite skillSprite = Resources.Load<Sprite>(imagePath);
            if (skillSprite != null)
            {
                skillImage.sprite = skillSprite;
            }
            else
            {
                Debug.LogError($"�̹����� �ε��� �� �����ϴ�: {imagePath}");
            }

            // EventTrigger �߰�
            EventTrigger trigger = skillButton.AddComponent<EventTrigger>();

            // PointerEnter �̺�Ʈ �߰�
            EventTrigger.Entry entryEnter = new EventTrigger.Entry();
            entryEnter.eventID = EventTriggerType.PointerEnter;
            entryEnter.callback.AddListener((eventData) => { ShowSkillInfo(skillName); });
            trigger.triggers.Add(entryEnter);

            // PointerExit �̺�Ʈ �߰�
            EventTrigger.Entry entryExit = new EventTrigger.Entry();
            entryExit.eventID = EventTriggerType.PointerExit;
            entryExit.callback.AddListener((eventData) => { HideSkillInfo(skillName); });
            trigger.triggers.Add(entryExit);
        }
    }

    void ShowSkillInfo(string skillName)
    {
        if (skillInfoPanels.TryGetValue(skillName, out GameObject skillText))
        {
            skillText.SetActive(true);
        }
        else
        {
            Debug.LogError($"��ų ������ ã�� �� �����ϴ�: {skillName}");
        }
    }

    void HideSkillInfo(string skillName)
    {
        if (skillInfoPanels.TryGetValue(skillName, out GameObject skillText))
        {
            skillText.SetActive(false);
        }
        else
        {
            Debug.LogError($"��ų ������ ã�� �� �����ϴ�: {skillName}");
        }
    }

    void UpgradeSkill(string skillName)
    {
        int userId = 1; // ���� ����� ID�� ���� �������� �����Ǿ�� �մϴ�
        bool success = dbManager.UpdateSkillLevelData(skillName, 1, userId);

        if (success)
        {
            Debug.Log("��ų ���׷��̵忡 �����߽��ϴ�.");
        }
        else
        {
            Debug.LogError("��ų ���׷��̵忡 �����߽��ϴ�.");
        }
    }
}